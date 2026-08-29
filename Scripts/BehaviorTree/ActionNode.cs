using System;

namespace WorkerStudy.BehaviorTree;

// Wraps an arbitrary delegate as a leaf, so worker-specific behaviors
// (move, pick up food, ...) can plug into the tree without a dedicated
// class per action.
public sealed class ActionNode : BTNode
{
    private readonly Func<BTStatus> _action;

    public ActionNode(Func<BTStatus> action)
    {
        _action = action;
    }

    public override BTStatus Tick() => _action();
}
