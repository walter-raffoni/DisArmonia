using UnityEngine;

public class Enemy_PatrollingState : State
{
    public Enemy_PatrollingState(Enemy _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.anim.ResetTrigger("VistoNemico");
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (enemy.facingRight)
        {
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.endPoint.transform.position, enemy.speed * Time.fixedDeltaTime);
            if (Vector3.Distance(enemy.transform.position, enemy.endPoint.position) < 0.5f) enemy.Flip();
        }
        else if (!enemy.facingRight)
        {
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.startPoint.transform.position, enemy.speed * Time.fixedDeltaTime);
            if (Vector3.Distance(enemy.transform.position, enemy.startPoint.position) < 0.5f) enemy.Flip();
        }
    }
}
