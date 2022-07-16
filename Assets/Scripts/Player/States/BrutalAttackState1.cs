using UnityEngine;

public class BrutalAttackState1 : State
{
    public BrutalAttackState1(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        player.tempoAttaccoAttuale = player.tempoAttacco;
        player.anim.SetInteger("StackSangue", player.stackDiSangue);
        player.OnStartBrutalAttackChangedInvoke();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.tempoAttaccoAttuale > 0) player.tempoAttaccoAttuale -= Time.deltaTime;
        else player.stateMachine.ChangeState(player.standingState);
    }

    public override void Exit()
    {
        base.Exit();

        player.stackDiSangue = 0;
        player.anim.SetInteger("StackSangue", player.stackDiSangue);
    }
}