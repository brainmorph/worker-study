using Godot;

namespace WorkerStudy.World;

public partial class HomeBase : Node2D
{
    [Export] public float Radius = 40f;

    private static readonly Color FillColor = new(0.85f, 0.65f, 0.2f);
    private static readonly Color OutlineColor = new(0.5f, 0.35f, 0.1f);

    public override void _Draw()
    {
        Vector2[] points =
        {
            new(0, -Radius),
            new(Radius * 0.87f, Radius * 0.5f),
            new(-Radius * 0.87f, Radius * 0.5f),
        };

        DrawColoredPolygon(points, FillColor);
        DrawPolyline(new[] { points[0], points[1], points[2], points[0] }, OutlineColor, 3f);
    }
}
