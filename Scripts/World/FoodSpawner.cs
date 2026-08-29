using System.Collections.Generic;
using Godot;

namespace WorkerStudy.World;

// Owns the active Food instances: spawns the initial batch, replaces any
// food that gets collected so the world never runs dry, and lets worker
// behavior trees query for the nearest food in range.
public partial class FoodSpawner : Node2D
{
    [Export] public int InitialFoodCount = 5;
    [Export] public float MinDistanceFromHomeBase = 150f;

    private readonly List<Food> _activeFood = new();
    private readonly RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        _rng.Randomize();
        for (int i = 0; i < InitialFoodCount; i++)
        {
            SpawnFood();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Debug aid: press Space to simulate a worker collecting a random
        // food item, so spawn/despawn/respawn can be verified before any
        // worker or behavior tree logic exists.
        if (@event is InputEventKey { Keycode: Key.Space, Pressed: true } && _activeFood.Count > 0)
        {
            int index = _rng.RandiRange(0, _activeFood.Count - 1);
            CollectFood(_activeFood[index]);
        }
    }

    public void CollectFood(Food food)
    {
        _activeFood.Remove(food);
        food.QueueFree();
        SpawnFood();
    }

    // Returns the closest active food within maxDistance of origin, or
    // null if none qualifies.
    public Food FindNearestFood(Vector2 origin, float maxDistance)
    {
        Food nearest = null;
        float nearestDistance = maxDistance;

        foreach (Food food in _activeFood)
        {
            float distance = origin.DistanceTo(food.Position);
            if (distance <= nearestDistance)
            {
                nearest = food;
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    private void SpawnFood()
    {
        var food = new Food { Position = GetRandomFoodPosition() };
        AddChild(food);
        _activeFood.Add(food);
    }

    private Vector2 GetRandomFoodPosition()
    {
        Rect2 bounds = WorldConfig.Bounds;
        Vector2 point;

        do
        {
            point = new Vector2(
                _rng.RandfRange(bounds.Position.X, bounds.End.X),
                _rng.RandfRange(bounds.Position.Y, bounds.End.Y));
        } while (point.DistanceTo(WorldConfig.HomeBasePosition) < MinDistanceFromHomeBase);

        return point;
    }
}
