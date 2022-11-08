using System.Numerics;
using Blazor.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MtgWeb.Core;

public enum ButtonState
{
    None,
    Down,
    Press,
    Up
}

public static class Input
{
    public static Vector2 Axis => new Vector2(_axisX, _axisY);

    public static ButtonState Fire { get; private set; }

    public static Vector2 MouseDelta { get; private set; }
    public static Vector2 MousePosition { get; private set; }
    public static Vector2 PrevMousePosition { get; private set; }

    private static bool _firstUpdate = true;
    
    private static float _axisX;
    private static float _axisY;

    private static Vector2 _rawMousePosition;
    private static bool _leftMouseButton;

    public static void Update()
    {
        if (_firstUpdate)
        {
            MousePosition = _rawMousePosition;
            PrevMousePosition = _rawMousePosition;
            _firstUpdate = false;
        }

        PrevMousePosition = MousePosition;
        MousePosition = _rawMousePosition;
        MouseDelta = MousePosition - PrevMousePosition;

        if (_leftMouseButton)
            Fire = Fire == ButtonState.None ? ButtonState.Down : ButtonState.Press;
        else
            Fire = Fire == ButtonState.Press ? ButtonState.Up : ButtonState.None;
    }


    private enum MouseButton
    {
        LEFT = 0,
        MIDDLE = 1,
        RIGHT = 2,
        FORTH = 3,
        FIFTH = 4
    }

    // private const int LEFT_MOUSE = 0, RIGHT_MOUSE = 2;
    private static void OnMouseDown(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.LEFT:
                _leftMouseButton = true;
                break;
        }
    }

    private static void OnMouseUp(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.LEFT:
                _leftMouseButton = false;
                break;
        }
    }

    private static void OnMouseMove(Vector2 position, Vector2 delta)
    {
        _rawMousePosition += delta;
    }

    private static void OnKeyDown(String key)
    {
        switch (key.ToLower())
        {
            case "d":
                _axisX = 1;
                break;
            case "a":
                _axisX = -1;
                break;
            case "w":
                _axisY = 1;
                break;
            case "s":
                _axisY = -1;
                break;
        }
    }

    private static void OnKeyUp(String key)
    {
        switch (key.ToLower())
        {
            case "d":
                _axisX = _axisX > 0 ? 0 : _axisX;
                break;
            case "a":
                _axisX = _axisX < 0 ? 0 : _axisX;
                break;
            case "w":
                _axisY = _axisY > 0 ? 0 : _axisY;
                break;
            case "s":
                _axisY = _axisY < 0 ? 0 : _axisY;
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

        public async Task Bind(IJSRuntime runtime, BECanvasComponent canvas)
        {
            await runtime.InvokeVoidAsync("BindInput", _reference);
        }

        [JSInvokable(nameof(OnMouseDown))]
        public void OnMouseDown(int button) => Input.OnMouseDown((MouseButton) button);

        [JSInvokable(nameof(OnMouseUp))]
        public void OnMouseUp(int button) => Input.OnMouseUp((MouseButton) button);

        [JSInvokable(nameof(OnMouseMove))]
        public void OnMouseMove(float x, float y, float dX, float dY)
        {
            Input.OnMouseMove(new Vector2(x, y), new Vector2(dX, dY));
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