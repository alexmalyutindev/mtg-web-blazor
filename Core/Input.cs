using System.Numerics;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MtgWeb.Core;

public static class Input
{
    public static Vector2 Axis => new Vector2(_axisX, _axisY);
    
    public static Vector2 MouseDelta { get; private set;  }
    public static Vector2 MousePosition { get; private set;  }

    private static float _axisX;
    private static float _axisY;

    private static void OnMouseMove(Vector2 position)
    {
        MouseDelta = position - MousePosition;
        MousePosition = position;
    }
    
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
    
    public class Bridge
    {
        private DotNetObjectReference<Bridge> _reference;

        public Bridge()
        {
            _reference = DotNetObjectReference.Create(this);
        }

        public async Task Bind(IJSRuntime runtime)
        {
            await runtime.InvokeVoidAsync("BindInput", _reference);
        }
    
        [JSInvokable(nameof(OnMouseMove))]
        public void OnMouseMove(float x, float y)
        {
            Console.WriteLine(new Vector2(x, y));
            Input.OnMouseMove(new Vector2(x, y));
        }

        [JSInvokable(nameof(OnKeyDown))]
        public void OnKeyDown(String key) => Input.OnKeyDown(key);

        [JSInvokable(nameof(OnKeyUp))]
        public void OnKeyUp(String key) => Input.OnKeyUp(key);

        public void Unbind(IJSRuntime runtime)
        {
            runtime.InvokeVoidAsync("UnbindInput");
        }
    }
}