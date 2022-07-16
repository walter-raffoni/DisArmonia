using UnityEngine;

public class HorizontalAttackState : State
{
    public HorizontalAttackState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        player.tempoAttaccoAttuale = player.tempoAttacco;

        player.OnStartAttackChangedInvoke();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        player.HandleInput();

        if (player.Input.AttackDown) player.stateMachine.ChangeState(player.horizontalAttackState);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.tempoAttaccoAttuale > 0) player.tempoAttaccoAttuale -= Time.deltaTime;
        else player.stateMachine.ChangeState(player.standingState);
    }
}