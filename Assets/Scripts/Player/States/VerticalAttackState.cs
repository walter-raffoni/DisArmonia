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

        player.MinFallSpeed = 40;
        player.MaxFallSpeed = 70;

        player.ComboAttacco = 0;
    }

    public override void HandleInput() //se non si vuole il movimento è da lasciare vuota ma non da rimuovere
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
        player.IsTopPoint();//permette all'animazione di atterraggio di airbornestate di non buggare tutto
        player.Gravity();
        player.Move();
    }
}