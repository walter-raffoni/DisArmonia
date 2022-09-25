using UnityEngine;

public class EnemyMelee : Enemy
{
    [Header("Attack System")]
    [SerializeField] float giocatoreRilevatoDist = 5;//melee
    [SerializeField] float attaccoGiocatoreDist = 1.5f;//melee
    [SerializeField] Vector3 offsetRight = new Vector3(0.65f, -0.4f, 0);
    [SerializeField] Vector3 offsetLeft = new Vector3(-0.65f, -0.4f, 0);

    #region Campi pubblici
    public float AttaccoGiocatoreDist => attaccoGiocatoreDist;
    public float GiocatoreRilevatoDist => giocatoreRilevatoDist;
    public bool AttackEnded
    {
        get { return attackEnded; }
        set { attackEnded = value; }
    }
    #endregion

    #region Campi privati
    private bool attackEnded;
    #endregion

    #region FSM
    public StateMachine stateMachine;

    public Melee_PatrollingState patrollingState;
    public Melee_FollowingState followingState;
    public Melee_AttackState attackState;
    public Enemy_DamagedState damagedState;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new StateMachine();

        patrollingState = new Melee_PatrollingState(this, stateMachine);
        followingState = new Melee_FollowingState(this, stateMachine);
        attackState = new Melee_AttackState(this, stateMachine);
        damagedState = new Enemy_DamagedState(this, stateMachine);

        stateMachine.Initialize(patrollingState);
    }

    private void Update() => stateMachine.currentState.LogicUpdate();

    private void FixedUpdate() => stateMachine.currentState.PhysicsUpdate();

    public void DealDamage()
    {
        Vector3 offsetTemp = offsetRight;

        if (facingRight) offsetTemp = offsetRight;
        else offsetTemp = offsetLeft;

        Collider2D[] playerHit = Physics2D.OverlapCircleAll(transform.position + offsetTemp, .5f, playerMask);

        foreach (Collider2D player in playerHit)
        {
            int direction = transform.position.x > player.transform.position.x ? -1 : 1;
            player.GetComponentInParent<Player>().TakeDamage(puntiDanno, direction);
        }
    }

    public void EndAttack() => attackEnded = true;

    private void OnDrawGizmosSelected()
    {
        Vector3 offsetTemp = offsetRight;

        if (facingRight) offsetTemp = offsetRight;
        else offsetTemp = offsetLeft;

        Gizmos.DrawWireSphere(transform.position + offsetTemp, .5f);
    }

    public void ManageStun()
    {
        if (stateMachine.currentState == patrollingState)
        {
            stateMachine.ChangeState(damagedState);
            Anim.Play("Melee_Damaged");
        }
        else
        {
            return;
        }
    }
}
