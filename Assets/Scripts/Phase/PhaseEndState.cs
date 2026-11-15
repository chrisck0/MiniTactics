using UnityEngine;

public sealed class PhaseEndState : StateBase
{
    private PhaseStateController _controller;

    public PhaseEndState(PhaseStateController controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
    }
}
