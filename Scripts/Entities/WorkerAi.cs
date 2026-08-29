using Godot;
using WorkerStudy.BehaviorTree;
using WorkerStudy.World;

namespace WorkerStudy.Entities;

// The worker's brain: builds its behavior tree and holds the target/
// wander state the tree's leaves read and write. Tree shape:
//
//   Selector
//     Sequence(IsCarryingFood -> MoveToHome -> DropOffFood)
//     Sequence(
//       Selector(HasValidFoodTarget -> FindNearestFood)
//       MoveToFood -> PickUpFood)
//     Wander
public sealed class WorkerAi
{
    private readonly Worker _worker;
    private readonly FoodSpawner _foodSpawner;
    private readonly RandomNumberGenerator _rng = new();
    private readonly BTNode _root;

    private Food _targetFood;
    private Vector2 _wanderTarget;
    private bool _hasWanderTarget;
    private double _delta;

    public WorkerAi(Worker worker, FoodSpawner foodSpawner)
    {
        _worker = worker;
        _foodSpawner = foodSpawner;
        _rng.Randomize();
        _root = BuildTree();
    }

    public void Tick(double delta)
    {
        _delta = delta;
        _root.Tick();
    }

    private BTNode BuildTree()
    {
        return new Selector(
            new Sequence(
                new ConditionNode(() => _worker.IsCarryingFood),
                new ActionNode(MoveToHome),
                new ActionNode(DropOffFood)),
            new Sequence(
                new Selector(
                    new ConditionNode(HasValidFoodTarget),
                    new ActionNode(FindNearestFood)),
                new ActionNode(MoveToFood),
                new ActionNode(PickUpFood)),
            new ActionNode(Wander));
    }

    private bool HasValidFoodTarget()
    {
        return _targetFood is not null && GodotObject.IsInstanceValid(_targetFood);
    }

    private BTStatus FindNearestFood()
    {
        _worker.BtState = WorkerBtState.SeekFood;
        Food nearest = _foodSpawner.FindNearestFood(_worker.Position, _worker.DetectionRadius);
        if (nearest is null)
        {
            return BTStatus.Failure;
        }

        _targetFood = nearest;
        return BTStatus.Success;
    }

    private BTStatus MoveToFood()
    {
        _worker.BtState = WorkerBtState.SeekFood;

        // The target may have been collected by another worker since we
        // picked it - fall through so the tree can wander or re-search.
        if (!HasValidFoodTarget())
        {
            _targetFood = null;
            return BTStatus.Failure;
        }

        if (_worker.HasReached(_targetFood.Position, _worker.ArrivalTolerance))
        {
            return BTStatus.Success;
        }

        _worker.MoveToward(_targetFood.Position, _delta);
        return BTStatus.Running;
    }

    private BTStatus PickUpFood()
    {
        if (!HasValidFoodTarget())
        {
            return BTStatus.Failure;
        }

        _foodSpawner.CollectFood(_targetFood);
        _targetFood = null;
        _worker.IsCarryingFood = true;
        return BTStatus.Success;
    }

    private BTStatus MoveToHome()
    {
        _worker.BtState = WorkerBtState.ReturnHome;
        Vector2 home = WorldConfig.HomeBasePosition;
        if (_worker.HasReached(home, _worker.ArrivalTolerance))
        {
            return BTStatus.Success;
        }

        _worker.MoveToward(home, _delta);
        return BTStatus.Running;
    }

    private BTStatus DropOffFood()
    {
        _worker.IsCarryingFood = false;
        return BTStatus.Success;
    }

    private BTStatus Wander()
    {
        _worker.BtState = WorkerBtState.Wander;

        if (!_hasWanderTarget || _worker.HasReached(_wanderTarget, _worker.ArrivalTolerance))
        {
            _wanderTarget = PickWanderTarget();
            _hasWanderTarget = true;
        }

        _worker.MoveToward(_wanderTarget, _delta);
        return BTStatus.Running;
    }

    private Vector2 PickWanderTarget()
    {
        Vector2 offset = new(
            _rng.RandfRange(-_worker.WanderRadius, _worker.WanderRadius),
            _rng.RandfRange(-_worker.WanderRadius, _worker.WanderRadius));

        Rect2 bounds = WorldConfig.Bounds;
        Vector2 target = _worker.Position + offset;
        target.X = Mathf.Clamp(target.X, bounds.Position.X, bounds.End.X);
        target.Y = Mathf.Clamp(target.Y, bounds.Position.Y, bounds.End.Y);
        return target;
    }
}
