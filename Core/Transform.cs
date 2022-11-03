using System.Numerics;

namespace MtgWeb.Core;

public class Transform
{
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

    private const int TRANSLATION_X = 12, TRANSLATION_Y = 13, TRANSLATION_Z = 14;
    private const int SCALE_X = 0, SCALE_Y = 5, SCALE_Z = 10;

    public Matrix4x4 Matrix0 = Matrix4x4.Identity;
    
    public float[] Matrix = new float[16]
    {
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1,
    };

    private bool _isDirty = true;
    private Vector3 _position = Vector3.Zero;
    private Vector3 _rotation = Vector3.Zero;
    private Vector3 _scale = Vector3.One;

    public void Update()
    {
        if (!_isDirty)
            return;

        _isDirty = false;
        Matrix[TRANSLATION_X] = _position.X;
        Matrix[TRANSLATION_Y] = _position.Y;
        Matrix[TRANSLATION_Z] = _position.Z;

        Matrix[SCALE_X] *= _scale.X;
        Matrix[SCALE_Y] *= _scale.Y;
        Matrix[SCALE_Z] *= _scale.Z;
    }
}