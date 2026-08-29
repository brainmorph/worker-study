using Godot;

namespace WorkerStudy.World;

// Drag-to-pan (middle mouse) + scroll-to-zoom. Rotation is intentionally
// never touched here, so the camera stays axis-aligned.
public partial class CameraController : Camera2D
{
    [Export] public float MinZoom = 0.3f;
    [Export] public float MaxZoom = 3f;
    [Export] public float ZoomStep = 0.1f;

    private bool _isPanning;

    public override void _UnhandledInput(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton { ButtonIndex: MouseButton.Middle } middleClick:
                _isPanning = middleClick.Pressed;
                break;

            case InputEventMouseButton { ButtonIndex: MouseButton.WheelUp, Pressed: true }:
                ApplyZoom(ZoomStep);
                break;

            case InputEventMouseButton { ButtonIndex: MouseButton.WheelDown, Pressed: true }:
                ApplyZoom(-ZoomStep);
                break;

            case InputEventMouseMotion motion when _isPanning:
                Position -= motion.Relative / Zoom;
                break;
        }
    }

    private void ApplyZoom(float delta)
    {
        float newZoom = Mathf.Clamp(Zoom.X + delta, MinZoom, MaxZoom);
        Zoom = new Vector2(newZoom, newZoom);
    }
}
