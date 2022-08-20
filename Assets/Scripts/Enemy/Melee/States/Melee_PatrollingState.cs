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

        enemyMelee.Anim.Play("Melee_Idle");
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (enemyMelee.FacingRight)
        {
            enemyMelee.transform.position = Vector2.MoveTowards(enemyMelee.transform.position, enemyMelee.EndPoint.position, enemyMelee.Speed * Time.fixedDeltaTime);
            if (Vector3.Distance(enemyMelee.transform.position, enemyMelee.EndPoint.position) < 0.5f) enemyMelee.Flip();
        }
        else if (!enemyMelee.FacingRight)
        {
            enemyMelee.transform.position = Vector2.MoveTowards(enemyMelee.transform.position, enemyMelee.StartPoint.position, enemyMelee.Speed * Time.fixedDeltaTime);
            if (Vector3.Distance(enemyMelee.transform.position, enemyMelee.StartPoint.position) < 0.5f) enemyMelee.Flip();
        }

        if (Vector3.Distance(enemyMelee.transform.position, enemyMelee.Target.position) <= enemyMelee.GiocatoreRilevatoDist) stateMachine.ChangeState(enemyMelee.followingState);
        else if (Vector3.Distance(enemyMelee.transform.position, enemyMelee.Target.position) <= enemyMelee.AttaccoGiocatoreDist) stateMachine.ChangeState(enemyMelee.attackState);
    }
}
