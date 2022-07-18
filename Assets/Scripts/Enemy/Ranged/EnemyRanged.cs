using UnityEngine;

public class EnemyRanged : MonoBehaviour
{
    [Header("Health System")]
    [SerializeField] int maxHP = 10;
    [HideInInspector] public int currentHP;

    [Header("Movement System")]
    public float speed;
    public Transform startPoint;
    public Transform endPoint;
    public LayerMask layerOstacoli;
    public Vector3 offsetRilevazione;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public bool facingRight = true;

    [Header("Attack System")]
    public GameObject target;
    public float speedAttacco;
    public int puntiDanno = 2;
    public GameObject projectilePrefab;
    public GameObject firePoint;
    public float distanzaRilevamentoGiocatore = 5;

    [Header("Stance System")]
    public int stanceMaggioreDanno;
    public ParticleSystem particleStance;
    public Color coloreStanceAgile = Color.blue;
    public Color coloreStanceBrutale = Color.red;

    [Header("Animation System")]
    public Animator anim;

    #region FSM
    public StateMachine stateMachine;
    public Ranged_PatrollingState patrollingState;
    public Ranged_AttackState attackState;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHP = maxHP;

        stateMachine = new StateMachine();
        patrollingState = new Ranged_PatrollingState(this, stateMachine);
        attackState = new Ranged_AttackState(this, stateMachine);

        stateMachine.Initialize(patrollingState);

        stanceMaggioreDanno = Random.Range(0, 2);

        var main = particleStance.main;
        if (stanceMaggioreDanno == 0) main.startColor = coloreStanceAgile;
        else if (stanceMaggioreDanno == 1) main.startColor = coloreStanceBrutale;
    }

    private void Update()
    {
        if (!GameManager.instance.isPaused) stateMachine.currentState.LogicUpdate();

        if (currentHP <= 0) Destroy(gameObject);

        if (Vector3.Distance(target.transform.position, transform.position) < distanzaRilevamentoGiocatore)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            if (target.transform.position.x > transform.position.x && !facingRight) Flip();
            if (target.transform.position.x < transform.position.x && facingRight) Flip();
        }
    }

    void FixedUpdate() => stateMachine.currentState.PhysicsUpdate();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out Player _)) stateMachine.ChangeState(attackState);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out Player _)) stateMachine.ChangeState(patrollingState);
    }

    public void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    public void Spara() => Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation, transform);

    public void TakeDamage(int damageTaken)
    {
        if (currentHP <= 0) return;
        else currentHP -= damageTaken;
    }
}