using System.Diagnostics;
using Blazor.Extensions.Canvas.WebGL;
using MtgWeb.Core.Physics;
using MtgWeb.Core.Render;
using MtgWeb.Pages;
using Mesh = MtgWeb.Core.Render.Mesh;

namespace MtgWeb.Core;

public class Game : IDisposable
{
    private readonly WebGLContext _context;
    private readonly PhysicsWorld _physicsWorld;

    private Scene? _currentScene;

    private Stopwatch _stopwatch;

    // Temp
    private Camera? _camera;
    private readonly Mesh _quad = Mesh.Quad();
    private readonly Mesh _cube = Mesh.Cube();

    public Game(WebGLContext context)
    {
        _context = context;
        _physicsWorld = new PhysicsWorld();

        _stopwatch = new Stopwatch();
    }

    public async Task Init()
    {
        await LoadScene("Scene");

        _currentScene.Root
            .First(entity => entity.Name.Contains("Player"))
            .TryGetComponent(out _camera);

        await _quad.Init(_context);
        await _cube.Init(_context);
    }

    private async Task LoadScene(string name)
    {
        _currentScene = await Resources.LoadScene(name);
        await Shader.CompileAll(_context);
        _physicsWorld.Add(_currentScene);

        foreach (var entity in _currentScene.Root)
        {
            entity.BindHierarchy();
            entity.InitComponents();
        }

        foreach (var entity in _currentScene.Root)
        {
            entity.StartComponents();
        }
    }

    public async Task MainLoop()
    {
        Time.StartFrame(1.0f / 60f);
        _stopwatch.Restart();

        // TODO: Separate Physics loop
        // _physicsWorld.Simulation.Timestep(1.0f / 60f);

        Input.Update();
        await Update();
        Input.LateUpdate();
        await Render();

        _stopwatch.Stop();
        Time.EndFrame(_stopwatch);

        if (_stopwatch.ElapsedMilliseconds < 1000f / 60f)
        {
            await Task.Delay((int) (1000f / 60f - _stopwatch.ElapsedMilliseconds));
        }
    }

    private async Task Update()
    {
        foreach (var entity in _currentScene.Root)
        {
            entity.UpdateComponents();
        }
    }


    struct RenderData : IComparable<RenderData>
    {
        public Entity Entity;
        public Mesh Mesh;
        public Shader? Shader;

        public void Deconstruct(out Entity entity, out Mesh mesh, out Shader? shader)
        {
            entity = Entity;
            mesh = Mesh;
            shader = Shader;
        }

        public int CompareTo(RenderData other)
        {
            return Shader.Name.Length - other.Shader.Name.Length;
        }
    }

    private RenderData[] _renderData = new RenderData[256];

    private async Task Render()
    {
        foreach (var entity in _currentScene.Root)
        {
            entity.Transform.Update();
        }

        var clearColor = _camera.ClearColor;
        await _context.ViewportAsync(0, 0, MainView.Width, MainView.Height);
        await _context.ClearColorAsync(clearColor.X, clearColor.Y, clearColor.Z, clearColor.W);
        await _context.ClearAsync(BufferBits.COLOR_BUFFER_BIT);

        await _context.DisableAsync(EnableCap.CULL_FACE);
        await _context.EnableAsync(EnableCap.DEPTH_TEST);

        var _renderesCount = 0;
        foreach (var entity in _currentScene.Root)
        {
            if (entity.TryGetComponents<Renderer>(out var renderers))
            {
                foreach (var renderer in renderers)
                {
                    var shader = renderer.Shader;
                    if (shader == null)
                        continue;


                    if (renderer.MeshType == MeshType.None)
                        continue;


                    var mesh = renderer.MeshType switch
                    {
                        MeshType.Quad => _quad,
                        MeshType.Cube => _cube,
                        MeshType.None => null,
                    };
                    
                    _renderData[_renderesCount].Entity = entity;
                    _renderData[_renderesCount].Mesh = mesh;
                    _renderData[_renderesCount].Shader = shader;
                    _renderesCount++;
                }
            }
        }

        Array.Sort(_renderData, 0, _renderesCount);
        Shader? _currentShader = default;
        
        for (int i = 0; i < _renderesCount; i++)
        {
            var (entity, mesh, shader) = _renderData[i];


            if (_currentShader != shader)
            {
                _currentShader = shader;
                await shader.Bind(_context);

                // Global uniforms.
                await _context.UniformAsync(shader.Time, Time.CurrentTime);
                await _context.UniformMatrixAsync(shader.WorldToView, false, _camera.WorldToView);
                await _context.UniformMatrixAsync(shader.Projection, false, _camera.Projection);
                var cameraPosition = _camera.Entity.Transform.Position;
                await _context.UniformAsync(
                    shader.CameraPositionWS,
                    cameraPosition.X,
                    cameraPosition.Y,
                    cameraPosition.Z
                );
            }

            // Object specific.
            await _context.UniformMatrixAsync(shader.ObjectToWorld, false, entity.Transform.Matrix);
            await _context.UniformMatrixAsync(shader.InvObjectToWorld, false, entity.Transform.InvMatrix);

            await mesh.Bind(_context, shader);
            await _context.DrawElementsAsync(
                Primitive.TRIANGLES,
                mesh.Indices.Length,
                DataType.UNSIGNED_SHORT,
                0
            );
        }
    }

    public void Dispose()
    {
        _physicsWorld.Dispose();
    }
}