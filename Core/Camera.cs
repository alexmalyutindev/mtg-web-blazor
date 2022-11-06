using System.Numerics;
using MtgWeb.Core.Utils;

namespace MtgWeb.Core;

public class Camera
{
    public readonly Transform Transform = new()
    {
        UpdateViewMatrix = true
    };
    public float[] WorldToView => Transform.WorldToView;
    public readonly float[] Projection = new float[16];

    public Vector4 ClearColor = new(0.8f, 0.8f, 0.8f, 1);
    
    public float AspectRatio = 800f / 600f;
    public float NearPlane = 0.01f;
    public float FarPlane = 100f;

    public Camera()
    {
        Matrix4x4.CreatePerspectiveFieldOfView(1.5f, AspectRatio, NearPlane, FarPlane)
            .ToArray(in Projection);
    }
}