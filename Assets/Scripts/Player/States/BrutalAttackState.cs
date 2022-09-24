using UnityEngine;

public class BrutalAttackState : State
{
    public BrutalAttackState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        player.TempoAttaccoAttuale = player.TempoAttaccoBrutale;
        if (player.StackDiSangue == 1) player.Anim.Play("Brutale_BassaCarica");
        else if (player.StackDiSangue == 2) player.Anim.Play("Brutale_MediaCarica");
        else if (player.StackDiSangue == 3) player.Anim.Play("Brutale_PienaCarica");

        player.MinFallSpeed = 80;
        player.MaxFallSpeed = 160;

        player.ComboAttacco = 0;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.TempoAttaccoAttuale > 0) player.TempoAttaccoAttuale -= Time.deltaTime;
        else player.stateMachine.ChangeState(player.standingState);
    }

    public override void Exit()
    {
        base.Exit();

        player.CambiaStack(0);

        player.CooldownAttaccoPotenteAttuale = player.CooldownAttaccoPotente;
    }
}