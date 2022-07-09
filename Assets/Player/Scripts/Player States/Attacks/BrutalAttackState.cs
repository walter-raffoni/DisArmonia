public class BrutalAttackState : State
{
    public BrutalAttackState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        player.HandleInput();
    }
}