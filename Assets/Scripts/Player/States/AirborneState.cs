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

        if (player.jumpToConsume)
        {
            player.speed.y = player.jumpHeight;
            player.endedJumpEarly = false;
            player.canUseCoyote = false;
            player.jumpToConsume = false;
            player.timeLeftGrounded = player.fixedFrame;
            player.didBufferedJump = true;
            player.OnJumpingInvoke();
        }
    }

    public override void HandleInput()
    {
        base.HandleInput();

        player.HandleInput();

        if (player.dashToConsume && player.canDash && player.Input.X != 0 && player.stance == Player.Stance.Agile) player.stateMachine.ChangeState(player.dashingState);

        if (player.Input.AttackDown) stateMachine.ChangeState(player.verticalAttackState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.BaseMovement();

        player.CollisionsChecks();

        player.HorizontalMovement();
        TopPoint();
        player.Gravity();
        Jump();
        player.Move();
    }

    void Jump()
    {
        if (player.doubleJumpAbility)
        {
            //Controlla se c'è un salto da poter consumare e può effettivamente fare il doppio salto
            if (player.jumpToConsume && player.CanDoubleJump)
            {
                player.speed.y = player.jumpHeight;
                player.canUseDoubleJump = false;
                player.endedJumpEarly = false;
                player.jumpToConsume = false;
                player.OnDoubleJumpingInvoke();
            }
        }

        // Termina il salto se il pulasnte viene rilasciato prima
        if (!player.isGrounded && !player.Input.JumpHeld && !player.endedJumpEarly && player.velocity.y > 0) player.endedJumpEarly = true;

        if (player.hasHitUp && player.speed.y > 0) player.speed.y = 0;
    }

    void TopPoint()
    {
        if (!player.isGrounded)
        {
            player.topPoint = Mathf.InverseLerp(player.jumpTopLimit, 0, Mathf.Abs(player.velocity.y));//Diventa sempre più forte man mano che ci sia avvicina alla cima
            player.fallSpeed = Mathf.Lerp(player.minFallSpeed, player.maxFallSpeed, player.topPoint);
        }
        else stateMachine.ChangeState(player.standingState);
    }
}