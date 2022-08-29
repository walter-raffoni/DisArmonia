using UnityEngine;

public class Melee_FollowingState : State
{
    public Melee_FollowingState(EnemyMelee _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
    {
        enemyMelee = _enemy;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        enemyMelee.Anim.Play("Melee_Idle");

        enemyMelee.AttackEnded = false;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (enemyMelee.Target.position.x > enemyMelee.transform.position.x && !enemyMelee.FacingRight) enemyMelee.Flip();
        if (enemyMelee.Target.position.x < enemyMelee.transform.position.x && enemyMelee.FacingRight) enemyMelee.Flip();

        enemyMelee.transform.position = Vector2.MoveTowards(enemyMelee.transform.position, enemyMelee.Target.position, enemyMelee.Speed * Time.fixedDeltaTime);

        if (Vector3.Distance(enemyMelee.transform.position, enemyMelee.Target.position) <= enemyMelee.AttaccoGiocatoreDist) stateMachine.ChangeState(enemyMelee.attackState);

        else if (Vector3.Distance(enemyMelee.transform.position, enemyMelee.Target.position) > enemyMelee.GiocatoreRilevatoDist) stateMachine.ChangeState(enemyMelee.patrollingState);
    }
}
