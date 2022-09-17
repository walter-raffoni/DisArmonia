using UnityEngine;

public class Melee_AttackState : State
{
    public Melee_AttackState(Enemy _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.Anim.Play("Melee_Attack");
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if ((Vector3.Distance(enemy.transform.position, enemy.Target.position) > enemy.AttaccoGiocatoreDist) && enemy.AttackEnded) stateMachine.ChangeState(enemy.followingStateMelee);
        else if ((Vector3.Distance(enemy.transform.position, enemy.Target.position) > enemy.GiocatoreRilevatoDist) && enemy.AttackEnded) stateMachine.ChangeState(enemy.patrollingStateMelee);
    }
}
