using UnityEngine;
using static Player;

public class StandingState : State
{
    public StandingState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        player.DashToConsume = false;

        player.GetComponent<SpriteRenderer>().flipX = false;//serve per togliere il flip dato dal dash in direzione del mouse

        if (player.Stance == Player.TipoStance.Agile) player.Anim.Play("Agile_Idle");
        else if (player.Stance == Player.TipoStance.Brutale) player.Anim.Play("Brutale_Idle");

        player.MinFallSpeed = 80;
        player.MaxFallSpeed = 160;

        player.ComboAttacco = 0;

        player.GroundedEffect();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        player.HandleInput();

        if (player.Input.AttackDown) stateMachine.ChangeState(player.horizontalAttackState);

        if (player.Input.BrutalAttackDown && player.Stance == Player.TipoStance.Brutale)
        {
            if (player.StackDiSangue == 0) return;
            else if (player.StackDiSangue >= 1 && player.StackDiSangue <= 3 && player.CooldownAttaccoPotenteAttuale <= 0) stateMachine.ChangeState(player.brutalAttackState);
        }

        if (player.Input.X != 0) player.transform.localScale = new Vector3(player.Input.X > 0 ? 1 : -1, 1, 1);//Ribalta lo sprite in orizzontale, è in questo controllo per evitare che si ribalti durante la pausa
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.BaseMovement();

        player.CollisionsChecks();

        player.HorizontalMovement();
        player.IsTopPoint();
        player.Gravity();
        player.CanJump();
        player.CanDashNow();
        player.Move();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        //Extended animator
        var inputPoint = Mathf.Abs(player.Input.X);

        if (player.Stance == TipoStance.Agile)
        {
            if (inputPoint == 0) player.Anim.Play("Agile_Idle");
            else if (inputPoint > 0) player.Anim.Play("Agile_Running");
        }
        else if (player.Stance == TipoStance.Brutale)
        {
            if (inputPoint == 0) player.Anim.Play("Brutale_Idle");
            else if (inputPoint > 0) player.Anim.Play("Brutale_Running");
        }

    }
}