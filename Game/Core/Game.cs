using System.Diagnostics;
using System.Numerics;
using Blazor.Extensions.Canvas.WebGL;
using MtgWeb.Core.Physics;
using MtgWeb.Core.Render;
using Newtonsoft.Json;

namespace MtgWeb.Core;

public class Game
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
        // _physicsWorld = new PhysicsWorld();

        _stopwatch = new Stopwatch();
    }

    public async Task Init()
    {
        await LoadScene("Scene");

        _camera = new Camera()
        {
            Transform =
            {
                Position = new Vector3(0.0f, 1f, 5f)
            }
        };
        _player = new PlayerController(_camera);

        await _quad.Init(_context);
        await _cube.Init(_context);
        _checkerShader = await Shader.Load(_context, "Checker");
    }

    private async Task LoadScene(string name)
    {
        _currentScene = await Resources.LoadScene(name);
        // _physicsWorld.Add(_currentScene);
    }

    public async Task MainLoop()
    {
        Time.Tick(0.032f);
        _stopwatch.Restart();

        // _physicsWorld.Simulation.Timestep(0.032f);

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
        _player.Update();
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
}

public class PlayerController
{
    private readonly Camera _camera;
    private bool _grounded;
    private float _playerHeight = 1.75f;
    private float _velocityY;

    private float _moveSpeed = 1.5f;
    private float _sprintSpeed = 3.5f;

    public PlayerController(Camera camera)
    {
        _camera = camera;
    }

    public void Update()
    {
        var axis = Input.Axis;
        if (axis.Y != 0 || axis.X != 0)
        {
            var movement =
                Vector2.Normalize(axis) *
                (IsSprinting() ? _sprintSpeed : _moveSpeed) *
                Time.DeltaTime;

            // TODO: Investigate negation of Forward.
            _camera.Transform.Position -= _camera.Transform.Forward * movement.Y; 
            _camera.Transform.Position += _camera.Transform.Right * movement.X;
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

        if (Input.GetKeyState(KeyCode.Space) == ButtonState.Down)
        {
            _grounded = false;
            _velocityY = 5;
        }
    }

    private static bool IsSprinting()
    {
        var shift = Input.GetKeyState(KeyCode.Shift);
        var isSprinting = shift is ButtonState.Press or ButtonState.Down;
        return isSprinting;
    }
}