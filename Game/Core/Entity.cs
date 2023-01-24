using MtgWeb.Core.Physics;
using Newtonsoft.Json;

namespace MtgWeb.Core;

public class Entity : IDisposable
{
    public bool Enabled { get; set; } = true;

    public String Name = "New Entity";
    public readonly Transform Transform = new(); // TODO: Add WorldSpace matrix for children.

    public RigidBody RigidBody;
    public StaticBody StaticBody;


    [JsonProperty(TypeNameHandling = TypeNameHandling.Auto, ItemTypeNameHandling = TypeNameHandling.Auto)]
    public Component[] Components = Array.Empty<Component>();

    [JsonIgnore] public Entity? Parent;

    public Entity[] Children = Array.Empty<Entity>();

    public void BindHierarchy()
    {
        foreach (var entity in Children)
        {
            entity.Parent = this;
            entity.BindHierarchy();
        }
    }

    public void InitComponents()
    {
        foreach (var entity in Children) entity.InitComponents();
        foreach (var component in Components) component.Init(this);
    }

    public bool TryGetComponent<T>(out T? component) where T : Component?
    {
        // TODO: Caching
        var components = Components.OfType<T>().ToArray();

        if (components.Length == 0)
        {
            component = null;
            return false;
        }

        component = components.First();
        return true;
    }

    public bool TryGetComponents<T>(out T[]? components) where T : Component
    {
        // TODO: Caching
        var comps = Components.OfType<T>();

        if (!comps.Any())
        {
            components = null;
            return false;
        }

        components = comps.ToArray();
        return true;
    }

    public async Task StartComponents()
    {
        foreach (var entity in Children) await entity.StartComponents();
        foreach (var component in Components) await component.Start();
    }

    public void UpdateComponents()
    {
        foreach (var child in Children)
        {
            child?.UpdateComponents();
        }

        foreach (var component in Components)
        {
            component.Update();
        }
    }

    public void Dispose()
    {
        foreach (var component in Components) component.Dispose();
        Array.Clear(Components);
        foreach (var child in Children) child.Dispose();
        Array.Clear(Children);
    }
}

public class PrefabEntity : Entity
{
    public string PrefabName;
}

public enum MeshType
{
    None = -1,
    Cube = 0,
    Quad = 1,
}