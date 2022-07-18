using UnityEngine;

public class Ranged_PatrollingState : State
{
    public Ranged_PatrollingState(EnemyRanged _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
    {
        enemyRanged = _enemy;
        stateMachine = _stateMachine;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (enemyRanged.facingRight)
        {
            enemyRanged.transform.position = Vector2.MoveTowards(enemyRanged.transform.position, enemyRanged.endPoint.transform.position, enemyRanged.speed * Time.fixedDeltaTime);
            if (Vector3.Distance(enemyRanged.transform.position, enemyRanged.endPoint.position) < 0.5f) enemyRanged.Flip();
        }
        else if (!enemyRanged.facingRight)
        {
            enemyRanged.transform.position = Vector2.MoveTowards(enemyRanged.transform.position, enemyRanged.startPoint.transform.position, enemyRanged.speed * Time.fixedDeltaTime);
            if (Vector3.Distance(enemyRanged.transform.position, enemyRanged.startPoint.position) < 0.5f) enemyRanged.Flip();
        }
    }
}
