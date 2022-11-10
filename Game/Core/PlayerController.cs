using System.Numerics;
using BepuUtilities;

namespace MtgWeb.Core;

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
    }

    private static bool IsSprinting()
    {
        var shift = Input.GetKeyState(KeyCode.Shift);
        var isSprinting = shift is ButtonState.Press or ButtonState.Down;
        return isSprinting;
    }
}