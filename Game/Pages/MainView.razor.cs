using Blazor.Extensions;
using Blazor.Extensions.Canvas.WebGL;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MtgWeb.Core;

namespace MtgWeb.Pages;

public partial class MainView : ComponentBase, IDisposable
{
    public const int Width = 1280;
    public const int Height = 720;

    [Inject] private IJSRuntime _runtime { get; set; }
    [Inject] private HttpClient _HttpClient { get; set; }

    private BECanvasComponent _canvasReference;
    private WebGLContext _context;
    private Game _game;

    private Input.Bridge _bridge;
    private DotNetObjectReference<MainView> _reference;

    [JSInvokable(nameof(Loop))]
    public async Task Loop()
    {
        await _game.MainLoop();
        ;
    }

    protected override async void OnInitialized()
    {
        _reference = DotNetObjectReference.Create(this);

        Resources.Init(_HttpClient);
        _bridge = new Input.Bridge();
        await _bridge.Bind(_runtime, _canvasReference);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _context = await _canvasReference.CreateWebGLAsync();
            _game = new Game(_context);
            await _game.Init();

            // First time request a loop on js side.
            await _runtime.InvokeVoidAsync("RequestAnimationFrame", _reference);
            await _game.MainLoop();
        }
    }

    protected override bool ShouldRender() => true;

    public void Dispose()
    {
        _bridge.Unbind(_runtime);
        _context.Dispose();
        _runtime.InvokeVoidAsync("StopLoop");
    }

    private void RequestMouseLock(MouseEventArgs obj)
    {
        _runtime.InvokeVoidAsync(nameof(RequestMouseLock), _canvasReference.Id);
    }
}