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

        if (player.Stance == Player.TipoStance.Agile)
        {
            if (player.ComboAttacco == 0)
            {
                player.TempoAttaccoAttuale = player.TempoAttaccoOrizAgile;
                player.Anim.Play("Attacco1Agile");
            }
            else if (player.ComboAttacco == 1)
            {
                player.TempoAttaccoAttuale = player.TempoAttaccoOrizAgile / 2;
                player.Anim.Play("Attacco2Agile");
                player.ComboAttacco = 0;
            }
            else if (player.ComboAttacco == 2)
            {
                player.TempoAttaccoAttuale = player.TempoAttaccoOrizAgile / 2;
                player.Anim.Play("Attacco3Agile");
                player.ComboAttacco = 0;
            }
        }
        else if (player.Stance == Player.TipoStance.Brutale)
        {
            player.TempoAttaccoAttuale = player.TempoAttaccoOrizBrutale;
            if (player.ComboAttacco == 0) player.Anim.Play("Attacco1");
            else if (player.ComboAttacco == 1)
            {
                player.Anim.Play("Attacco2");
                player.ComboAttacco = 0;
            }
            else if (player.ComboAttacco == 2)
            {
                player.Anim.Play("Attacco3");
                player.ComboAttacco = 0;
            }
        }
    }

    public override void HandleInput()
    {
        base.HandleInput();

        player.HandleInput();

        if (player.Input.AttackDown && player.ComboAttacco < 2) player.ComboAttacco++;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.TempoAttaccoAttuale > 0) player.TempoAttaccoAttuale -= Time.deltaTime;
        else
        {
            if (player.ComboAttacco == 0) player.stateMachine.ChangeState(player.standingState);
            else if (player.ComboAttacco == 1) player.stateMachine.ChangeState(player.horizontalAttackState);
            else if (player.ComboAttacco == 2) player.stateMachine.ChangeState(player.horizontalAttackState);
        }
    }
}