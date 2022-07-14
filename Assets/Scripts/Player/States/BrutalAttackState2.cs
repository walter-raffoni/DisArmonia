using UnityEngine;

public class BrutalAttackState2 : State
{
    public BrutalAttackState2(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        player.tempoAttacco = player.tempoAttaccoOriginale;
        player.anim.SetInteger("StackSangue", player.stackDiSangue);
        player.OnStartBrutalAttackChangedInvoke();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.tempoAttacco > 0) player.tempoAttacco -= Time.deltaTime;
        else player.stateMachine.ChangeState(player.standingState);
    }

    public override void Exit()
    {
        base.Exit();

        player.stackDiSangue = 0;
        player.anim.SetInteger("StackSangue", player.stackDiSangue);
    }
}