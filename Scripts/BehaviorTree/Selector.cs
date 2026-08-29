using System.Collections.Generic;

namespace WorkerStudy.BehaviorTree;

// Runs children in order. Stops at the first child that doesn't fail
// (returning its Success or Running); fails only once every child has.
public sealed class Selector : BTNode
{
    private readonly IReadOnlyList<BTNode> _children;

    public Selector(params BTNode[] children)
    {
        _children = children;
    }

    public override BTStatus Tick()
    {
        foreach (BTNode child in _children)
        {
            BTStatus status = child.Tick();
            if (status != BTStatus.Failure)
            {
                return status;
            }
        }

        return BTStatus.Failure;
    }
}
