using UnityEngine;

public class EnemyRanged : MonoBehaviour
{
    [Header("Health System")]
    [SerializeField] int maxHP = 10;
    private int currentHP;

    [Header("Movement System")]
    public float speed;
    public Transform startPoint;
    public Transform endPoint;
    public LayerMask layerOstacoli;
    private bool facingRight = true;
    public bool FacingRight => facingRight;

    [Header("Attack System")]
    public GameObject target;
    public int puntiDanno = 2;
    public GameObject projectilePrefab;
    public GameObject firePoint;
    public float distPgTroppoVicino = 1.5f;
    public float distPgTroppoLontano = 5f;
    public float distAttaccoPg = 3f;

    [Header("Stance System")]
    public ParticleSystem particleStance;
    public Color coloreStanceAgile = Color.blue;
    public Color coloreStanceBrutale = Color.red;
    private int stanceMaggioreDanno;

    #region Campi privati
    private Animator anim;
    public Animator Anim => anim;
    #endregion

    #region FSM
    public StateMachine stateMachine;
    public Ranged_PatrollingState patrollingState;
    public Ranged_AttackState attackState;
    #endregion

    private void Awake()
    {
        anim = GetComponent<Animator>();

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

    private void Update() => stateMachine.currentState.LogicUpdate();

    void FixedUpdate() => stateMachine.currentState.PhysicsUpdate();

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
        if (currentHP <= 0) Destroy(gameObject);
        else currentHP -= damageTaken;
    }
}