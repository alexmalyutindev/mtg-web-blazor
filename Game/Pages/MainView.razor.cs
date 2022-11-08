using Blazor.Extensions;
using Blazor.Extensions.Canvas.WebGL;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MtgWeb.Core;

namespace MtgWeb.Pages;

public partial class MainView : ComponentBase, IDisposable
{
    [Inject] private IJSRuntime _runtime { get; set; }
    [Inject] private HttpClient _HttpClient { get; set; }

    private BECanvasComponent _canvasReference;
    private WebGLContext _context;
    private Game _game;

    private Input.Bridge _bridge;

    protected override async void OnInitialized()
    {
        Resources.Init(_HttpClient);
        _bridge = new Input.Bridge();
        await _bridge.Bind(_runtime, _canvasReference);
    }

    private void RequestMouseLock(MouseEventArgs obj)
    {
        _runtime.InvokeVoidAsync(nameof(RequestMouseLock), _canvasReference.Id);
    }

    private bool _shouldRender = true;
    private Task mainLoop = Task.CompletedTask;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _context = await _canvasReference.CreateWebGLAsync();
            _game = new Game(_context);
            await _game.Init();
        }

        if (!mainLoop.IsCompleted) // Called from another event!
        {
            Console.WriteLine(new System.Diagnostics.StackTrace());
            return;
        }

        mainLoop = _game.MainLoop();
        await mainLoop;

        StateHasChanged();
    }

    protected override bool ShouldRender()
    {
        return mainLoop.IsCompleted;
    }

    public void Dispose()
    {
        _bridge.Unbind(_runtime);
        _context.Dispose();
    }
}