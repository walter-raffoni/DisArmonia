using UnityEngine;

public class EnemyRanged : MonoBehaviour
{
    [Header("Health System")]
    [SerializeField] int maxHP = 10;
    [SerializeField] GameObject barraVita;

    [Header("Movement System")]
    [SerializeField] float speed;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    [Header("Attack System")]
    [SerializeField] Transform target;
    [SerializeField] Transform firePoint;
    [SerializeField] float distPgTroppoVicino = 3f;
    [SerializeField] float distPgTroppoLontano = 8f;
    [SerializeField] float distAttaccoPg = 5f;

    [Header("Projectile System")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float destroyTimeProiettile = 2;
    [SerializeField] float velocitaProiettile = 1;

    [Header("Stance System")]
    [SerializeField] ParticleSystem particleStance;
    [SerializeField] Color coloreStanceAgile = Color.blue;
    [SerializeField] Color coloreStanceBrutale = Color.red;

    #region Campi visibili ma non modificabili
    public float Speed => speed;
    public Animator Anim => anim;
    public Transform Target => target;
    public Transform EndPoint => endPoint;
    public bool FacingRight => facingRight;
    public Transform StartPoint => startPoint;
    public float DistAttaccoPg => distAttaccoPg;
    public float VelocitaProiettile => velocitaProiettile;
    public float DistPgTroppoVicino => distPgTroppoVicino;
    public float DistPgTroppoLontano => distPgTroppoLontano;
    public float DestroyTimeProiettile => destroyTimeProiettile;
    #endregion

    #region Campi privati
    private Animator anim;
    private bool facingRight = true;
    private int currentHP, stanceMaggioreDanno;
    #endregion

    #region FSM
    public StateMachine stateMachine;
    public Ranged_PatrollingState patrollingState;
    public Ranged_AttackState attackState;
    #endregion

    private void Awake()
    {
        anim = GetComponent<Animator>();

        stateMachine = new StateMachine();
        patrollingState = new Ranged_PatrollingState(this, stateMachine);
        attackState = new Ranged_AttackState(this, stateMachine);

        stateMachine.Initialize(patrollingState);

        currentHP = maxHP;

        stanceMaggioreDanno = Random.Range(0, 2);

        var main = particleStance.main;
        if (stanceMaggioreDanno == 0) main.startColor = coloreStanceAgile;
        else if (stanceMaggioreDanno == 1) main.startColor = coloreStanceBrutale;
    }

    private void Update()
    {
        stateMachine.currentState.LogicUpdate();

        if (currentHP <= 0) Destroy(gameObject);
    }

    void FixedUpdate() => stateMachine.currentState.PhysicsUpdate();

    public void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    public void TakeDamage(int damageTaken)
    {
        currentHP -= damageTaken;
        float barValue = (float)currentHP / maxHP;
        barraVita.gameObject.transform.localScale = new Vector3(barValue, 0.2f, 1);
    }

    public void Spara() => Instantiate(projectilePrefab, firePoint.position, firePoint.rotation, firePoint);
}