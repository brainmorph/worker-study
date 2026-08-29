namespace WorkerStudy.BehaviorTree;

// Flips its single child's Success/Failure; Running passes through
// unchanged.
public sealed class Inverter : BTNode
{
    private readonly BTNode _child;

    public Inverter(BTNode child)
    {
        _child = child;
    }

    public override BTStatus Tick()
    {
        BTStatus status = _child.Tick();
        return status switch
        {
            BTStatus.Success => BTStatus.Failure,
            BTStatus.Failure => BTStatus.Success,
            _ => status,
        };
    }
}
