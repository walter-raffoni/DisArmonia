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
    [SerializeField] int puntiDanno = 1;
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

    #region Campi pubblici
    public int StanceMaggioreDanno => stanceMaggioreDanno;
    public bool FacingRight => facingRight;
    public float Speed => speed;
    public float DistAttaccoPg => distAttaccoPg;
    public float DistPgTroppoVicino => distPgTroppoVicino;
    public float DistPgTroppoLontano => distPgTroppoLontano;
    public Animator Anim => anim;
    public Transform Target => target;
    public Transform EndPoint => endPoint;
    public Transform FirePoint => firePoint;
    public Transform StartPoint => startPoint;
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
        Room room = GetComponentInParent<Room>();

        anim = GetComponent<Animator>();

        stateMachine = new StateMachine();
        patrollingState = new Ranged_PatrollingState(this, stateMachine);
        attackState = new Ranged_AttackState(this, stateMachine);

        stateMachine.Initialize(patrollingState);

        currentHP = maxHP;

        stanceMaggioreDanno = Random.Range(0, 2);

        var main = particleStance.main;
        if (stanceMaggioreDanno == 0)
        {
            room.TotalAgile++;
            if ((room.TotalBrutale - room.TotalAgile) != 0)
            {
                stanceMaggioreDanno = 1;
                room.TotalAgile--;
                room.TotalBrutale++;
                main.startColor = coloreStanceBrutale;
            }
            main.startColor = coloreStanceAgile;
        }
        else if (stanceMaggioreDanno == 1)
        {
            room.TotalBrutale++;
            if ((room.TotalBrutale - room.TotalAgile) != 0)
            {
                stanceMaggioreDanno = 0;
                room.TotalAgile++;
                room.TotalBrutale--;
                main.startColor = coloreStanceAgile;
            }
            main.startColor = coloreStanceBrutale;
        }
    }

    private void Update()
    {
        stateMachine.currentState.LogicUpdate();

        if (currentHP <= 0)
        {
            Destroy(gameObject);
            target.GetComponent<Player>().CooldownDashAttuale = 0;
            target.GetComponent<Player>().JumpToConsume = true;
        }
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
}