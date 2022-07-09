using UnityEngine;

public class CrouchingState : State
{
    public CrouchingState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        player.isCrouching = true;
        player.OnCrouchingChangedInvoke(true);

        player.coll.size = player.defaultSizeCollider * new Vector2(1, player.sizeModifierCrouch);

        //Abbassa il collider per la differenza calcolata
        var difference = player.defaultSizeCollider.y - (player.defaultSizeCollider.y * player.sizeModifierCrouch);
        player.coll.offset = -new Vector2(0, difference * 0.5f);

        player.crouchVelocity = Mathf.Abs(player.velocity.x);
        player.frameStartedCrouching = player.fixedFrame;
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
        CanStand();
        Crouch();
        player.HorizontalMovement();
        player.Move();
    }

    public override void Exit()
    {
        base.Exit();

        player.isCrouching = false;
        player.OnCrouchingChangedInvoke(false);
    }

    void Crouch()
    {
        var immediate = player.crouchVelocity <= player.immediateSlowdownLimitCrouch ? player.crouchSlowdownFrames : 0;
        var crouchPoint = Mathf.InverseLerp(0, player.crouchSlowdownFrames, player.fixedFrame - player.frameStartedCrouching + immediate);
        player.frameClamp *= Mathf.Lerp(1, player.speedModifierCrouch, crouchPoint);
    }

    void CanStand()
    {
        if (!player.isGrounded || (player.Input.Y >= 0 && player.isCrouching))
        {
            //Rileva se ci sono ostruzioni nell'area sopra la testa
            if (!player.CanStand) return;

            stateMachine.ChangeState(player.standingState);
        }
    }
}