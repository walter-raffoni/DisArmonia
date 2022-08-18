using UnityEngine;

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

        if (player.Stance == Player.TipoStance.Agile) player.Anim.Play("IdleAgile");
        else if (player.Stance == Player.TipoStance.Brutale) player.Anim.Play("Idle");
    }

    public override void HandleInput()
    {
        base.HandleInput();

        player.HandleInput();

        if (player.Input.AttackDown) stateMachine.ChangeState(player.horizontalAttackState);

        if (player.Input.BrutalAttackDown && player.Stance == Player.TipoStance.Brutale)
        {
            if (player.StackDiSangue == 0) return;
            else if (player.StackDiSangue >= 1 && player.StackDiSangue <= 3) stateMachine.ChangeState(player.brutalAttackState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.BaseMovement();

        player.CollisionsChecks();

        player.HorizontalMovement();
        IsTopPoint();
        player.Gravity();
        CanJump();
        CanDash();
        player.Move();
    }

    void IsTopPoint()
    {
        if (!player.IsGrounded)
        {
            player.TopPoint = Mathf.InverseLerp(player.JumpTopLimit, 0, Mathf.Abs(player.Velocity.y));//Diventa sempre più forte man mano che ci sia avvicina alla cima
            player.FallSpeed = Mathf.Lerp(player.MinFallSpeed, player.MaxFallSpeed, player.TopPoint);
        }
        else player.TopPoint = 0;
    }

    void CanJump()
    {
        //Controlla se: Ha premuto il tasto di salto o comunque nella soglia possibile per il salto "coyote" || Se c'è un buffer per il salto sufficiente || È a terra
        if ((player.JumpToConsume && player.CanUseCoyote) || player.HasBufferedJump || !player.IsGrounded) stateMachine.ChangeState(player.airborneState);
    }

    void CanDash()
    {
        if (player.DashToConsume && player.CanDash && player.Input.X != 0 && player.DashAbility && player.CooldownDashAttuale <= 0) stateMachine.ChangeState(player.dashingState);
    }
}