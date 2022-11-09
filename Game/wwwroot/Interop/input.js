window.BindInput = function (bridge) {
    console.log("Bind Input...")
    let inputListener = {
        mousedown : (e) => bridge.invokeMethodAsync("OnMouseDown", e.button),
        mouseup : (e) => bridge.invokeMethodAsync("OnMouseUp", e.button),
        mousemove : (e) => bridge.invokeMethodAsync("OnMouseMove", e.x, e.y, e.movementX, e.movementY),
        keydown : (e) => bridge.invokeMethodAsync("OnKeyDown", e.keyCode),
        keyup : (e) => bridge.invokeMethodAsync("OnKeyUp", e.keyCode)
    }

    window.addEventListener("mousedown", inputListener.mousedown);
    window.addEventListener("mouseup", inputListener.mouseup);
    window.addEventListener("mousemove", inputListener.mousemove);
    window.addEventListener("keydown", inputListener.keydown);
    window.addEventListener("keyup", inputListener.keyup);

    window.UnbindInput = () => {
        window.removeEventListener("mousedown", inputListener.mousedown);
        window.removeEventListener("mouseup", inputListener.mouseup);
        window.removeEventListener("mousemove", inputListener.mousemove);
        window.removeEventListener("keydown", inputListener.keydown);
        window.removeEventListener("keyup", inputListener.keyup);
    };
}

window.RequestMouseLock = function (canvasId)
{
    if (document.pointerLockElement === null || document.pointerLockElement.id !== canvasId)
        document.getElementById(canvasId).requestPointerLock();
}