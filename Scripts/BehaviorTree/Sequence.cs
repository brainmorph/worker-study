using System.Collections.Generic;

namespace WorkerStudy.BehaviorTree;

// Runs children in order. Stops at the first child that doesn't succeed
// (returning its Failure or Running); succeeds only once every child has.
public sealed class Sequence : BTNode
{
    private readonly IReadOnlyList<BTNode> _children;

    public Sequence(params BTNode[] children)
    {
        _children = children;
    }

    public override BTStatus Tick()
    {
        foreach (BTNode child in _children)
        {
            BTStatus status = child.Tick();
            if (status != BTStatus.Success)
            {
                return status;
            }
        }

        return BTStatus.Success;
    }
}
