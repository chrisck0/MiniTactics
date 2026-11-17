using UnityEngine;

public sealed class CommandEndState : StateBase
{
    private UnitCommandStateController _controller;

    public CommandEndState(UnitCommandStateController controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
        Debug.Log("CommandEndState Enter");
        _controller.HideCursor();
        _controller.OnUnitCommandEnd?.Invoke(_controller.SelectedPlayerUnit);
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}
