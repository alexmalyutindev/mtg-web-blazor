using System.Numerics;
using MtgWeb.Core.Utils;
using Newtonsoft.Json;

namespace MtgWeb.Core;

public class Transform
{
    [JsonIgnore] public bool WasChanged { get; private set; } = false;

    public Vector3 Position
    {
        get => _position;
        set
        {
            _isDirty = true;
            _position = value;
        }
    }

    public Vector3 Rotation
    {
        get => _rotation;
        set
        {
            _isDirty = true;
            _rotation = value;
            _quaternion = Quaternion.CreateFromYawPitchRoll(
                _rotation.Y * Math.DEG_TO_RAD,
                _rotation.X * Math.DEG_TO_RAD,
                _rotation.Z * Math.DEG_TO_RAD
            );
        }
    }

    public Quaternion Quaternion
    {
        get => _quaternion;
        set
        {
            _isDirty = true;
            _quaternion = value;
            _rotation = _quaternion.ToEuler();
        }
    }

    public Vector3 Scale
    {
        get => _scale;
        set
        {
            _isDirty = true;
            _scale = value;
        }
    }

    [JsonIgnore] public Vector3 Forward => new Vector3(Matrix[FORWARD_X], Matrix[FORWARD_Y], Matrix[FORWARD_Z]);
    [JsonIgnore] public Vector3 Right => new Vector3(Matrix[RIGHT_X], Matrix[RIGHT_Y], Matrix[RIGHT_Z]);
    [JsonIgnore] public Vector3 Up => new Vector3(Matrix[UP_X], Matrix[UP_Y], Matrix[UP_Z]);

    private const int TRANSLATION_X = 12, TRANSLATION_Y = 13, TRANSLATION_Z = 14;

    private const int RIGHT_X = 0, RIGHT_Y = 4, RIGHT_Z = 8;
    private const int UP_X = 1, UP_Y = 5, UP_Z = 9;
    private const int FORWARD_X = 2, FORWARD_Y = 6, FORWARD_Z = 10;

    [JsonIgnore] public float[] Matrix = new float[16]
    {
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1,
    };

    [JsonIgnore] public float[] InvMatrix = new float[16]
    {
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1,
    };

    private bool _isDirty = true;

    private Vector3 _position = Vector3.Zero;
    private Vector3 _rotation = Vector3.Zero;
    private Quaternion _quaternion = Quaternion.Identity;
    private Vector3 _scale = Vector3.One;

    public void Update()
    {
        if (WasChanged)
            WasChanged = false;

        if (!_isDirty)
            return;

        _isDirty = false;
        var translation = Matrix4x4.CreateTranslation(_position);
        var scale = Matrix4x4.CreateScale(_scale);
        var rotation = Matrix4x4.CreateFromQuaternion(_quaternion);

        var transform = scale * rotation * translation;
        transform.ToArray(in Matrix);

        Matrix4x4.Invert(transform, out transform);
        transform.ToArray(in InvMatrix);

        WasChanged = true;
    }
}