using UnityEngine;

public class VerticalAttackState : State
{
    public VerticalAttackState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        player.HandleInput();
    }

    public override void LogicUpdate()
    {
        //TODO: Far sì che cada più veloce
        base.LogicUpdate();

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.attackPoint.position, player.attackRange, player.enemyLayers);//rileva nemici nel raggio dell'attacco

        //danneggia i nemici
        foreach (Collider2D enemy in hitEnemies) enemy.gameObject.GetComponent<Enemy>().TakeDamage(player.dannoAlNemico);

        player.stateMachine.ChangeState(player.standingState);
    }
}