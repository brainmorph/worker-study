using Godot;

namespace WorkerStudy.Entities;

// Just rendering, movement, and state - a WorkerAi drives it by calling
// MoveToward/HasReached and setting IsCarryingFood.
public partial class Worker : Node2D
{
    [Export] public float Radius = 18f;
    [Export] public float Speed = 120f;
    [Export] public float DetectionRadius = 220f;
    [Export] public float WanderRadius = 250f;
    [Export] public float ArrivalTolerance = 6f;

    private static readonly Color BodyColor = new(0.3f, 0.55f, 0.8f);
    private static readonly Color OutlineColor = new(0.15f, 0.3f, 0.45f);
    private static readonly Color CarryingColor = new(0.85f, 0.65f, 0.2f);

    private Vector2 _facing = Vector2.Up;
    private bool _isCarryingFood;

    public bool IsCarryingFood
    {
        get => _isCarryingFood;
        set
        {
            _isCarryingFood = value;
            QueueRedraw();
        }
    }

    public void MoveToward(Vector2 target, double delta)
    {
        Vector2 offset = target - Position;
        float distance = offset.Length();
        if (distance <= 0.01f)
        {
            return;
        }

        Vector2 direction = offset / distance;
        float step = Speed * (float)delta;
        Position += direction * Mathf.Min(step, distance);
        _facing = direction;
        QueueRedraw();
    }

    public bool HasReached(Vector2 target, float tolerance = 4f)
    {
        return Position.DistanceTo(target) <= tolerance;
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, Radius, BodyColor);
        DrawArc(Vector2.Zero, Radius, 0, Mathf.Tau, 32, OutlineColor, 3f);
        DrawLine(Vector2.Zero, _facing * Radius, OutlineColor, 3f);

        if (IsCarryingFood)
        {
            DrawCircle(new Vector2(0, -Radius * 0.9f), Radius * 0.35f, CarryingColor);
        }
    }
}
