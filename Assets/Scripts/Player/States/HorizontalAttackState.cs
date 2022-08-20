using UnityEngine;

public class HorizontalAttackState : State
{
    private float attackTimerDefault = 1.5f;
    private float currentAttackTimer = 1.5f;

    public HorizontalAttackState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        currentAttackTimer = attackTimerDefault;

        if (player.Stance == Player.TipoStance.Agile)
        {
            if (player.ComboAttacco == 0)
            {
                player.TempoAttaccoAttuale = player.TempoAttaccoOrizAgile;
                player.Anim.Play("Agile_Attacco1");
                player.ComboAttacco++;
            }
            else if (player.ComboAttacco == 1)
            {
                player.TempoAttaccoAttuale = player.TempoAttaccoOrizAgile * .5f;
                player.Anim.Play("Agile_Attacco2");
                player.ComboAttacco++;
            }
            else if (player.ComboAttacco == 2)
            {
                player.TempoAttaccoAttuale = player.TempoAttaccoOrizAgile;
                player.Anim.Play("Agile_Attacco3");
                player.ComboAttacco = 0;
            }
        }
        else if (player.Stance == Player.TipoStance.Brutale)
        {
            player.TempoAttaccoAttuale = player.TempoAttaccoOrizBrutale;
            if (player.ComboAttacco == 0)
            {
                player.Anim.Play("Brutale_Attacco1");
                player.ComboAttacco++;
            }
            else if (player.ComboAttacco == 1)
            {
                player.Anim.Play("Brutale_Attacco2");
                player.ComboAttacco++;
            }
            else if (player.ComboAttacco == 2)
            {
                player.Anim.Play("Brutale_Attacco3");
                player.ComboAttacco = 0;
            }
        }
    }

    public override void HandleInput()
    {
        base.HandleInput();

        player.HandleInput();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (currentAttackTimer > 0) currentAttackTimer -= Time.deltaTime;
        else
        {
            player.ComboAttacco = 0;
            player.stateMachine.ChangeState(player.standingState);
        }

        if (player.TempoAttaccoAttuale > 0) player.TempoAttaccoAttuale -= Time.deltaTime;
        else
        {
            if (player.Input.AttackDown && player.ComboAttacco <= 2) player.stateMachine.ChangeState(player.horizontalAttackState);
            else if (player.Input.X != 0) player.stateMachine.ChangeState(player.standingState);
            else if (player.Input.JumpDown) player.stateMachine.ChangeState(player.airborneState);
        }
    }
}