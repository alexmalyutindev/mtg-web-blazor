using System.Numerics;
using Blazor.Extensions;
using Microsoft.JSInterop;

namespace MtgWeb.Core;

public enum ButtonState
{
    None,
    Down,
    Press,
    Up
}

public enum KeyCode
{
    Space = 32,
    Shift = 16,
    W = 87,
    A = 65,
    S = 83,
    D = 68
}

public static class Input
{
    public static Vector2 Axis => new Vector2(_axisX, _axisY);

    public static ButtonState Fire { get; private set; }

    public static Vector2 MouseDelta { get; private set; }
    public static Vector2 MousePosition { get; private set; }
    public static Vector2 PrevMousePosition { get; private set; }

    private static bool _firstUpdate = true;

    private static readonly ButtonState[] _keyMap = new ButtonState[124];
    
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

    public static void LateUpdate()
    {
        for (var i = 0; i < _keyMap.Length; i++)
        {
            switch (_keyMap[i])
            {
                case ButtonState.Down:
                    _keyMap[i] = ButtonState.Press;
                    break;
                case ButtonState.Up:
                    _keyMap[i] = ButtonState.None;
                    break;
            }
        }
    }

    public static ButtonState GetKeyState(KeyCode keyCode)
    {
        return _keyMap[(int) keyCode];
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

    private static void OnKeyDown(int keyCode)
    {
        _keyMap[keyCode] = _keyMap[keyCode] != ButtonState.Press ? ButtonState.Down : _keyMap[keyCode];

        var key = (KeyCode) keyCode;
        switch (key)
        {
            case KeyCode.D:
                _axisX = 1;
                break;
            case KeyCode.A:
                _axisX = -1;
                break;
            case KeyCode.W:
                _axisY = 1;
                break;
            case KeyCode.S:
                _axisY = -1;
                break;
        }
    }

    private static void OnKeyUp(int keyCode)
    {
        _keyMap[keyCode] = ButtonState.Up;

        var key = (KeyCode) keyCode;
        switch (key)
        {
            case KeyCode.D:
                _axisX = _axisX > 0 ? 0 : _axisX;
                break;
            case KeyCode.A:
                _axisX = _axisX < 0 ? 0 : _axisX;
                break;
            case KeyCode.W:
                _axisY = _axisY > 0 ? 0 : _axisY;
                break;
            case KeyCode.S:
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
        public void OnKeyDown(int keyCode) => Input.OnKeyDown(keyCode);

        [JSInvokable(nameof(OnKeyUp))]
        public void OnKeyUp(int key) => Input.OnKeyUp(key);

        public void Unbind(IJSRuntime runtime)
        {
            runtime.InvokeVoidAsync("UnbindInput");
        }
    }
}