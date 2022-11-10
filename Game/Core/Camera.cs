using System.Numerics;
using MtgWeb.Core.Utils;
using MtgWeb.Pages;

namespace MtgWeb.Core;

public class Camera : Component
{
    
    public readonly float[] WorldToView = new float[16];
    public readonly float[] InvWorldToView = new float[16];
    public readonly float[] ViewProjection = new float[16];
    public readonly float[] InvViewProjection = new float[16];
    public readonly float[] Projection = new float[16];

    public Vector4 ClearColor = new(0.8f, 0.8f, 0.8f, 1);
    
    public float AspectRatio = (float) MainView.Width / MainView.Height;
    public float NearPlane = 0.01f;
    public float FarPlane = 100f;
    public float FieldOfView { get; set; } = 60f;

    private Matrix4x4 _projection;

    public Camera()
    {
        _projection = Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView * Math.DEG_TO_RAD, AspectRatio, NearPlane, FarPlane);
        _projection.ToArray(in Projection);
    }

    public override void Start()
    {
    }

    public override void Update()
    {
        var transform = Entity.Transform;
        if (transform.WasChanged)
        {
            var worldToView = Matrix4x4.CreateTranslation(-transform.Position);
            var rotation = transform.Quaternion;
            worldToView = Matrix4x4.Transform(worldToView, rotation);
            
            worldToView.ToArray(in WorldToView);
            Matrix4x4.Invert(worldToView, out var invWorldToView);
            invWorldToView.ToArray(in InvWorldToView);

            var viewProjection = (worldToView * _projection);
            viewProjection.ToArray(ViewProjection);
            Matrix4x4.Invert(worldToView, out var invViewProjection);
            invViewProjection.ToArray(InvViewProjection);
        }
    }
}