using System.Collections.Generic;
using Godot;
using WorkerStudy.World;

namespace WorkerStudy.Entities;

// Spawns the workers near home base, gives each one a WorkerAi brain, and
// ticks those brains every frame - this is what actually drives movement
// now that the behavior tree exists.
public partial class WorkerSpawner : Node2D
{
    [Export] public int WorkerCount = 5;
    [Export] public float SpawnRadius = 60f;

    private readonly List<WorkerAi> _brains = new();
    private readonly RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        _rng.Randomize();
        var foodSpawner = GetNode<FoodSpawner>("../FoodSpawner");

        for (int i = 0; i < WorkerCount; i++)
        {
            SpawnWorker(foodSpawner);
        }
    }

    public override void _Process(double delta)
    {
        foreach (WorkerAi brain in _brains)
        {
            brain.Tick(delta);
        }
    }

    private void SpawnWorker(FoodSpawner foodSpawner)
    {
        Vector2 offset = new(
            _rng.RandfRange(-SpawnRadius, SpawnRadius),
            _rng.RandfRange(-SpawnRadius, SpawnRadius));

        var worker = new Worker { Position = WorldConfig.HomeBasePosition + offset };
        AddChild(worker);
        _brains.Add(new WorkerAi(worker, foodSpawner));
    }
}
