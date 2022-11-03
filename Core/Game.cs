using System.Diagnostics;
using System.Numerics;
using Blazor.Extensions.Canvas.WebGL;

namespace MtgWeb.Core;

public class Game
{
    private readonly WebGLContext _context;
    private Scene? _currentScene;

    private Stopwatch _stopwatch;

    // Temp
    private Transform _camera = new Transform();
    private Mesh _quad = Mesh.Quad();
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
                Transform = {Position = new Vector3(0.1f, 0.1f, 0f)}
            },
            new Entity()
            {
                Transform = {Position = new Vector3(-0.1f, -0.1f, 0f)}
            },
            new Entity(),
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

        await Task.Delay((int) Math.Max(0, 32 - _stopwatch.ElapsedMilliseconds));
    }

    private async Task Update()
    {
        await Task.Delay(7);
    }

    private async Task Render()
    {
        _camera.Update();

        foreach (var entity in _currentScene.root)
        {
            entity.Transform.Update();
        }

        await _context.DisableAsync(EnableCap.CULL_FACE);
        // await _context.EnableAsync(EnableCap.DEPTH_TEST); // TODO: Just for now

        await _context.ClearColorAsync(0, 0, 0, 1);
        await _context.ClearAsync(BufferBits.COLOR_BUFFER_BIT);

        await _context.ViewportAsync(0, 0, 800, 600);
        await _context.UniformMatrixAsync(_checkerShader.WorldToView, false, _camera.Matrix);
        await _context.UniformMatrixAsync(_checkerShader.Projection, false, _camera.Matrix);

        await _checkerShader.Bind(_context);
        await _quad.Bind(_context, _checkerShader);
        foreach (var entity in _currentScene.root)
        {
            await _context.UniformMatrixAsync(_checkerShader.ObjectToWorld, false, entity.Transform.Matrix);
            await _context.DrawElementsAsync(Primitive.TRIANGLES, _quad.Indices.Length, DataType.UNSIGNED_SHORT, 0);
        }
    }
}