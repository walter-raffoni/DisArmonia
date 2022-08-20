using UnityEngine;

public class VerticalAttackState : State
{
    public VerticalAttackState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        player.Anim.Play("Brutale_AttaccoSalto");
    }

    public override void HandleInput() { } //DEVE RIMANERE VUOTO, COSì NON SI MUOVE DURANTE LA CADUTA

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.BaseMovement();

        player.CollisionsChecks();

        player.HorizontalMovement();
        IsTopPoint();
        player.Gravity();
        player.Move();
    }

    void IsTopPoint()//permette all'animazione di atterraggio di airbornestate di non buggare tutto
    {
        if (!player.IsGrounded)
        {
            player.TopPoint = Mathf.InverseLerp(player.JumpTopLimit, 0, Mathf.Abs(player.Velocity.y));//Diventa sempre più forte man mano che ci sia avvicina alla cima
            player.FallSpeed = Mathf.Lerp(player.MinFallSpeed, player.MaxFallSpeed, player.TopPoint);
        }
        else stateMachine.ChangeState(player.standingState);
    }
}