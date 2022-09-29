using UnityEngine;

public class AirborneState : State
{
    public AirborneState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        if (player.JumpToConsume)
        {
            player.Speed.y = player.JumpHeight;
            player.EndedJumpEarly = false;
            player.JumpToConsume = false;
            player.TimeLeftGrounded = player.FixedFrame;
            player.DidBufferedJump = true;
            if (player.IsGrounded) player.JumpEffect();//Fa riprodurre il particellare solo quando è a terra
        }

        player.MinFallSpeed = 80;
        player.MaxFallSpeed = 160;

        player.ComboAttacco = 0;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        player.HandleInput();

        if (player.DashToConsume && player.CanDash && player.Input.X != 0 && player.Stance == Player.TipoStance.Agile && player.CooldownDashAttuale <= 0 && !player.EnemyTouched) player.stateMachine.ChangeState(player.dashingState);//impedisce che dashi non appena salti dopo aver toccato il nemico e dashato

        if (player.Input.AttackDown && player.Stance == Player.TipoStance.Brutale) stateMachine.ChangeState(player.verticalAttackState);

        if (player.Input.X != 0) player.transform.localScale = new Vector3(player.Input.X > 0 ? 1 : -1, 1, 1);//Ribalta lo sprite in orizzontale, è in questo controllo per evitare che si ribalti durante la pausa
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.BaseMovement();

        player.CollisionsChecks();

        player.HorizontalMovement();
        player.IsTopPoint();
        player.Gravity();
        player.Jump();
        player.Move();

        if (player.Stance == Player.TipoStance.Brutale && stateMachine.currentState != player.verticalAttackState)
        {
            if (player.Velocity.y >= 0) player.Anim.Play("Brutale_Jump");
            else if (player.Velocity.y < 0) player.Anim.Play("Brutale_AirborneAtterra");
        }
        else if (player.Stance == Player.TipoStance.Agile && stateMachine.currentState != player.verticalAttackState)
        {
            if (player.Velocity.y >= 0) player.Anim.Play("Agile_Jump");
            else if (player.Velocity.y < 0) player.Anim.Play("Agile_AirborneAtterra");
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.JumpToConsume = false;
    }
}