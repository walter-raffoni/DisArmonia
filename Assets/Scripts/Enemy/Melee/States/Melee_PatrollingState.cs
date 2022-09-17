using UnityEngine;

public class Melee_PatrollingState : State
{
    public Melee_PatrollingState(Enemy _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
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

        if (enemy.FacingRight)
        {
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.EndPoint.position, enemy.Speed * Time.fixedDeltaTime);
            if (Vector3.Distance(enemy.transform.position, enemy.EndPoint.position) < 0.5f) enemy.Flip();
        }
        else if (!enemy.FacingRight)
        {
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.StartPoint.position, enemy.Speed * Time.fixedDeltaTime);
            if (Vector3.Distance(enemy.transform.position, enemy.StartPoint.position) < 0.5f) enemy.Flip();
        }

        if (Vector3.Distance(enemy.transform.position, enemy.Target.position) <= enemy.GiocatoreRilevatoDist) stateMachine.ChangeState(enemy.followingStateMelee);
        else if (Vector3.Distance(enemy.transform.position, enemy.Target.position) <= enemy.AttaccoGiocatoreDist) stateMachine.ChangeState(enemy.attackStateMelee);
    }
}
