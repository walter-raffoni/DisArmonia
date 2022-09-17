using UnityEngine;

public class Ranged_PatrollingState : State
{
    public Ranged_PatrollingState(Enemy _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.Anim.Play("Ranged_IdleAndAttack");

        enemy.CancelInvoke("Spara");//sennò continua a sparare
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

        if (Vector3.Distance(enemy.transform.position, enemy.Target.position) <= enemy.DistAttaccoPg) stateMachine.ChangeState(enemy.attackStateRanged);
    }
}
