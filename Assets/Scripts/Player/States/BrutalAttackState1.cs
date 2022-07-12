public class BrutalAttackState1 : State
{
    public BrutalAttackState1(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
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