using UnityEngine;

public class Ranged_PatrollingState : State
{
    public Ranged_PatrollingState(EnemyRanged _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
    {
        enemyRanged = _enemy;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        enemyRanged.Anim.Play("RangedAnimation");

        enemyRanged.CancelInvoke("Spara");//sennò continua a sparare
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (enemyRanged.FacingRight)
        {
            enemyRanged.transform.position = Vector2.MoveTowards(enemyRanged.transform.position, enemyRanged.EndPoint.position, enemyRanged.Speed * Time.fixedDeltaTime);
            if (Vector3.Distance(enemyRanged.transform.position, enemyRanged.EndPoint.position) < 0.5f) enemyRanged.Flip();
        }
        else if (!enemyRanged.FacingRight)
        {
            enemyRanged.transform.position = Vector2.MoveTowards(enemyRanged.transform.position, enemyRanged.StartPoint.position, enemyRanged.Speed * Time.fixedDeltaTime);
            if (Vector3.Distance(enemyRanged.transform.position, enemyRanged.StartPoint.position) < 0.5f) enemyRanged.Flip();
        }

        if (Vector3.Distance(enemyRanged.transform.position, enemyRanged.Target.position) <= enemyRanged.DistAttaccoPg) stateMachine.ChangeState(enemyRanged.attackState);
    }
}
