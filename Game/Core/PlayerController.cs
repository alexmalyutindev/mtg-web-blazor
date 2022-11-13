using System.Numerics;
using MtgWeb.Core.Render;
using Newtonsoft.Json;

namespace MtgWeb.Core;

public class PlayerController : Component
{
    // private Camera? _camera;
    private bool _grounded;
    private float _playerHeight = 1.75f;
    private float _velocityY;

    private float _moveSpeed = 2.5f;
    private float _sprintSpeed = 4.5f;

    public int _currentBulletPoolIndex = 0;

    public override void Start()
    {
        Array.Resize(ref Entity.Children, 5);
        for (int i = 0; i < Entity.Children.Length; i++)
        {
            Entity.Children[i] = new Entity()
            {
                Enabled = false,
                Name = $"Bullet {i}",
                Parrent = Entity.Parrent,
                Transform =
                {
                    Scale = new Vector3(-0.5f)
                },
                Components = new Component[]
                {
                    new BulletController()
                    {
                        Velocity = Entity.Transform.Forward,
                        LifeTime = 3f
                    },
                    new Renderer()
                    {
                        Shader = Shader.Create("Volume"),
                        MeshType = MeshType.Cube
                    }
                }
            };
            Entity.Children[i].InitComponents();
            Entity.Children[i].StartComponents();
        }
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
            var forward = Vector3.Normalize(Entity.Transform.Forward with {Y = 0});
            Entity.Transform.Position -= forward * movement.Y;
            Entity.Transform.Position += Entity.Transform.Right * movement.X;
        }

        if (Input.MouseDelta.X != 0 || Input.MouseDelta.Y != 0)
        {
            float lookSpeed = Math.DEG_TO_RAD * Time.DeltaTime * 5f;
            Entity.Transform.Quaternion *= Quaternion.CreateFromAxisAngle(
                Entity.Transform.Right,
                Input.MouseDelta.Y * lookSpeed
            );
            Entity.Transform.Quaternion *=
                Quaternion.CreateFromAxisAngle(Vector3.UnitY, Input.MouseDelta.X * lookSpeed);
            Entity.Transform.Quaternion = Quaternion.Normalize(Entity.Transform.Quaternion);
        }

        if (!_grounded)
        {
            if (Entity.Transform.Position.Y >= _playerHeight)
            {
                Entity.Transform.Position += Vector3.UnitY * _velocityY * Time.DeltaTime;
                _velocityY -= 9.81f * Time.DeltaTime;
            }

            if (Entity.Transform.Position.Y < _playerHeight)
            {
                _grounded = true;
                _velocityY = 0;
                Entity.Transform.Position = Entity.Transform.Position with {Y = _playerHeight};
            }
        }

        if (_grounded && Input.GetKeyState(KeyCode.Space) == ButtonState.Down)
        {
            _grounded = false;
            _velocityY = 5f;
        }

        if (Input.Fire == ButtonState.Down)
        {
            InstantiateBullet();
        }
    }

    private void InstantiateBullet()
    {
        var bulletEntity = Entity.Children[_currentBulletPoolIndex];
        _currentBulletPoolIndex = (_currentBulletPoolIndex + 1) % Entity.Children.Length;

        bulletEntity.TryGetComponent(out BulletController bullet);
        bulletEntity.Transform.Position = Entity.Transform.Position - Entity.Transform.Forward;
        bullet.Velocity = Entity.Transform.Forward;
        bullet.Reset();

        Console.WriteLine(JsonConvert.SerializeObject(bullet));
    }

    private static bool IsSprinting()
    {
        var shift = Input.GetKeyState(KeyCode.Shift);
        var isSprinting = shift is ButtonState.Press or ButtonState.Down;
        return isSprinting;
    }
}

public class BulletController : Component
{
    public float LifeTime = 5.0f;
    private float _lifeTime = 0.0f;
    public Vector3 Velocity;

    public override void Update()
    {
        if (_lifeTime > LifeTime)
        {
            Entity.Enabled = false;
        }

        Entity.Transform.Position -= Velocity * Time.DeltaTime * 10.0f;

        Velocity += Vector3.UnitY * 5f * Time.DeltaTime;
        if (Entity.Transform.Position.Y < 0.5f)
        {
            Velocity.Y = -Velocity.Y;
        }

        _lifeTime += Time.DeltaTime;
    }

    public void Reset()
    {
        _lifeTime = 0;
        Entity.Enabled = true;
    }
}