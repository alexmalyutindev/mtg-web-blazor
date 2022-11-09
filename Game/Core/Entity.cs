using System.Numerics;
using System.Text.Json.Serialization;
using BepuPhysics;
using BepuPhysics.Collidables;
using MtgWeb.Core.Physics;
using Newtonsoft.Json;

namespace MtgWeb.Core;

public class Entity
{
    public String Name = "New Entity";
    public readonly Transform Transform = new(); // TODO: Add WorldSpace matrix for children.

    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public MeshType MeshType;

    public RigidBody RigidBody;
    public StaticBody StaticBody;

    [JsonProperty(TypeNameHandling = TypeNameHandling.Auto, ItemTypeNameHandling = TypeNameHandling.Auto)]
    public Component[] Components = Array.Empty<Component>();

    public Entity Parrent;
    public Entity[] Children = Array.Empty<Entity>();

    public void InitComponents()
    {
        foreach (var entity in Children) entity.InitComponents();
        foreach (var component in Components) component.Init(this);
    }

    public void StartComponents()
    {
        foreach (var entity in Children) entity.StartComponents();
        foreach (var component in Components) component.Start();
    }

    public void BindHierarchy()
    {
        foreach (var entity in Children)
        {
            entity.Parrent = this;
            entity.BindHierarchy();
        }
    }

    public void UpdateComponents()
    {
        foreach (var component in Components)
        {
            component.Update();
        }
    }
}

public class Component
{
    public Entity Entity;

    public void Init(Entity entity)
    {
        Entity = entity;
    }

    public virtual void Start() { }

    public virtual void Update() { }
}

public class PlayerController : Component
{
    // private Camera? _camera;
    private bool _grounded;
    private float _playerHeight = 1.75f;
    private float _velocityY;

    private float _moveSpeed = 2.5f;
    private float _sprintSpeed = 4.5f;

    public override void Start()
    {
        // _camera = Entity.Components.First(component => component is Camera) as Camera;
    }

    public override void Update()
    {
        var axis = Input.Axis;
        if (axis.Y != 0 || axis.X != 0)
        {
            var movement =
                Vector2.Normalize(axis) *
                (IsSprinting() ? _sprintSpeed : _moveSpeed) *
                Time.DeltaTime;

            // TODO: Investigate negation of Forward.
            Entity.Transform.Position -= Entity.Transform.Forward * movement.Y;
            Entity.Transform.Position += Entity.Transform.Right * movement.X;
        }

        Entity.Transform.Rotation += new Vector3(0, Input.MouseDelta.X * Time.DeltaTime * 5f, 0);

        if (!_grounded && Entity.Transform.Position.Y >= _playerHeight)
        {
            Entity.Transform.Position += Vector3.UnitY * _velocityY * Time.DeltaTime;
            _velocityY -= 9.81f * Time.DeltaTime;
        }
        else
        {
            _grounded = true;
            _velocityY = 0;
            Entity.Transform.Position = Entity.Transform.Position with {Y = _playerHeight};
        }

        if (_grounded && Input.GetKeyState(KeyCode.Space) == ButtonState.Down)
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

public enum MeshType
{
    None = -1,
    Cube = 0,
    Quad = 1,
}