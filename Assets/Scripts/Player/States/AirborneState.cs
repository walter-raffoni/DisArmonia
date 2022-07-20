using UnityEngine;

public class AirborneState : State
{
    public AirborneState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        if (player.JumpToConsume)
        {
            player.Speed.y = player.JumpHeight;
            player.EndedJumpEarly = false;
            player.CanUseCoyotePriv = false;
            player.JumpToConsume = false;
            player.TimeLeftGrounded = player.FixedFrame;
            player.DidBufferedJump = true;
            player.OnJumping();
        }

        if (player.IsGrounded)//Fa riprodurre il particellare solo quando � a terra
        {
            player.SetColor(player.JumpParticles);
            player.SetColor(player.LaunchParticles);
            player.JumpParticles.Play();
        }

        if (player.Stance == Player.TipoStance.Brutale) player.Anim.Play("Jump");
        else if (player.Stance == Player.TipoStance.Agile) player.Anim.Play("JumpAgile");
    }

    public override void HandleInput()
    {
        base.HandleInput();

        player.HandleInput();

        if (player.DashToConsume && player.CanDash && player.Input.X != 0 && player.Stance == Player.TipoStance.Agile) player.stateMachine.ChangeState(player.dashingState);

        if (player.Input.AttackDown && player.Stance == Player.TipoStance.Brutale) stateMachine.ChangeState(player.verticalAttackState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.BaseMovement();

        player.CollisionsChecks();

        player.HorizontalMovement();
        IsTopPoint();
        player.Gravity();
        Jump();
        player.Move();
    }

    void Jump()
    {
        if (player.DoubleJumpAbility)
        {
            //Controlla se c'� un salto da poter consumare e pu� effettivamente fare il doppio salto
            if (player.JumpToConsume && player.CanDoubleJump)
            {
                player.Speed.y = player.JumpHeight;
                player.CanUseDoubleJump = false;
                player.EndedJumpEarly = false;
                player.JumpToConsume = false;
                player.OnDoubleJumping();
            }
        }

        // Termina il salto se il pulsante viene rilasciato prima
        if (!player.IsGrounded && !player.Input.JumpHeld && !player.EndedJumpEarly && player.Velocity.y > 0) player.EndedJumpEarly = true;

        if (player.HasHitUp && player.Speed.y > 0) player.Speed.y = 0;
    }

    void IsTopPoint()
    {
        if (!player.IsGrounded)
        {
            player.TopPoint = Mathf.InverseLerp(player.JumpTopLimit, 0, Mathf.Abs(player.Velocity.y));//Diventa sempre pi� forte man mano che ci sia avvicina alla cima
            player.FallSpeed = Mathf.Lerp(player.MinFallSpeed, player.MaxFallSpeed, player.TopPoint);
        }
        else stateMachine.ChangeState(player.standingState);
    }
}