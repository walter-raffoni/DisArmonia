using UnityEngine;

public class EnemyMelee : Enemy
{
    #region FSM
    public StateMachine stateMachine;
    public Enemy_PatrollingState patrollingState;
    public Melee_AttackState attackState;
    #endregion

    private void Awake()
    {
        stateMachine = new StateMachine();
        patrollingState = new Enemy_PatrollingState(this, stateMachine);
        attackState = new Melee_AttackState(this, stateMachine);

        stateMachine.Initialize(patrollingState);

        stanceMaggioreDanno = Random.Range(0, 2);

        var main = particleStance.main;
        if (stanceMaggioreDanno == 0) main.startColor = Color.green;
        else if (stanceMaggioreDanno == 1) main.startColor = Color.red;
    }

    private void Update()
    {
        if (!GameManager.instance.IsPaused) stateMachine.currentState.LogicUpdate();

        if (Vector3.Distance(target.transform.position, transform.position) < 5)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            if (target.transform.position.x > transform.position.x && !facingRight) Flip();
            if (target.transform.position.x < transform.position.x && facingRight) Flip();
        }

        if (currentHP <= 0)
        {
            target.GetComponent<Player>().stackDiSangue++;
            Destroy(gameObject);
        }
    }

    void FixedUpdate() => stateMachine.currentState.PhysicsUpdate();

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out Player player)) player.TakeDamage(puntiDanno);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out Player _)) stateMachine.ChangeState(attackState);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out Player _)) stateMachine.ChangeState(patrollingState);
    }
}