using System.Text.Json.Serialization;
using MtgWeb.Core.Physics;
using Newtonsoft.Json;

namespace MtgWeb.Core;

public class Entity
{
    public bool Enabled { get; set; } = true;
    
    public String Name = "New Entity";
    public readonly Transform Transform = new(); // TODO: Add WorldSpace matrix for children.
    
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public MeshType MeshType;

    public RigidBody RigidBody;
    public StaticBody StaticBody;


    [JsonProperty(TypeNameHandling = TypeNameHandling.Auto, ItemTypeNameHandling = TypeNameHandling.Auto)]
    public Component[] Components = Array.Empty<Component>();

    [Newtonsoft.Json.JsonIgnore]
    public Entity Parrent;

    public Entity[] Children = Array.Empty<Entity>();

    public void InitComponents()
    {
        foreach (var entity in Children) entity.InitComponents();
        foreach (var component in Components) component.Init(this);
    }

    public bool TryGetComponent<T>(out T component) where T : Component
    {
        // TODO: Caching
        var components = Components.OfType<T>();

        if (!components.Any())
        {
            component = null;
            return false;
        }
    
        component = components.First();
        return true;
    }
    
    public bool TryGetComponents<T>(out T[] component) where T : Component
    {
        // TODO: Caching
        var components = Components.OfType<T>();

        if (!components.Any())
        {
            component = null;
            return false;
        }
    
        component = components.ToArray();
        return true;
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
        foreach (var child in Children)
        {
            child.UpdateComponents();
        }

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