public class VerticalAttackState : AirborneState
{
    public VerticalAttackState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        player.Anim.Play("AttaccoSalto");
    }

    public override void HandleInput() { } //DEVE RIMANERE VUOTO, COS� NON SI MUOVE DURANTE LA CADUTA

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.BaseMovement();
        player.CollisionsChecks();
        player.Gravity();
    }
}