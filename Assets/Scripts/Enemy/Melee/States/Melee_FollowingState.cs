using UnityEngine;

public class Melee_FollowingState : State
{
    public Melee_FollowingState(EnemyMelee _enemy, StateMachine _stateMachine) : base(_enemy, _stateMachine)
    {
        enemyMelee = _enemy;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        enemyMelee.Anim.Play("MeleeIdleAnimation");
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (enemyMelee.target.transform.position.x > enemyMelee.transform.position.x && !enemyMelee.FacingRight) enemyMelee.Flip();
        if (enemyMelee.target.transform.position.x < enemyMelee.transform.position.x && enemyMelee.FacingRight) enemyMelee.Flip();

        enemyMelee.transform.position = Vector2.MoveTowards(enemyMelee.transform.position, enemyMelee.target.transform.position, enemyMelee.speed * Time.fixedDeltaTime);

        if (Vector3.Distance(enemyMelee.transform.position, enemyMelee.target.transform.position) <= enemyMelee.distanzaAttaccoGiocatore) stateMachine.ChangeState(enemyMelee.attackState);

        else if (Vector3.Distance(enemyMelee.transform.position, enemyMelee.target.transform.position) > enemyMelee.distanzaRilevamentoGiocatore) stateMachine.ChangeState(enemyMelee.patrollingState);
    }
}
