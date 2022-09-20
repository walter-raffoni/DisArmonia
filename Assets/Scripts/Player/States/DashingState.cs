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

        player.DashEffect();

        player.Anim.Play("Agile_Dash");

        player.MinFallSpeed = 80;
        player.MaxFallSpeed = 160;

        player.ComboAttacco = 0;

        player.IsInvulnerable = true;
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
        player.Dash();
        player.Move();
    }

    public override void Exit()
    {
        base.Exit();

        player.JumpToConsume = false;//impedisce che salti se premi il tasto mentre in dash

        player.CooldownDashAttuale = player.CooldownDash;

        player.IsInvulnerable = false;
    }
}