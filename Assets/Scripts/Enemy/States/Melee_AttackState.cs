using UnityEngine;

public class Melee_AttackState : State
{
    public Melee_AttackState(Enemy _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        enemy.anim.SetTrigger("VistoNemico");

        enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.target.transform.position, enemy.speedAttacco * Time.fixedDeltaTime);
    }
}
