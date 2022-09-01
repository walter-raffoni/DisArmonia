using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    [Header("Health System")]
    [SerializeField] int maxHP = 10;
    [SerializeField] SpriteRenderer barraVita;

    [Header("Movement System")]
    [SerializeField] float speed;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    [Header("Attack System")]
    [SerializeField] Transform target;
    [SerializeField] int puntiDanno = 2;
    [SerializeField] float giocatoreRilevatoDist = 5;
    [SerializeField] float attaccoGiocatoreDist = 1.5f;

    [Header("Stance System")]
    [SerializeField] ParticleSystem particleStance;
    [SerializeField] Color coloreStanceAgile = Color.blue;
    [SerializeField] Color coloreStanceBrutale = Color.red;

    #region Campi pubblici
    public float Speed => speed;
    public float AttaccoGiocatoreDist => attaccoGiocatoreDist;
    public float GiocatoreRilevatoDist => giocatoreRilevatoDist;
    public Animator Anim => anim;
    public Transform Target => target;
    public Transform EndPoint => endPoint;
    public Transform StartPoint => startPoint;
    public bool FacingRight => facingRight;
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
    public Melee_PatrollingState patrollingState;
    public Melee_FollowingState followingState;
    public Melee_AttackState attackState;
    #endregion

    private void Awake()
    {
        anim = GetComponent<Animator>();

        stateMachine = new StateMachine();
        patrollingState = new Melee_PatrollingState(this, stateMachine);
        followingState = new Melee_FollowingState(this, stateMachine);
        attackState = new Melee_AttackState(this, stateMachine);

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

        if (currentHP <= 0)
        {
            Destroy(gameObject);
            target.GetComponent<Player>().CooldownDashAttuale = 0;
            target.GetComponent<Player>().JumpToConsume = true;
        }
    }

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
    }

    public void DealDamage()
    {
        if (Vector3.Distance(transform.position, target.transform.position) < attaccoGiocatoreDist)
        {
            if (target.TryGetComponent(out Player player) && !player.IsInvulnerable) player.TakeDamage(puntiDanno);
        }
    }

    public void OnAttackEnd() => attackEnded = true;
}