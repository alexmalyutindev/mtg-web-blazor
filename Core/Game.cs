using System.Diagnostics;
using System.Numerics;
using Blazor.Extensions.Canvas.WebGL;
using MtgWeb.Core.Render;
using MtgWeb.Core.Utils;
using MtgWeb.Pages;

namespace MtgWeb.Core;

public class Camera
{
    public readonly Transform Transform = new();
    public float[] WorldToView => Transform.WorldToView;
    public readonly float[] Projection = new float[16];

    public Vector4 ClearColor = new(0.8f, 0.8f, 0.8f, 1);
    
    public float AspectRatio = 800f / 600f;
    public float NearPlane = 0.01f;
    public float FarPlane = 100f;

    public Camera()
    {
        Matrix4x4.CreatePerspectiveFieldOfView(1.5f, AspectRatio, NearPlane, FarPlane)
            .ToArray(in Projection);
    }
}

public class Game
{
    private readonly WebGLContext _context;
    private Scene? _currentScene;

    private Stopwatch _stopwatch;

    // Temp
    private Camera? _camera;
    private readonly Mesh _quad = Mesh.Quad();
    private Shader? _checkerShader;

    public Game(WebGLContext context)
    {
        _context = context;
        _stopwatch = new Stopwatch();
    }

    public async Task Init()
    {
        _currentScene = new Scene();
        _currentScene.root = new[]
        {
            new Entity()
            {
                Name = "Test Quad",
                Transform = {Position = new Vector3(0.2f, 0.5f, 1f)}
            },
            new Entity()
            {
                Name = "Floor",
                Transform =
                {
                    Rotation = new Vector3(90, 0, 0),
                    Scale = new Vector3(5f, 5f, 5f)
                }
            },
            new Entity()
            {
                Transform = {Position = new Vector3(0.0f, 0.5f, 0.0f)}
            }
        };

        _camera = new Camera()
        {
            Transform =
            {
                Position = new Vector3(0.0f, 1f, 5f)
            }
        };
        await _quad.Init(_context);
        _checkerShader = await Shader.CheckerShader(_context);
    }

    public async Task MainLoop()
    {
        Time.Tick(0.032f);
        _stopwatch.Restart();

        await Update();
        await Render();

        _stopwatch.Stop();

        if (_stopwatch.ElapsedMilliseconds < 32)
            await Task.Delay(32 - (int) _stopwatch.ElapsedMilliseconds);
    }

    private async Task Update()
    {
        var axis = Input.Axis * Time.DeltaTime;
        _camera.Transform.Position += new Vector3(axis.X, 0, -axis.Y);
        _camera.Transform.Rotation += new Vector3(0, Input.MouseDelta.X, 0);
    }

    private async Task Render()
    {
        _camera.Transform.Update();

        foreach (var entity in _currentScene.root)
        {
            entity.Transform.Update();
        }

        await _context.DisableAsync(EnableCap.CULL_FACE);
        await _context.EnableAsync(EnableCap.DEPTH_TEST);

        var clearColor = _camera.ClearColor;
        await _context.ClearColorAsync(clearColor.X, clearColor.Y, clearColor.Z, clearColor.W);
        await _context.ClearAsync(BufferBits.COLOR_BUFFER_BIT);

        await _context.ViewportAsync(0, 0, 800, 600);

        await _checkerShader.Bind(_context);

        await _context.UniformAsync(_checkerShader.Time, Time.CurrentTime);
        await _context.UniformMatrixAsync(_checkerShader.WorldToView, false, _camera.WorldToView);
        await _context.UniformMatrixAsync(_checkerShader.Projection, false, _camera.Projection);

        await _quad.Bind(_context, _checkerShader);
        foreach (var entity in _currentScene.root)
        {
            await _context.UniformMatrixAsync(_checkerShader.ObjectToWorld, false, entity.Transform.Matrix);
            await _context.DrawElementsAsync(Primitive.TRIANGLES, _quad.Indices.Length, DataType.UNSIGNED_SHORT, 0);
        }
    }
}