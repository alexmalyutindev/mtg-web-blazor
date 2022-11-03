using System.Numerics;
using Microsoft.JSInterop;

namespace MtgWeb.Pages;

public static class Input
{
    public static Vector2 Axis => new Vector2(_axisX, _axisY);

    private static float _axisX;
    private static float _axisY;
    
    private static void OnKeyDown(String key)
    {
        switch (key.ToLower())
        {
            case "d": _axisX = 1;
                break;
            case "a": _axisX = -1;
                break;
            case "w": _axisY = 1;
                break;
            case "s": _axisY = -1;
                break;
        }
    }
    
    private static void OnKeyUp(String key)
    {
        switch (key.ToLower())
        {
            case "d": _axisX = _axisX > 0 ? 0 : _axisX;
                break;
            case "a": _axisX = _axisX < 0 ? 0 : _axisX;
                break;
            case "w": _axisY = _axisY > 0 ? 0 : _axisY;
                break;
            case "s": _axisY = _axisY < 0 ? 0 : _axisY;
                break;
        }
    }
    
    public class InputBridge
    {
        private DotNetObjectReference<InputBridge> _reference;

        public InputBridge()
        {
            _reference = DotNetObjectReference.Create(this);
        }

        public async Task Bind(IJSRuntime runtime)
        {
            await runtime.InvokeVoidAsync("AddInputListener", _reference);
        }
    
        [JSInvokable(nameof(OnKeyDown))]
        public void OnKeyDown(String key) => Input.OnKeyDown(key);

        [JSInvokable(nameof(OnKeyUp))]
        public void OnKeyUp(String key) => Input.OnKeyUp(key);
    }
}