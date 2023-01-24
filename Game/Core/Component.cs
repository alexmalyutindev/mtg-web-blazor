using Newtonsoft.Json;

namespace MtgWeb.Core;

public class Component : IDisposable
{
    [JsonIgnore]
    public Entity? Entity;

    public void Init(Entity entity)
    {
        Entity ??= entity;
    }

    public virtual async Task Start() { }

    public virtual void Update() { }

    public void Dispose()
    {
        ComponentsBucket.Remove(this);
    }
}
