using UnityEngine;

public class DamagedState : State
{
    private float stunTimer;

    public DamagedState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        stunTimer = player.StunDuration;
    }

    public override void LogicUpdate()
    {
        if (stunTimer > 0) stunTimer -= Time.deltaTime;
        else stateMachine.ChangeState(player.standingState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.Gravity();
    }

    public override void Exit()
    {
        base.Exit();

        if (player.Stance == Player.TipoStance.Brutale) player.Anim.Play("Brutale_Idle");
        player.Anim.Play("Agile_Idle");

        player.rb.velocity = Vector2.zero;
    }
}