using Godot;

namespace WorkerStudy.World;

public static class WorldConfig
{
    public static readonly Vector2 WorldSize = new(2000, 1200);

    public static Rect2 Bounds => new(-WorldSize / 2f, WorldSize);
}
