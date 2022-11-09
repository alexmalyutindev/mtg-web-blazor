using System.Text.Json.Serialization;
using BepuPhysics;
using BepuPhysics.Collidables;
using MtgWeb.Core.Physics;
using Newtonsoft.Json;

namespace MtgWeb.Core;

public class Entity
{
    public String Name = "New Entity";
    public readonly Transform Transform = new(); // TODO: Add WorldSpace matrix for children.

    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public MeshType MeshType;

    public RigidBody RigidBody;
    public StaticBody StaticBody;

    [JsonProperty(TypeNameHandling = TypeNameHandling.Auto, ItemTypeNameHandling = TypeNameHandling.Auto)]
    public Component[] Components = Array.Empty<Component>();

    public Entity Parrent;
    public Entity[] Children = Array.Empty<Entity>();

    public void InitComponents()
    {
        foreach (var entity in Children) entity.InitComponents();
        foreach (var component in Components) component.Init(this);
    }

    public void StartComponents()
    {
        foreach (var entity in Children) entity.StartComponents();
        foreach (var component in Components) component.Start();
    }

    public void BindHierarchy()
    {
        foreach (var entity in Children)
        {
            entity.Parrent = this;
            entity.BindHierarchy();
        }
    }

    public void UpdateComponents()
    {
        foreach (var component in Components)
        {
            component.Update();
        }
    }
}

public enum MeshType
{
    None = -1,
    Cube = 0,
    Quad = 1,
}