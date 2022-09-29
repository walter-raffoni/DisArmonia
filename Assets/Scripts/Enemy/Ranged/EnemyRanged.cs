using UnityEngine;

public class EnemyRanged : Enemy
{
    [Header("Projectile System")]
    [SerializeField] GameObject projectilePrefab;//ranged
    [SerializeField] float destroyTimeProiettile = 0.5f;//ranged
    [SerializeField] float velocitaProiettile = 15;//ranged

    [Header("Attack System")]
    [SerializeField] Transform firePoint;//ranged
    [SerializeField] float distPgTroppoVicino = 3f;//ranged
    [SerializeField] float distPgTroppoLontano = 8f;//ranged
    [SerializeField] float distAttaccoPg = 5f;//ranged

    #region Campi pubblici
    public float DistAttaccoPg => distAttaccoPg;
    public float DistPgTroppoVicino => distPgTroppoVicino;
    public float DistPgTroppoLontano => distPgTroppoLontano;
    #endregion

    #region Campi privati
    private Rigidbody2D rb;
    #endregion

    #region FSM
    public StateMachine stateMachine;

    public Ranged_PatrollingState patrollingState;
    public Ranged_AttackState attackState;
    public Enemy_DamagedState damagedState;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();

        patrollingState = new Ranged_PatrollingState(this, stateMachine);
        attackState = new Ranged_AttackState(this, stateMachine);
        damagedState = new Enemy_DamagedState(this, stateMachine);

        stateMachine.Initialize(patrollingState);
    }

    private void Update() => stateMachine.currentState.LogicUpdate();

    private void FixedUpdate() => stateMachine.currentState.PhysicsUpdate();

    public void Spara()
    {
        GameObject proiettileIstanziato = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        if (proiettileIstanziato.TryGetComponent(out Projectile projectile))
        {
            projectile.VelocitaProiettile = velocitaProiettile;
            projectile.PuntiDanno = puntiDanno;
            projectile.DestroyTimeProiettile = destroyTimeProiettile;
            projectile.Target = target;
        }
    }

    public void ManageStun()
    {
        stateMachine.ChangeState(damagedState);
        Anim.Play("Ranged_Damaged");
        rb.velocity = Vector2.zero;
    }
}