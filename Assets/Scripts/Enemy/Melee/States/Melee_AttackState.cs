using UnityEngine;

public class Melee_AttackState : State
{
    public Melee_AttackState(EnemyMelee _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
    {
        enemyMelee = _enemy;
        stateMachine = _stateMachine;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        enemyMelee.anim.SetTrigger("VistoNemico");

        if (Vector3.Distance(enemyMelee.transform.position, enemyMelee.target.transform.position) > 3f) stateMachine.ChangeState(enemyMelee.patrollingState);
    }
}
