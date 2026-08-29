using System;

namespace WorkerStudy.BehaviorTree;

// Wraps a boolean check as a leaf: Success if true, Failure if false.
// Never returns Running.
public sealed class ConditionNode : BTNode
{
    private readonly Func<bool> _condition;

    public ConditionNode(Func<bool> condition)
    {
        _condition = condition;
    }

    public override BTStatus Tick() => _condition() ? BTStatus.Success : BTStatus.Failure;
}
