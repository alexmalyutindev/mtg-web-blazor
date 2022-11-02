using Blazor.Extensions;
using Blazor.Extensions.Canvas.WebGL;
using Microsoft.AspNetCore.Components;

namespace MtgWeb.Pages;
public partial class MainView : ComponentBase
{
    BECanvasComponent _canvasReference;
    WebGLContext _context;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _context = await _canvasReference.CreateWebGLAsync();
    
        await _context.ClearColorAsync(0, 0, 0, 1);
        await _context.ClearAsync(BufferBits.COLOR_BUFFER_BIT);
        
        Console.WriteLine("OnAfterRenderAsync");
        
        // TODO: Init Game context and setup main loop.
    }
}