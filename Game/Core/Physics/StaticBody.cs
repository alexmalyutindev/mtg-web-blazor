using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using Newtonsoft.Json;

namespace MtgWeb.Core.Physics;

public class StaticBody
{
    public Vector3 Offset;
    public StaticDescription Description;
    [JsonConverter(typeof(ShapeConverter))]

    public IShape Shape;
    public TypedIndex ShapeId;
}