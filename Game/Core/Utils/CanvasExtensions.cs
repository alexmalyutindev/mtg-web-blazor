using Blazor.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MtgWeb.Core.Utils;

public static class CanvasExtensions
{
    [Inject] private static IJSRuntime _jsRuntime { get; set; }
    
    public static async Task RequestPointerLock(this BECanvasComponent canvas)
    {
        await _jsRuntime.InvokeVoidAsync("RequestPointerLock", canvas.Id);
    }
}