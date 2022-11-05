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

    private const float DEG_TO_RAD = 0.017453292519943295769236907684886f;
    private const int TRANSLATION_X = 12, TRANSLATION_Y = 13, TRANSLATION_Z = 14;
    
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
    
    public float[] WorldToView = new float[16]
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
        var translation = Matrix4x4.CreateTranslation(_position);
        var scale = Matrix4x4.CreateScale(_scale);
        var quaternion = Quaternion.CreateFromYawPitchRoll(
            _rotation.Y * DEG_TO_RAD,
            _rotation.X * DEG_TO_RAD,
            _rotation.Z * DEG_TO_RAD
        );

        var result = translation;
        result = Matrix4x4.Transform(result, quaternion);
        result = scale * result;

        result.ToArray(in Matrix);
        
        // BUG: Fix it!
        // Why need to invert translation?
        result.ToArray(in WorldToView);
        WorldToView[TRANSLATION_X] *= -1;
        WorldToView[TRANSLATION_Y] *= -1;
        WorldToView[TRANSLATION_Z] *= -1;
        
        Matrix4x4.Invert(result, out result);
        result.ToArray(in InvMatrix);
    }
}