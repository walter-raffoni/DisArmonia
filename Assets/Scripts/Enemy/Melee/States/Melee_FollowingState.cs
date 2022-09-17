using UnityEngine;

public class Melee_FollowingState : State
{
    public Melee_FollowingState(Enemy _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.Anim.Play("Melee_Idle");

        enemy.AttackEnded = false;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (enemy.Target.position.x > enemy.transform.position.x && !enemy.FacingRight) enemy.Flip();
        if (enemy.Target.position.x < enemy.transform.position.x && enemy.FacingRight) enemy.Flip();

        enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.Target.position, enemy.Speed * Time.fixedDeltaTime);

        if (Vector3.Distance(enemy.transform.position, enemy.Target.position) <= enemy.AttaccoGiocatoreDist) stateMachine.ChangeState(enemy.attackStateMelee);

        else if (Vector3.Distance(enemy.transform.position, enemy.Target.position) > enemy.GiocatoreRilevatoDist) stateMachine.ChangeState(enemy.patrollingStateMelee);
    }
}
