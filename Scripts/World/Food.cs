using Godot;

namespace WorkerStudy.World;

public partial class Food : Node2D
{
    [Export] public float Radius = 14f;

    private static readonly Color FillColor = new(0.75f, 0.15f, 0.15f);
    private static readonly Color HighlightColor = new(0.95f, 0.5f, 0.45f);

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, Radius, FillColor);
        DrawCircle(new Vector2(-Radius * 0.3f, -Radius * 0.3f), Radius * 0.3f, HighlightColor);
    }
}
