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

    public override void Exit()
    {
        base.Exit();

        player.JumpToConsume = false;//impedisce che salti se premi il tasto mentre in dash

        player.CooldownDashAttuale = player.CooldownDash;
    }

    void Dash()
    {
        float floatFacingRight = 0;

        if (player.DashFacingRight == false) floatFacingRight = -1;
        else if (player.DashFacingRight == true) floatFacingRight = 1;

        if (floatFacingRight - player.transform.localScale.x == 0)
        {
            if (player.DashToConsume && player.CanDash)
            {
                var vel = new Vector2(floatFacingRight, player.IsGrounded && player.Input.Y < 0 ? 0 : player.Input.Y).normalized;
                if (vel == Vector2.zero) { player.DashToConsume = false; return; }
                //if (floatFacingRight - player.transform.localScale.x != 0) stateMachine.ChangeState(player.standingState);
                player.DashVelocity = vel * player.DashPower;
                player.OnDashingChanged(true);
                player.CanDash = false;
                player.HasStartedDashing = player.FixedFrame;

                player.ForceBuildup = Vector2.zero;//Toglie ogni forza aggiunta dall'esterno
            }

            player.Speed.x = player.DashVelocity.x;
            player.Speed.y = player.DashVelocity.y;

            //Annulla la corsa quando il tempo � finita o � stata raggiunta la distanza di sicurezza mssima
            if (player.HasStartedDashing + player.DashLength < player.FixedFrame)
            {
                player.OnDashingChanged(false);
                if (player.Speed.y > 0) player.Speed.y = 0;
                player.Speed.x *= player.HorizontalMultiplierDashEnd;
                if (player.IsGrounded)
                {
                    player.CanDash = true;
                    stateMachine.ChangeState(player.standingState);
                }
                else stateMachine.ChangeState(player.airborneState);//cos� non passa allo stato di standing per un attimo anche se in aria
            }
            player.DashToConsume = false;
        }
        else stateMachine.ChangeState(player.standingState);
    }
}