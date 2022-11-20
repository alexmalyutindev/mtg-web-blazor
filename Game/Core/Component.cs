using Newtonsoft.Json;

namespace MtgWeb.Core;

public class Component
{
    [JsonIgnore]
    public Entity? Entity;

    public void Init(Entity entity)
    {
        Entity ??= entity;
    }

    public virtual async Task Start() { }

    public virtual void Update() { }
}