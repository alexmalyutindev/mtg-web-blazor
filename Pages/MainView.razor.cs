using Blazor.Extensions;
using Blazor.Extensions.Canvas.WebGL;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MtgWeb.Core;

namespace MtgWeb.Pages;

public partial class MainView : ComponentBase
{
    [Inject] private IJSRuntime _runtime { get; set; }
    
    private BECanvasComponent _canvasReference;
    private WebGLContext _context;
    private Game _game;

    private Input.InputBridge _inputBridge;

    protected override async void OnInitialized()
    {
        _inputBridge = new Input.InputBridge();
        await _inputBridge.Bind(_runtime);
    }

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