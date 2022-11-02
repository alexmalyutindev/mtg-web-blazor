using System.Numerics;
using Blazor.Extensions.Canvas.WebGL;

namespace MtgWeb.Core;

static class Time
{
    public static float CurrentTime;
    public static float DeltaTime;
    public static float LastUpdateTime;
}

public class Transform
{
    public Vector3 Position {
        get => _position;
        set
        {
            _isDirty = true;
            _position = value;
        }
    }
    public Vector3 Rotation {
        get => _rotation;
        set
        {
            _isDirty = true;
            _rotation = value;
        }
    }
    public Vector3 Scale {
        get => _scale;
        set
        {
            _isDirty = true;
            _scale = value;
        }
    }
    
    public float[] Matrix = new float[16];

    private bool _isDirty = true;
    private Vector3 _position = Vector3.Zero;
    private Vector3 _rotation = Vector3.Zero;
    private Vector3 _scale = Vector3.One;
    
    public void Update()
    {
        _isDirty = false;
        Matrix[13] = Position.X;
        Matrix[14] = Position.Y;
        Matrix[15] = Position.Z;
    }
}

public class Entity
{
    public String Name = "New Entity";
    public readonly Transform Transform = new();
}

public class Scene
{
    public Entity[] root;
}

public class Game
{
    private readonly WebGLContext _context;
    private Scene? _currentScene;

    public Game(WebGLContext context)
    {
        _context = context;
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
    }

    public async Task MainLoop()
    {
        UpdateTime();
        Update();
        Render();

        await Task.Delay(32);
    }

    private void UpdateTime()
    {
        Time.LastUpdateTime = Time.CurrentTime;
        Time.CurrentTime += 0.032f;
        Time.DeltaTime = 0.032f;
    }

    private void Update()
    {
        
    }

    private void Render()
    {
        foreach (var entity in _currentScene.root)
        {
            entity.Transform.Update();
        }
        
        _context.ClearColorAsync(MathF.Sin(Time.CurrentTime), 0, 0, 1);
        _context.ClearAsync(BufferBits.COLOR_BUFFER_BIT);
    }
}