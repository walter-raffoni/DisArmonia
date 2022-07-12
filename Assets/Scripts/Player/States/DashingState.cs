using UnityEngine;

public class DashingState : State
{
    public DashingState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        player.HandleInput();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.BaseMovement();

        player.CollisionsChecks();

        player.HorizontalMovement();
        player.Gravity();
        Dash();
        player.Move();
    }

    void Dash()
    {
        if (player.dashToConsume && player.canDash)
        {
            var vel = new Vector2(player.Input.X, player.isGrounded && player.Input.Y < 0 ? 0 : player.Input.Y).normalized;
            if (vel == Vector2.zero) { player.dashToConsume = false; return; }
            player.dashVelocity = vel * player.dashPower;
            player.OnDashingChangedInvoke(true);
            player.canDash = false;
            player.hasStartedDashing = player.fixedFrame;

            player.forceBuildup = Vector2.zero;//Toglie ogni forza aggiunta dall'esterno
        }

        player.speed.x = player.dashVelocity.x;
        player.speed.y = player.dashVelocity.y;

        //Annulla la corsa quando il tempo è finita o è stata raggiunta la distanza di sicurezza mssima
        if (player.hasStartedDashing + player.dashLength < player.fixedFrame)
        {
            player.OnDashingChangedInvoke(false);
            if (player.speed.y > 0) player.speed.y = 0;
            player.speed.x *= player.horizontalMultiplierDashEnd;
            if (player.isGrounded) player.canDash = true;
            stateMachine.ChangeState(player.standingState);
        }
        player.dashToConsume = false;
    }
}