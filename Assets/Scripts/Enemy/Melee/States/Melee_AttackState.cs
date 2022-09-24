using UnityEngine;

public class Melee_AttackState : State
{
    public Melee_AttackState(EnemyMelee _enemyMelee, StateMachine _stateMachine) : base(_enemyMelee, _stateMachine)
    {
        enemyMelee = _enemyMelee;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        enemyMelee.Anim.Play("Melee_Attack");
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if ((Vector3.Distance(enemyMelee.transform.position, enemyMelee.Target.position) > enemyMelee.AttaccoGiocatoreDist) && enemyMelee.AttackEnded) stateMachine.ChangeState(enemyMelee.followingState);
        else if ((Vector3.Distance(enemyMelee.transform.position, enemyMelee.Target.position) > enemyMelee.GiocatoreRilevatoDist) && enemyMelee.AttackEnded) stateMachine.ChangeState(enemyMelee.patrollingState);
    }
}
