using System.Diagnostics;
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
            new Entity() {Name = "Test Quad"},
            new Entity(),
            new Entity(),
        };

        await _quad.Init(_context);
        // TODO: Shader
    }

    public async Task MainLoop()
    {
        Time.Tick(0.032f);
        _stopwatch.Restart();

        await Update();
        Render();

        _stopwatch.Stop();

        await Task.Delay((int) Math.Max(0, 32 - _stopwatch.ElapsedMilliseconds));
    }

    private async Task Update()
    {
        await Task.Delay(7);
    }

    private void Render()
    {
        _camera.Update();

        foreach (var entity in _currentScene.root)
        {
            entity.Transform.Update();
        }

        _context.ClearColorAsync(MathF.Cos(Time.CurrentTime * MathF.PI * 2) * 0.5f + 0.5f, 0, 0, 1);
        _context.ClearAsync(BufferBits.COLOR_BUFFER_BIT);
    }
}