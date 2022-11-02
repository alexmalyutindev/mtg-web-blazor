using Blazor.Extensions;
using Blazor.Extensions.Canvas.WebGL;
using Microsoft.AspNetCore.Components;
using MtgWeb.Core;

namespace MtgWeb.Pages;

public partial class MainView : ComponentBase
{
    private BECanvasComponent _canvasReference;
    private WebGLContext _context;
    private Game _game;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _context = await _canvasReference.CreateWebGLAsync();
            _game = new Game(_context);
            await _game.Init();
        }

        await _game.MainLoop();

        StateHasChanged();
    }
}