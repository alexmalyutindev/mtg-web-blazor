using MtgWeb.Core.Render;

namespace MtgWeb.Core;

public class Component
{
    public Entity? Entity;

    public void Init(Entity entity)
    {
        Entity ??= entity;
    }

    public virtual void Start() { }

    public virtual void Update() { }
}