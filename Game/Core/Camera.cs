using System.Numerics;
using MtgWeb.Core.Utils;
using MtgWeb.Pages;

namespace MtgWeb.Core;

public class Camera : Component
{
    public readonly Transform Transform = new()
    {
        UpdateViewMatrix = true
    };
    public float[] WorldToView => Transform.WorldToView;
    public readonly float[] Projection = new float[16];

    public Vector4 ClearColor = new(0.8f, 0.8f, 0.8f, 1);
    
    public float AspectRatio = (float) MainView.Width / MainView.Height;
    public float NearPlane = 0.01f;
    public float FarPlane = 100f;
    public float FieldOfView { get; set; } = 60f;

    public Camera()
    {
        Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView * Math.DEG_TO_RAD, AspectRatio, NearPlane, FarPlane)
            .ToArray(in Projection);
    }

    public override void Start()
    {
        Entity.Transform.UpdateViewMatrix = true;
    }
}