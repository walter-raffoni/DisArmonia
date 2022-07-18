using UnityEngine;

public class Melee_PatrollingState : State
{
    public Melee_PatrollingState(EnemyMelee _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
    {
        enemyMelee = _enemy;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        enemyMelee.anim.ResetTrigger("VistoNemico");
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (enemyMelee.facingRight)
        {
            enemyMelee.transform.position = Vector2.MoveTowards(enemyMelee.transform.position, enemyMelee.endPoint.transform.position, enemyMelee.speed * Time.fixedDeltaTime);
            if (Vector3.Distance(enemyMelee.transform.position, enemyMelee.endPoint.position) < 0.5f) enemyMelee.Flip();
        }
        else if (!enemyMelee.facingRight)
        {
            enemyMelee.transform.position = Vector2.MoveTowards(enemyMelee.transform.position, enemyMelee.startPoint.transform.position, enemyMelee.speed * Time.fixedDeltaTime);
            if (Vector3.Distance(enemyMelee.transform.position, enemyMelee.startPoint.position) < 0.5f) enemyMelee.Flip();
        }

        if (Vector3.Distance(enemyMelee.transform.position, enemyMelee.target.transform.position) < 3f) stateMachine.ChangeState(enemyMelee.attackState);
    }
}
