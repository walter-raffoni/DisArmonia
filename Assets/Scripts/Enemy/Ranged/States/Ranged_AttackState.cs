using UnityEngine;

public class Ranged_AttackState : State
{
    public Ranged_AttackState(EnemyRanged _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
    {
        enemyRanged = _enemy;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        enemyRanged.InvokeRepeating("Spara", 0, 1);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        enemyRanged.transform.position = Vector2.MoveTowards(enemyRanged.transform.position, enemyRanged.target.transform.position, enemyRanged.speedAttacco * Time.fixedDeltaTime);
    }
}
