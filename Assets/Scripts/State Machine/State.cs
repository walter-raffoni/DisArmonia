using UnityEngine;

public class State
{
    public Player player;
    public EnemyMelee enemyMelee;
    public EnemyRanged enemyRanged;
    public StateMachine stateMachine;

    public State(Player _player, StateMachine _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public State(EnemyMelee _enemyMelee, StateMachine _stateMachine)
    {
        enemyMelee = _enemyMelee;
        stateMachine = _stateMachine;
    }

    public State(EnemyRanged _enemyRanged, StateMachine _stateMachine)
    {
        enemyRanged = _enemyRanged;
        stateMachine = _stateMachine;
    }

    public virtual void Enter()
    {
        //Debug.Log(stateMachine.currentState);
    }

    public virtual void HandleInput() { }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void Exit() { }
}