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
    public readonly float[] Projection = new float[16];

    public Camera()
    {
        Matrix4x4.CreatePerspectiveFieldOfView(1.5f, 800f / 600f, 0.01f, 100f)
            .ToArray(ref Projection);
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
                Transform =
                {
                    Position = new Vector3(-0.5f, 0.5f, 2f),
                    Scale = new Vector3(1f, 1.5f, 1f)
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
                Position = new Vector3(0.0f, -1f, -5)
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
        _camera.Transform.Position += new Vector3(-axis.X, 0, axis.Y);
    }

    private async Task Render()
    {
        _camera.Transform.Update();

        foreach (var entity in _currentScene.root)
        {
            entity.Transform.Update();
        }

        await _context.DisableAsync(EnableCap.CULL_FACE);
        await _context.EnableAsync(EnableCap.DEPTH_TEST); // TODO: Just for now

        await _context.ClearColorAsync(0, 0, 0, 1);
        await _context.ClearAsync(BufferBits.COLOR_BUFFER_BIT);

        await _context.ViewportAsync(0, 0, 800, 600);

        await _checkerShader.Bind(_context);

        await _context.UniformAsync(_checkerShader.Time, Time.CurrentTime);
        await _context.UniformMatrixAsync(_checkerShader.WorldToView, false, _camera.Transform.Matrix);
        await _context.UniformMatrixAsync(_checkerShader.Projection, false, _camera.Projection);

        await _quad.Bind(_context, _checkerShader);
        foreach (var entity in _currentScene.root)
        {
            await _context.UniformMatrixAsync(_checkerShader.ObjectToWorld, false, entity.Transform.Matrix);
            await _context.DrawElementsAsync(Primitive.TRIANGLES, _quad.Indices.Length, DataType.UNSIGNED_SHORT, 0);
        }
    }
}