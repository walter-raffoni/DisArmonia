using UnityEngine;

public class Enemy_DamagedState : State
{
    public float stunTimer = 1.1f;

    public Enemy_DamagedState(EnemyMelee _enemyMelee, StateMachine _stateMachine) : base(_enemyMelee, _stateMachine)
    {
        enemyMelee = _enemyMelee;
        stateMachine = _stateMachine;
    }

    public Enemy_DamagedState(EnemyRanged _enemyRanged, StateMachine _stateMachine) : base(_enemyRanged, _stateMachine)
    {
        enemyRanged = _enemyRanged;
        stateMachine = _stateMachine;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (stunTimer > 0) stunTimer -= Time.deltaTime;
        else
        {
            if (enemyMelee != null)
            {
                stateMachine.ChangeState(enemyMelee.patrollingState);
                enemyMelee.Anim.Play("Melee_Idle");
            }
            else
            {
                stateMachine.ChangeState(enemyRanged.patrollingState);
                enemyRanged.Anim.Play("Ranged_Idle");
            }
        }
    }
}
