namespace MtgWeb.Core.Render;

public class Renderer : Component
{
    public MeshType MeshType;
    public Material? Material;

    public Renderer()
    {
        ComponentsBucket<Renderer>.Add(this);
    }

    public override async Task Start()
    {
        // await Material.Init();
        if (Material != null)
            await Material.Load();
    }
}