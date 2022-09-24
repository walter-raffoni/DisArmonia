using UnityEngine;

public class Ranged_PatrollingState : State
{
    public Ranged_PatrollingState(EnemyRanged _enemyRanged, StateMachine _stateMachine) : base(_enemyRanged, _stateMachine)
    {
        enemyRanged = _enemyRanged;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        enemyRanged.Anim.Play("Ranged_IdleAndAttack");

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
