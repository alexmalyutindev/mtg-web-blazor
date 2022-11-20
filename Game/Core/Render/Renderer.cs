namespace MtgWeb.Core.Render;

public class Renderer : Component
{
    public MeshType MeshType;
    public Material? Material;
    
    public override async Task Start()
    {
        // await Material.Init();
        if (Material != null)
            await Material.Load();
    }
}