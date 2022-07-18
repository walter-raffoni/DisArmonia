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

        player.coll.size = player.defaultSizeCollider;
        player.coll.offset = player.defaultOffsetCollider;

        player.dashToConsume = false;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        player.HandleInput();

        if (player.Input.AttackDown) stateMachine.ChangeState(player.horizontalAttackState);
        
        if(player.Input.BrutalAttackDown /*&& player.stance == Player.Stance.Brutale*/)
        {
            if (player.stackDiSangue == 0) return;
            else if (player.stackDiSangue == 1) stateMachine.ChangeState(player.brutalAttackState1);
            else if (player.stackDiSangue == 2) stateMachine.ChangeState(player.brutalAttackState2);
            else if (player.stackDiSangue == 3) stateMachine.ChangeState(player.brutalAttackState3);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.BaseMovement();

        player.CollisionsChecks();

        player.HorizontalMovement();
        TopPoint();
        player.Gravity();
        CanJump();
        CanDash();
        player.Move();
    }

    public override void Exit()
    {
        base.Exit();

        player.isAttackOver = false;
    }

    void TopPoint()
    {
        if (!player.isGrounded)
        {
            player.topPoint = Mathf.InverseLerp(player.jumpTopLimit, 0, Mathf.Abs(player.velocity.y));//Diventa sempre più forte man mano che ci sia avvicina alla cima
            player.fallSpeed = Mathf.Lerp(player.minFallSpeed, player.maxFallSpeed, player.topPoint);
        }
        else player.topPoint = 0;
    }

    void CanJump()
    {
        if (!player.CanStand) return;

        //Controlla se: Ha premuto il tasto di salto o comunque nella soglia possibile per il salto "coyote" || Se c'è un buffer per il salto sufficiente || È a terra
        if ((player.jumpToConsume && player.CanUseCoyote) || player.HasBufferedJump || !player.isGrounded) stateMachine.ChangeState(player.airborneState);
    }

    void CanDash()
    {
        if (player.dashToConsume && player.canDash && player.Input.X != 0 && player.dashAbility) stateMachine.ChangeState(player.dashingState);
    }
}