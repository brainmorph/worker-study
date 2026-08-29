using System.Collections.Generic;
using Godot;
using WorkerStudy.World;

namespace WorkerStudy.Entities;

// Spawns the 5 workers near home base. Until the behavior tree exists
// (Phase 7), this also drives movement directly as a debug aid.
public partial class WorkerSpawner : Node2D
{
    [Export] public int WorkerCount = 5;
    [Export] public float SpawnRadius = 60f;

    private readonly List<Worker> _workers = new();
    private readonly Dictionary<Worker, Vector2> _debugMoveTargets = new();
    private readonly RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        _rng.Randomize();
        for (int i = 0; i < WorkerCount; i++)
        {
            SpawnWorker();
        }
    }

    public override void _Process(double delta)
    {
        foreach ((Worker worker, Vector2 target) in _debugMoveTargets)
        {
            worker.MoveToward(target, delta);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Debug aid: left-click sends every worker toward the clicked
        // point, so MoveToward can be verified before any behavior tree
        // logic exists.
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
        {
            Vector2 target = GetGlobalMousePosition();
            foreach (Worker worker in _workers)
            {
                _debugMoveTargets[worker] = target;
            }
        }
    }

    private void SpawnWorker()
    {
        Vector2 offset = new(
            _rng.RandfRange(-SpawnRadius, SpawnRadius),
            _rng.RandfRange(-SpawnRadius, SpawnRadius));

        var worker = new Worker { Position = WorldConfig.HomeBasePosition + offset };
        AddChild(worker);
        _workers.Add(worker);
    }
}
