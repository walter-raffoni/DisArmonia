using UnityEngine;

public class Ranged_AttackState : State
{
    public Ranged_AttackState(Enemy _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.InvokeRepeating("Spara", 0, 1);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (enemy.Target.position.x > enemy.transform.position.x && !enemy.FacingRight) enemy.Flip();
        if (enemy.Target.position.x < enemy.transform.position.x && enemy.FacingRight) enemy.Flip();

        if (Vector3.Distance(enemy.transform.position, enemy.Target.position) >= enemy.DistPgTroppoLontano) stateMachine.ChangeState(enemy.patrollingStateRanged);
        else if (Vector3.Distance(enemy.transform.position, enemy.Target.position) <= enemy.DistPgTroppoVicino)
        {
            if (enemy.FacingRight) enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.transform.position + Vector3.left, enemy.Speed * Time.fixedDeltaTime);
            else if (!enemy.FacingRight) enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.transform.position + Vector3.right, enemy.Speed * Time.fixedDeltaTime);
        }
    }
}