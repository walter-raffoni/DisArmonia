public class State
{
    public Player player;
    public Enemy enemy;
    public StateMachine stateMachine;

    public State(Player _player, StateMachine _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public State(Enemy _enemy, StateMachine _stateMachine)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
    }

    public virtual void Enter() { }

    public virtual void HandleInput() { }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void Exit() { }
}