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

        if (enemyRanged.target.transform.position.x > enemyRanged.transform.position.x && !enemyRanged.FacingRight) enemyRanged.Flip();
        if (enemyRanged.target.transform.position.x < enemyRanged.transform.position.x && enemyRanged.FacingRight) enemyRanged.Flip();

        if (Vector3.Distance(enemyRanged.transform.position, enemyRanged.target.transform.position) >= enemyRanged.distPgTroppoLontano) stateMachine.ChangeState(enemyRanged.patrollingState);
        else if (Vector3.Distance(enemyRanged.transform.position, enemyRanged.target.transform.position) <= enemyRanged.distPgTroppoVicino) enemyRanged.transform.position = Vector2.MoveTowards(enemyRanged.transform.position, enemyRanged.transform.position + Vector3.right, enemyRanged.speed * Time.fixedDeltaTime);
    }
}