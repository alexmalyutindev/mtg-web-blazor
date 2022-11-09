using System.Numerics;
using MtgWeb.Core.Utils;

namespace MtgWeb.Core;

public class Transform
{
    public Vector3 Position
    {
        get => _position;
        set
        {
            _hasChanged = true;
            _position = value;
        }
    }

    public Vector3 Rotation
    {
        get => _rotation;
        set
        {
            _hasChanged = true;
            _rotation = value;
        }
    }
    
    public Quaternion Quaternion => Quaternion.CreateFromYawPitchRoll(
        _rotation.Y * Math.DEG_TO_RAD,
        _rotation.X * Math.DEG_TO_RAD,
        _rotation.Z * Math.DEG_TO_RAD
    );

    public Vector3 Scale
    {
        get => _scale;
        set
        {
            _hasChanged = true;
            _scale = value;
        }
    }

    public Vector3 Forward => new Vector3(Matrix[FORWARD_X], Matrix[FORWARD_Y], Matrix[FORWARD_Z]);
    public Vector3 Right => new Vector3(Matrix[RIGHT_X], Matrix[RIGHT_Y], Matrix[RIGHT_Z]);
    public Vector3 Up => new Vector3(Matrix[UP_X], Matrix[UP_Y], Matrix[UP_Z]);

    public bool UpdateViewMatrix;

    private const int TRANSLATION_X = 12, TRANSLATION_Y = 13, TRANSLATION_Z = 14;

    private const int RIGHT_X = 0, RIGHT_Y = 4, RIGHT_Z = 8;
    private const int UP_X = 1, UP_Y = 5, UP_Z = 9;
    private const int FORWARD_X = 2, FORWARD_Y = 6, FORWARD_Z = 10;

    public float[] Matrix = new float[16]
    {
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1,
    };

    public float[] InvMatrix = new float[16]
    {
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1,
    };

    // Updated only if UpdateViewMatrix is set to true.
    public float[] WorldToView = new float[16]
    {
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1,
    };

    private bool _hasChanged = true;

    private Vector3 _position = Vector3.Zero;
    private Vector3 _rotation = Vector3.Zero;
    private Vector3 _scale = Vector3.One;

    public void Update()
    {
        if (!_hasChanged)
            return;

        _hasChanged = false;
        var translation = Matrix4x4.CreateTranslation(_position);
        var scale = Matrix4x4.CreateScale(_scale);
        var quaternion = Quaternion.CreateFromYawPitchRoll(
            _rotation.Y * Math.DEG_TO_RAD,
            _rotation.X * Math.DEG_TO_RAD,
            _rotation.Z * Math.DEG_TO_RAD
        );

        var transform = translation;
        transform = Matrix4x4.Transform(transform, quaternion);

        if (UpdateViewMatrix)
        {
            // WorldToView is Rotation * InvTranslation
            transform.ToArray(in WorldToView);
            WorldToView[TRANSLATION_X] = -WorldToView[TRANSLATION_X];
            WorldToView[TRANSLATION_Y] = -WorldToView[TRANSLATION_Y];
            WorldToView[TRANSLATION_Z] = -WorldToView[TRANSLATION_Z];
        }

        transform = scale * transform;
        transform.ToArray(in Matrix);

        Matrix4x4.Invert(transform, out transform);
        transform.ToArray(in InvMatrix);
    }
}