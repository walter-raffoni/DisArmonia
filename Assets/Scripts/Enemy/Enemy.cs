using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health System")]
    [SerializeField] int maxHP = 10;
    [SerializeField] GameObject barraVita;//SpriteRenderer

    [Header("Movement System")]
    [SerializeField] float speed;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    [Header("Attack System")]
    public TipoNemico tipoNemico;
    public enum TipoNemico { Melee, Ranged }
    [SerializeField] Transform target;
    [SerializeField] int puntiDanno = 2;
    [SerializeField] Transform firePoint;//ranged
    [SerializeField] float giocatoreRilevatoDist = 5;//melee
    [SerializeField] float attaccoGiocatoreDist = 1.5f;//melee
    [SerializeField] float distPgTroppoVicino = 3f;//ranged
    [SerializeField] float distPgTroppoLontano = 8f;//ranged
    [SerializeField] float distAttaccoPg = 5f;//ranged
    [SerializeField] LayerMask playerMask;
    [SerializeField] Vector3 offsetRight;
    [SerializeField] Vector3 offsetLeft;

    [Header("Projectile System")]
    [SerializeField] GameObject projectilePrefab;//ranged
    [SerializeField] float destroyTimeProiettile = 2;//ranged
    [SerializeField] float velocitaProiettile = 1;//ranged

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
    public Transform StartPoint => startPoint;

    public float AttaccoGiocatoreDist => attaccoGiocatoreDist;

    public float GiocatoreRilevatoDist => giocatoreRilevatoDist;

    public bool AttackEnded
    {
        get { return attackEnded; }
        set { attackEnded = value; }
    }
    #endregion

    #region Campi privati
    private Animator anim;
    private bool attackEnded;
    private bool facingRight = true;
    private int currentHP, stanceMaggioreDanno;
    #endregion

    #region FSM
    public StateMachine stateMachine;

    public Melee_PatrollingState patrollingStateMelee;
    public Melee_FollowingState followingStateMelee;
    public Melee_AttackState attackStateMelee;

    public Ranged_PatrollingState patrollingStateRanged;
    public Ranged_AttackState attackStateRanged;
    #endregion

    private void Awake()
    {
        Room room = GetComponentInParent<Room>();

        anim = GetComponent<Animator>();

        stateMachine = new StateMachine();

        patrollingStateMelee = new Melee_PatrollingState(this, stateMachine);
        followingStateMelee = new Melee_FollowingState(this, stateMachine);
        attackStateMelee = new Melee_AttackState(this, stateMachine);

        patrollingStateRanged = new Ranged_PatrollingState(this, stateMachine);
        attackStateRanged = new Ranged_AttackState(this, stateMachine);

        if (tipoNemico == TipoNemico.Melee) stateMachine.Initialize(patrollingStateMelee);

        if (tipoNemico == TipoNemico.Ranged) stateMachine.Initialize(patrollingStateRanged);

        currentHP = maxHP;

        stanceMaggioreDanno = Random.Range(0, 2);//0: agile, 1: brutale

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

    private void Update() => stateMachine.currentState.LogicUpdate();

    private void FixedUpdate() => stateMachine.currentState.PhysicsUpdate();

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

        if (currentHP <= 0)
        {
            Destroy(gameObject);
            target.GetComponent<Player>().CooldownDashAttuale = 0;
            target.GetComponent<Player>().JumpToConsume = true;
        }
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

    public void DealDamage()
    {
        Vector3 offsetTemp = offsetRight;

        if (facingRight) offsetTemp = offsetRight;
        else offsetTemp = offsetLeft;

        Collider2D[] playerHit = Physics2D.OverlapCircleAll(transform.position + offsetTemp, .5f, playerMask);

        foreach (Collider2D player in playerHit) player.GetComponentInParent<Player>().TakeDamage(puntiDanno);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 offsetTemp = offsetRight;

        if (facingRight) offsetTemp = offsetRight;
        else offsetTemp = offsetLeft;

        Gizmos.DrawWireSphere(transform.position + offsetTemp, .5f);
    }

    public void EndAttack() => attackEnded = true;
}
