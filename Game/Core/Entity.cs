using System.Text.Json.Serialization;
using MtgWeb.Core.Physics;

namespace MtgWeb.Core;

public class Entity
{
    public String Name = "New Entity";
    public readonly Transform Transform = new();

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MeshType MeshType;

    public RigidBody RigidBody;
    public StaticBody StaticBody;
}

public enum MeshType
{
    Cube = 0,
    Quad = 1,
}