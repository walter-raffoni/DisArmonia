using UnityEngine;

public class Melee_AttackState : State
{
    public Melee_AttackState(EnemyMelee _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
    {
        enemyMelee = _enemy;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        enemyMelee.Anim.Play("MeleeAttackAnimation");
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (Vector3.Distance(enemyMelee.transform.position, enemyMelee.Target.position) > enemyMelee.AttaccoGiocatoreDist) stateMachine.ChangeState(enemyMelee.followingState);
        else if (Vector3.Distance(enemyMelee.transform.position, enemyMelee.Target.position) > enemyMelee.GiocatoreRilevatoDist) stateMachine.ChangeState(enemyMelee.patrollingState);
    }
}
