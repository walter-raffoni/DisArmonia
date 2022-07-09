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

        enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.target.transform.position, enemy.speedAttacco * Time.fixedDeltaTime);
    }
}
