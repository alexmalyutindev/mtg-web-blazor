using System.Diagnostics;
using System.Numerics;
using Blazor.Extensions.Canvas.WebGL;
using MtgWeb.Core.Render;

namespace MtgWeb.Core;

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
        _currentScene = await Resources.LoadScene("Scene");

        _camera = new Camera()
        {
            Transform =
            {
                Position = new Vector3(0.0f, 1f, 5f)
            }
        };
        await _quad.Init(_context);
        _checkerShader = await Shader.Load(_context, "Checker");
    }

    public async Task MainLoop()
    {
        Time.Tick(0.032f);
        _stopwatch.Restart();

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

    private float _playerHeight = 1f;
    private float _velocityY = 0;
    private bool _grounded = false;

    private async Task Update()
    {
        var axis = Input.Axis;
        if (axis.Y != 0 || axis.X != 0)
        {
            axis = Vector2.Normalize(axis);
            var shift = Input.GetKeyState(KeyCode.Shift);
            if (shift is ButtonState.Press or ButtonState.Down)
                axis *= 2;

            axis *= Time.DeltaTime;
            _camera.Transform.Position -= _camera.Transform.Forward * axis.Y; // TODO: Investigate negation of Forward.
            _camera.Transform.Position += _camera.Transform.Right * axis.X;
        }

        _camera.Transform.Rotation += new Vector3(0, Input.MouseDelta.X * Time.DeltaTime * 5f, 0);

        if (!_grounded && _camera.Transform.Position.Y >= _playerHeight)
        {
            _camera.Transform.Position += Vector3.UnitY * _velocityY * Time.DeltaTime;
            _velocityY -= 9.81f * Time.DeltaTime;
        }
        else
        {
            _grounded = true;
            _velocityY = 0;
            _camera.Transform.Position = _camera.Transform.Position with {Y = _playerHeight};
        }

        Console.WriteLine(Input.GetKeyState(KeyCode.Space));

        if (Input.GetKeyState(KeyCode.Space) == ButtonState.Down)
        {
            _grounded = false;
            _velocityY = 5;
        }
    }

    private async Task Render()
    {
        _camera.Transform.Update();

        foreach (var entity in _currentScene.Root)
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
        foreach (var entity in _currentScene.Root)
        {
            await _context.UniformMatrixAsync(_checkerShader.ObjectToWorld, false, entity.Transform.Matrix);
            await _context.DrawElementsAsync(Primitive.TRIANGLES, _quad.Indices.Length, DataType.UNSIGNED_SHORT, 0);
        }
    }
}