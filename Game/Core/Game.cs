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
    private PlayerController _player;
    private readonly Mesh _quad = Mesh.Quad();
    private readonly Mesh _cube = Mesh.Cube();
    private Shader? _checkerShader;

    public Game(WebGLContext context)
    {
        _context = context;
        _physicsWorld = new PhysicsWorld();

        _stopwatch = new Stopwatch();
    }

    public async Task Init()
    {
        await LoadScene("Scene");

        var _playerEntity = _currentScene.Root.First(entity => entity.Name.Contains("Player"));
        _camera = _playerEntity.Components.OfType<Camera>().First();

        await _quad.Init(_context);
        await _cube.Init(_context);
        _checkerShader = await Shader.Load(_context, "Checker");
    }

    private async Task LoadScene(string name)
    {
        _currentScene = await Resources.LoadScene(name);
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
        Time.Tick(0.032f);
        _stopwatch.Restart();

        _physicsWorld.Simulation.Timestep(0.032f);

        Input.Update();
        await Update();
        Input.LateUpdate();
        await Render();

        _stopwatch.Stop();

        if (_stopwatch.ElapsedMilliseconds < 32)
        {
            await Task.Delay(32 - (int) _stopwatch.ElapsedMilliseconds);
        }
    }

    private async Task Update()
    {
        foreach (var entity in _currentScene.Root)
        {
            entity.UpdateComponents();
        }
    }

    private async Task Render()
    {
        // _camera.Transform.Update();

        foreach (var entity in _currentScene.Root)
        {
            entity.Transform.Update();
        }

        await _context.DisableAsync(EnableCap.CULL_FACE);
        await _context.EnableAsync(EnableCap.DEPTH_TEST);

        var clearColor = _camera.ClearColor;
        await _context.ClearColorAsync(clearColor.X, clearColor.Y, clearColor.Z, clearColor.W);
        await _context.ClearAsync(BufferBits.COLOR_BUFFER_BIT);

        await _context.ViewportAsync(0, 0, MainView.Width, MainView.Height);

        await _checkerShader.Bind(_context);

        await _context.UniformAsync(_checkerShader.Time, Time.CurrentTime);
        await _context.UniformMatrixAsync(_checkerShader.WorldToView, false, _camera.Entity.Transform.WorldToView);
        await _context.UniformMatrixAsync(_checkerShader.Projection, false, _camera.Projection);

        await _quad.Bind(_context, _checkerShader);
        foreach (var entity in _currentScene.Root)
        {
            if (entity.MeshType == MeshType.Quad)
            {
                await _context.UniformMatrixAsync(_checkerShader.ObjectToWorld, false, entity.Transform.Matrix);
                await _context.DrawElementsAsync(Primitive.TRIANGLES, _quad.Indices.Length, DataType.UNSIGNED_SHORT, 0);
            }
        }

        await _cube.Bind(_context, _checkerShader);
        foreach (var entity in _currentScene.Root)
        {
            if (entity.MeshType == MeshType.Cube)
            {
                await _context.UniformMatrixAsync(_checkerShader.ObjectToWorld, false, entity.Transform.Matrix);
                await _context.DrawElementsAsync(Primitive.TRIANGLES, _cube.Indices.Length, DataType.UNSIGNED_SHORT, 0);
            }
        }
    }

    public void Dispose()
    {
        _physicsWorld.Dispose();
    }
}