using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using MtgWeb.Core.Physics;
using Newtonsoft.Json;

namespace MtgWeb.Core;

public class Entity
{
    public String Name = "New Entity";
    public readonly Transform Transform = new();

    public RigidBody RigidBody;
    public StaticBody StaticBody;
}

public class RigidBody
{
    public bool IsStatic = false;

    public RigidPose Pose = RigidPose.Identity;
    public BodyVelocity Velocity;
    public Collidable Collidable;

    public BodyDescription Body;
    public BodyHandle BodyHandle;
    public IShape[] Shapes;
    public IShape Shape;
}

public class StaticBody
{
    public Vector3 Offset;
    public StaticDescription Description;
    [JsonConverter(typeof(ShapeConverter))]

    public IShape Shape;
    public TypedIndex ShapeId;
}
