using UnityEngine;

public class DashingState : State
{
    public DashingState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        player.Anim.Play("Agile_Dash");

        player.MinFallSpeed = 80;
        player.MaxFallSpeed = 160;

        //Debug.Log(stateMachine.currentState);
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
        int intFacingRight = 0;
        Vector2 velocityExt = Vector2.zero;

        if (player.DashFacingRight == false) intFacingRight = -1;
        else if (player.DashFacingRight == true) intFacingRight = 1;

        if (player.DashToConsume && player.CanDash)
        {
            if ((intFacingRight == -1) && (intFacingRight - player.Input.X == 0)) velocityExt = new Vector2(-1, player.IsGrounded && player.Input.Y < 0 ? 0 : player.Input.Y).normalized;
            else if ((intFacingRight == 1) && (intFacingRight - player.Input.X == 0)) velocityExt = new Vector2(1, player.IsGrounded && player.Input.Y < 0 ? 0 : player.Input.Y).normalized;

            else if ((intFacingRight == -1) && (intFacingRight - player.Input.X != 0) || (intFacingRight == 1) && (intFacingRight - player.Input.X != 0))
            {
                velocityExt = new Vector2(intFacingRight, player.IsGrounded && player.Input.Y < 0 ? 0 : player.Input.Y).normalized;
                player.GetComponent<SpriteRenderer>().flipX = true;
            }

            if (velocityExt == Vector2.zero) { player.DashToConsume = false; return; }
            player.DashVelocity = velocityExt * player.DashPower;
            player.OnDashingChanged(true);
            player.CanDash = false;
            player.HasStartedDashing = player.FixedFrame;

            player.ForceBuildup = Vector2.zero;//Toglie ogni forza aggiunta dall'esterno
        }

        player.Speed.x = player.DashVelocity.x;
        player.Speed.y = player.DashVelocity.y;

        //Annulla la corsa quando il tempo è finita o è stata raggiunta la distanza di sicurezza mssima
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
            else stateMachine.ChangeState(player.airborneState);//così non passa allo stato di standing per un attimo anche se in aria
        }
        player.DashToConsume = false;
    }
}