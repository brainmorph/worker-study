using Godot;

namespace WorkerStudy.World;

public partial class GroundArea : Node2D
{
    [Export] public int GridSpacing = 100;

    private static readonly Color GroundColor = new(0.16f, 0.18f, 0.16f);
    private static readonly Color GridColor = new(0.24f, 0.26f, 0.24f);
    private static readonly Color BorderColor = new(0.4f, 0.42f, 0.4f);

    public override void _Draw()
    {
        Rect2 bounds = WorldConfig.Bounds;

        DrawRect(bounds, GroundColor);

        for (float x = bounds.Position.X; x <= bounds.End.X; x += GridSpacing)
        {
            DrawLine(new Vector2(x, bounds.Position.Y), new Vector2(x, bounds.End.Y), GridColor);
        }

        for (float y = bounds.Position.Y; y <= bounds.End.Y; y += GridSpacing)
        {
            DrawLine(new Vector2(bounds.Position.X, y), new Vector2(bounds.End.X, y), GridColor);
        }

        DrawRect(bounds, BorderColor, filled: false, width: 4f);
    }
}
