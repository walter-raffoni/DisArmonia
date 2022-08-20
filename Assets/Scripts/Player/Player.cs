using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [Header("Health System")]
    [SerializeField] int maxHP = 10;

    [Header("Movement System")]
    [SerializeField] float slowingValue = 60;
    [SerializeField] float moveClamp = 13;
    [SerializeField] float speedingValue = 120;
    [SerializeField] float forceDecay = 1;//Effectors

    [Header("Gravity System")]
    [SerializeField] float fallClamp = -60;
    [SerializeField] float minFallSpeed = 80;
    [SerializeField] float maxFallSpeed = 160;
    [SerializeField, Range(0, -10)] float gravityPower = -1.5f;

    [Header("Jump System")]
    [SerializeField] float jumpTopBonus = 100;
    [SerializeField] int jumpBuffer = 7;
    [SerializeField] float jumpHeight = 35;
    [SerializeField] float jumpTopLimit = 40;
    [SerializeField] int coyoteTimeLimit = 7;
    [SerializeField] float gravityModifierJumpEndedEarly = 3;

    [Header("Collision System")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float rayLengthDetection = 0.1f;

    [Header("Dash System")]
    [SerializeField] float cooldownDash = 2;
    [SerializeField] int dashLength = 6;
    [SerializeField] float dashPower = 30;
    [SerializeField] float horizontalMultiplierDashEnd = 0.25f;

    [Header("Attack System")]
    [SerializeField] Transform attackPoint;
    [SerializeField] Transform verticalAttackPoint;
    [SerializeField] float attackRangeAgile = 0.5f;
    [SerializeField] float attackRangeBrutale = 0.42f;
    [SerializeField] float verticalAttackRange = 0.5f;
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] int dannoAlNemico = 2;
    [SerializeField] float tempoAttaccoOrizAgile = 0.75f;
    [SerializeField] float tempoAttaccoOrizBrutale = 0.75f;
    [SerializeField] float tempoAttaccoBrutale = 0.75f;
    [SerializeField] float moltiplicatoreRimbalzoAttaccoVerticale = 50;
    [SerializeField] float probabilitaCritico = 25;

    [Header("Stance System")]
    [SerializeField] float cooldownStance = 1;
    [SerializeField] CapsuleCollider2D colliderAgile;
    [SerializeField] CapsuleCollider2D colliderBrutale;

    [Header("Particle System")]
    [SerializeField] ParticleSystem moveParticles;
    [SerializeField] ParticleSystem dashParticles;
    [SerializeField] ParticleSystem dashRingParticles;
    [SerializeField] ParticleSystem launchParticles;
    [SerializeField] ParticleSystem jumpParticles;
    [SerializeField] ParticleSystem doubleJumpParticles;
    [SerializeField] ParticleSystem landParticles;
    [SerializeField] float maxFallSpeedParticles = -40;
    [SerializeField] Transform dashRingTransform;

    [Header("Audio System")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] footsteps;
    [SerializeField] AudioClip dashClip;
    [SerializeField] AudioClip[] slideClips;
    [SerializeField] AudioClip doubleJumpClip;

    #region Campi misti
    [HideInInspector] internal Vector2 DashVelocity, LastPos, ForceBuildup, LastPosition, Velocity, Speed;

    public FrameInput Input { get; private set; }
    public Vector2 RawMovement { get; private set; }
    #endregion  

    #region Campi pubblici
    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;
    public int FixedFrame => fixedFrame;
    public int DashLength => dashLength;
    public int ComboAttacco
    {
        get { return comboAttacco; }
        set { comboAttacco = value; }
    }
    public int StackDiSangue
    {
        get { return stackDiSangue; }
        set { stackDiSangue = value; }
    }

    public Animator Anim => anim;

    public bool IsGrounded => isGrounded;
    public bool DashAbility => dashAbility;
    public bool HasHitUp => hasHitUp;
    public bool DoubleJumpAbility => doubleJumpAbility;
    public bool DashFacingRight => dashFacingRight;
    public bool CanDoubleJump => canUseDoubleJump && !canUseCoyotePriv;
    public bool CanUseCoyote => canUseCoyotePriv && !isGrounded && timeLeftGrounded + coyoteTimeLimit > fixedFrame;
    public bool HasBufferedJump => ((isGrounded && !didBufferedJump) || isStuckInCorner) && lastJumpPressed + jumpBuffer > fixedFrame;
    public bool DidBufferedJump
    {
        get { return didBufferedJump; }
        set { didBufferedJump = value; }
    }
    public bool CanDash
    {
        get { return canDash; }
        set { canDash = value; }
    }
    public bool CanUseCoyotePriv
    {
        get { return canUseCoyotePriv; }
        set { canUseCoyotePriv = value; }
    }
    public bool CanUseDoubleJump
    {
        get { return canUseDoubleJump; }
        set { canUseDoubleJump = value; }
    }
    public bool JumpToConsume
    {
        get { return jumpToConsume; }
        set { jumpToConsume = value; }
    }
    public bool DashToConsume
    {
        get { return dashToConsume; }
        set { dashToConsume = value; }
    }
    public bool EndedJumpEarly
    {
        get { return endedJumpEarly; }
        set { endedJumpEarly = value; }
    }

    public float CooldownStanceAttuale => cooldownStanceAttuale;
    public float CooldownDash => cooldownDash;
    public float DashPower => dashPower;
    public float JumpHeight => jumpHeight;
    public float JumpTopLimit => jumpTopLimit;
    public float MinFallSpeed => minFallSpeed;
    public float MaxFallSpeed => maxFallSpeed;
    public float TempoAttaccoBrutale => tempoAttaccoBrutale;
    public float TempoAttaccoOrizAgile => tempoAttaccoOrizAgile;
    public float MaxFallSpeedParticles => maxFallSpeedParticles;
    public float TempoAttaccoOrizBrutale => tempoAttaccoOrizBrutale;
    public float HorizontalMultiplierDashEnd => horizontalMultiplierDashEnd;
    public float TimeLeftGrounded
    {
        get { return timeLeftGrounded; }
        set { timeLeftGrounded = value; }
    }
    public float FallSpeed
    {
        get { return fallSpeed; }
        set { fallSpeed = value; }
    }
    public float HasStartedDashing
    {
        get { return hasStartedDashing; }
        set { hasStartedDashing = value; }
    }
    public float TopPoint
    {
        get { return topPoint; }
        set { topPoint = value; }
    }
    public float TempoAttaccoAttuale
    {
        get { return tempoAttaccoAttuale; }
        set { tempoAttaccoAttuale = value; }
    }
    public float CooldownDashAttuale
    {
        get { return cooldownDashAttuale; }
        set { cooldownDashAttuale = value; }
    }

    public TipoStance Stance => stance;
    public enum TipoStance { Agile, Brutale }
    public enum PlayerForce { Burst, Decay } //Burst: Aggiunta direttamente alla velocità di movimento del pg, da controllare con la decelerazione standard | Decay: Forza additiva gestita dal sistema di decadimento

    public ParticleSystem MoveParticles => moveParticles;
    public ParticleSystem DashParticles => dashParticles;
    public ParticleSystem DashRingParticles => dashRingParticles;
    public ParticleSystem LaunchParticles => launchParticles;
    public ParticleSystem JumpParticles => jumpParticles;
    public ParticleSystem DoubleJumpParticles => doubleJumpParticles;
    public ParticleSystem LandParticles => landParticles;

    public AudioSource AudioSource => audioSource;

    public AudioClip DashClip => dashClip;
    public AudioClip DoubleJumpClip => doubleJumpClip;
    public AudioClip[] Footsteps => footsteps;

    public Transform DashRingTransform => dashRingTransform;

    public CapsuleCollider2D Coll => coll;
    #endregion

    #region Campi privati
    private Animator anim;

    private Rigidbody2D rb;

    private TipoStance stance;

    private CapsuleCollider2D coll;

    private ExtendedInputActions input;

    private int currentHP, comboAttacco, stackDiSangue, fixedFrame;

    private bool doubleJumpAbility, dashAbility, isGrounded, isStuckInCorner, didBufferedJump, jumpToConsume, dashToConsume, hasHitUp, hasHitRight, hasHitLeft, canUseCoyotePriv, canUseDoubleJump, canDash, dashFacingRight;
    private bool endedJumpEarly = true;

    private RaycastHit2D[] hitsUp = new RaycastHit2D[1];
    private RaycastHit2D[] hitsDown = new RaycastHit2D[3];
    private RaycastHit2D[] hitsLeft = new RaycastHit2D[1];
    private RaycastHit2D[] hitsRight = new RaycastHit2D[1];

    private float tempoAttaccoAttuale, cooldownStanceAttuale, timeLeftGrounded, frameClamp, fallSpeed, hasStartedDashing, cooldownDashAttuale;
    private float lastJumpPressed = float.MinValue;
    private float topPoint;//Diventa 1 in cima al salto
    private float attackRange;

    private ParticleSystem.MinMaxGradient currentGradient;
    #endregion

    #region FSM
    public StateMachine stateMachine;
    public StandingState standingState;
    public AirborneState airborneState;
    public DashingState dashingState;
    public BrutalAttackState brutalAttackState;
    public VerticalAttackState verticalAttackState;
    public HorizontalAttackState horizontalAttackState;
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        input = GetComponent<ExtendedInputActions>();

        currentHP = maxHP;

        stateMachine = new StateMachine();
        standingState = new StandingState(this, stateMachine);
        airborneState = new AirborneState(this, stateMachine);
        dashingState = new DashingState(this, stateMachine);
        horizontalAttackState = new HorizontalAttackState(this, stateMachine);
        verticalAttackState = new VerticalAttackState(this, stateMachine);
        brutalAttackState = new BrutalAttackState(this, stateMachine);

        stateMachine.Initialize(standingState);
    }

    private void Start() => CambiaStance(TipoStance.Agile);

    private void Update()
    {
        if (!GameManager.instance.IsPaused)
        {
            stateMachine.currentState.HandleInput();
            stateMachine.currentState.LogicUpdate();
        }

        if (currentHP <= 0) SceneManager.LoadScene(1, LoadSceneMode.Single);//DEBUG: X MORTE

        if (stackDiSangue < 0) stackDiSangue = 0;
        else if (stackDiSangue > 3) stackDiSangue = 3;

        if (cooldownStanceAttuale > 0) cooldownStanceAttuale -= Time.deltaTime;

        if (cooldownDashAttuale > 0) cooldownDashAttuale -= Time.deltaTime;

        GameManager.instance.BarraVita.value = currentHP;
        GameManager.instance.BarraStackDiSangue.value = stackDiSangue;

        //Extended animator
        var inputPoint = Mathf.Abs(Input.X);

        if (!GameManager.instance.IsPaused)//Per non fargli ribaltare lo sprite in pausa
        {
            if (Input.X != 0 && (stateMachine.currentState == airborneState || stateMachine.currentState == dashingState || stateMachine.currentState == standingState)) transform.localScale = new Vector3(Input.X > 0 ? 1 : -1, 1, 1);//Ribalta lo sprite in orizzontale
        }

        if (stateMachine.currentState == standingState)
        {
            if (stance == TipoStance.Agile)
            {
                if (inputPoint == 0) anim.Play("Agile_Idle");
                else if (inputPoint > 0) anim.Play("Agile_Running");
            }
            else if (stance == TipoStance.Brutale)
            {
                if (inputPoint == 0) anim.Play("Brutale_Idle");
                else if (inputPoint > 0) anim.Play("Brutale_Running");
            }
        }

        DetectGroundColor();

        moveParticles.transform.localScale = Vector3.MoveTowards(moveParticles.transform.localScale, Vector3.one * inputPoint, 2 * Time.deltaTime);

        Vector3 ray = Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue()); //con il viewport ha dei valori che non cambiano se ti muovi nello schermo

        if (ray.x < 0.47) dashFacingRight = false;
        else if (ray.x > 0.47) dashFacingRight = true;

        //Effetti secondari stack sete di sangue (maggiore attacco, probabilità critico), da sistemare, così è scritto male
        if (stackDiSangue == 0)
        {
            dannoAlNemico = 2;
            probabilitaCritico = 0;
        }
        else if (stackDiSangue == 1)
        {
            dannoAlNemico = 3;
            probabilitaCritico = 25;
        }
        else if (stackDiSangue == 2)
        {
            dannoAlNemico = 4;
            probabilitaCritico = 50;
        }
        else if (stackDiSangue == 3)
        {
            dannoAlNemico = 5;
            probabilitaCritico = 75;
        }
    }

    void FixedUpdate() => stateMachine.currentState.PhysicsUpdate();

    // TODO: Probabilità critico e danno critico: Con la probabilità di critico 10 % significa che un colpo su 10 è critico
    //void IncreaseCritChance(float critInc)
    //{
    //    probabilitaCritico += critInc;

    //    //Never let the crit chance go out of range
    //    if (probabilitaCritico > 100.0f) probabilitaCritico = 100.0f;
    //}

    //void DoAttack()
    //{
    //    float randValue = Random.value;

    //    if (randValue < probabilitaCritico)
    //    {
    //        //Do crit attack
    //    }
    //    else
    //    {
    //        //Do normal attack
    //    }
    //}

    #region Movimento di base
    public void BaseMovement()
    {
        fixedFrame++;
        frameClamp = moveClamp;

        Velocity = (rb.position - LastPosition) / Time.fixedDeltaTime;//Calcola la velocità del pg
        LastPosition = rb.position;
    }
    #endregion

    #region Gestione degli input
    public void HandleInput()
    {
        Input = input.HandleInput();

        if (!GameManager.instance.IsPaused)
        {
            if (Input.DashDown && Input.X != 0) dashToConsume = true;
            if (Input.JumpDown)
            {
                lastJumpPressed = fixedFrame;
                jumpToConsume = true;
            }

            //STANCE
            if (Input.CambiaStanceDown && cooldownStanceAttuale <= 0 && (stateMachine.currentState == airborneState || stateMachine.currentState == dashingState || stateMachine.currentState == standingState))
            {
                if (stance == TipoStance.Agile) CambiaStance(TipoStance.Brutale);
                else if (stance == TipoStance.Brutale) CambiaStance(TipoStance.Agile);
            }
        }
    }
    #endregion

    #region Stance
    public void CambiaStance(TipoStance stanceAttivata)
    {
        if (stanceAttivata == TipoStance.Agile)
        {
            cooldownStanceAttuale = cooldownStance;
            colliderAgile.gameObject.SetActive(true);
            colliderBrutale.gameObject.SetActive(false);
            coll = colliderAgile;
            stance = TipoStance.Agile;
            doubleJumpAbility = true;
            moveClamp = 13;
            dashAbility = true;
            attackRange = attackRangeAgile;
            dashToConsume = false;//Sennò dasha appena rientri nella stance dall'altra
        }

        if (stanceAttivata == TipoStance.Brutale)
        {
            cooldownStanceAttuale = cooldownStance;
            colliderAgile.gameObject.SetActive(false);
            colliderBrutale.gameObject.SetActive(true);
            coll = colliderBrutale;
            stance = TipoStance.Brutale;
            doubleJumpAbility = false;
            moveClamp = 5;
            dashAbility = false;
            attackRange = attackRangeBrutale;
        }
    }
    #endregion

    #region Collisioni
    public void CollisionsChecks()//I controlli dei raycast vengono usati per le informazioni pre-collisione
    {
        var bounds = coll.bounds;//Genera i range dei raggi.

        //Terreno
        var hasHitDown = RunDetection(Vector2.down, out hitsDown);
        hasHitUp = RunDetection(Vector2.up, out hitsUp);
        hasHitLeft = RunDetection(Vector2.left, out hitsLeft);
        hasHitRight = RunDetection(Vector2.right, out hitsRight);

        if (isGrounded && !hasHitDown)
        {
            timeLeftGrounded = fixedFrame;//Si triggera solo quando lascia il terreno la prima volta
            OnGroundedChanged(false);
        }
        else if (!isGrounded && hasHitDown)
        {
            canUseCoyotePriv = true;//Si triggera solo quando tocca il terreno per la prima volta
            didBufferedJump = false;
            canUseDoubleJump = true;
            canDash = true;
            OnGroundedChanged(true);
            Speed.y = 0;
        }

        isGrounded = hasHitDown;

        bool RunDetection(Vector2 direction, out RaycastHit2D[] hits)
        {
            hits = Physics2D.CapsuleCastAll(bounds.center, bounds.size, CapsuleDirection2D.Vertical, 0, direction, rayLengthDetection, groundLayer);//evita bug strani tipo che si appiccica alle piattaforme di lato

            foreach (var hit in hits)
            {
                if (hit.collider && !hit.collider.isTrigger)
                {
                    return true;
                }
            }
            return false;
        }
    }
    #endregion

    #region Movimento orizzontale
    public void HorizontalMovement()
    {
        if (Input.X != 0)
        {
            //Imposta la velocità di movimento orizzontale
            Speed.x += Input.X * speedingValue * Time.fixedDeltaTime;

            Speed.x = Mathf.Clamp(Speed.x, -frameClamp, frameClamp);//Fa il clamp con il massimo movimento del frame

            var topBonus = Mathf.Sign(Input.X) * jumpTopBonus * topPoint;//Applica il bonus in cima al salto
            Speed.x += topBonus * Time.fixedDeltaTime;
        }
        else Speed.x = Mathf.MoveTowards(Speed.x, 0, slowingValue * Time.fixedDeltaTime);//Se non ci sono input rallenta il pg

        if (!isGrounded && (Speed.x > 0 && hasHitRight || Speed.x < 0 && hasHitLeft)) Speed.x = 0;//Non aumentare la velocità in verticale, solo in orizzontale, per evitare che si appiccichi ai muri mentre è in aria
    }
    #endregion

    #region Gravità
    public void Gravity()
    {
        if (isGrounded)
        {
            //Pendenze
            Speed.y = gravityPower;
            foreach (var hit in hitsDown)
            {
                if (hit.collider.isTrigger) continue;
                var slopePerp = Vector2.Perpendicular(hit.normal).normalized;
                var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (slopeAngle != 0)//Fa sì che venga data priorità a cosa viene colpito davanti per una scivolata dalle pendenze migliore
                {
                    Speed.y = Speed.x * -slopePerp.y;
                    Speed.y += gravityPower;
                    break;
                }
            }
        }
        else
        {
            var fallingSpeed = endedJumpEarly && Speed.y > 0 ? fallSpeed * gravityModifierJumpEndedEarly : fallSpeed;//Aggiunge una forza che butta giù il pg mentre sale se il salto viene concluso prima del previsto

            Speed.y -= fallingSpeed * Time.fixedDeltaTime;//Caduta

            if (Speed.y < fallClamp) Speed.y = fallClamp;//Fa il clamp della velocità
        }
    }

    public void IsTopPoint()
    {
        if (!isGrounded)
        {
            topPoint = Mathf.InverseLerp(jumpTopLimit, 0, Mathf.Abs(Velocity.y));//Diventa sempre più forte man mano che ci sia avvicina alla cima
            fallSpeed = Mathf.Lerp(minFallSpeed, maxFallSpeed, topPoint);
        }
        else topPoint = 0;
    }
    #endregion

    #region Movimento avanzato
    public void Move()//Viene fatto un cast dei limiti prima di muoversi in modo da evitare collisioni future
    {
        RawMovement = Speed;//Per usare la velocità esternamente
        var move = RawMovement * Time.fixedDeltaTime;

        //Applica gli effector
        move += EvaluateForces();

        rb.MovePosition(rb.position + move);

        RunCornerPrevention();
    }
    #endregion

    #region Prevenzione incastro negli angoli
    void RunCornerPrevention()//Fa camminare e saltare anche sul bordo di una piattaforma
    {
        //Le linee sotto sono per quando i raggi non rilevano il terreno e il collider non entra nello spazio dell'oggetto in collisione, determina se il pg si muove o no.
        isStuckInCorner = !isGrounded && LastPos == rb.position && lastJumpPressed + 1 < fixedFrame;
        Speed.y = isStuckInCorner ? 0 : Speed.y;
        LastPos = rb.position;
    }
    #endregion

    #region Forze
    public void AddForce(Vector2 force, PlayerForce mode = PlayerForce.Burst, bool cancelMovement = true)
    {
        if (cancelMovement) Speed = Vector2.zero;

        switch (mode)
        {
            case PlayerForce.Burst:
                Speed += force;
                break;
            case PlayerForce.Decay:
                ForceBuildup += force * Time.fixedDeltaTime;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private Vector2 EvaluateForces()
    {
        //Previene il rimbalzo
        if (hasHitLeft || hasHitRight) ForceBuildup.x = 0;
        if (isGrounded || hasHitUp) ForceBuildup.y = 0;

        var force = ForceBuildup;

        ForceBuildup = Vector2.MoveTowards(ForceBuildup, Vector2.zero, forceDecay * Time.fixedDeltaTime);

        return force;
    }
    #endregion

    #region Vita
    public void TakeDamage(int damageTaken)
    {
        if (currentHP <= 0) return;
        else currentHP -= damageTaken;
    }
    #endregion

    #region Debug
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(verticalAttackPoint.position, verticalAttackRange);
    }
    #endregion

    #region Sistema particellare
    public void SetColor(ParticleSystem ps)
    {
        var main = ps.main;
        main.startColor = currentGradient;
    }

    public void DetectGroundColor()
    {
        var groundHits = Physics2D.RaycastAll(transform.position, Vector3.down, 2, groundLayer);//Rileva il colore del terreno per il particellare
        foreach (var hit in groundHits)
        {
            if (!hit || hit.collider.isTrigger || !hit.transform.TryGetComponent(out SpriteRenderer r)) continue;
            currentGradient = new ParticleSystem.MinMaxGradient(r.color * 0.9f, r.color * 1.2f);
            SetColor(moveParticles);
            return;
        }
    }
    #endregion

    #region Attacco
    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);//rileva nemici nel raggio dell'attacco

        //danneggia i nemici
        foreach (Collider2D enemy in hitEnemies)
        {
            stackDiSangue++;
            if (enemy.gameObject.TryGetComponent(out EnemyMelee melee)) melee.TakeDamage(dannoAlNemico);
            if (enemy.gameObject.TryGetComponent(out EnemyRanged ranged)) ranged.TakeDamage(dannoAlNemico);
        }
    }

    public void DealVerticalDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(verticalAttackPoint.position, verticalAttackRange, enemyLayers);//rileva nemici nel raggio dell'attacco

        //danneggia i nemici
        foreach (Collider2D enemy in hitEnemies)
        {
            stackDiSangue++;
            if (enemy.gameObject.TryGetComponent(out EnemyMelee melee))
            {
                melee.TakeDamage(dannoAlNemico);
                rb.AddForce(Vector2.up * moltiplicatoreRimbalzoAttaccoVerticale, ForceMode2D.Force);
            }
            if (enemy.gameObject.TryGetComponent(out EnemyRanged ranged))
            {
                ranged.TakeDamage(dannoAlNemico);
                rb.AddForce(Vector2.up * moltiplicatoreRimbalzoAttaccoVerticale, ForceMode2D.Force);
            }
        }
    }
    #endregion

    #region Animator
    private void OnEnable() => moveParticles.Play();

    private void OnDisable() => moveParticles.Stop();

    private void OnGroundedChanged(bool grounded)
    {
        if (grounded)
        {
            audioSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
            moveParticles.Play();

            landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, maxFallSpeedParticles, RawMovement.y);
            SetColor(landParticles);
            landParticles.Play();
        }
        else moveParticles.Stop();
    }

    public void OnJumping()
    {
        if (isGrounded)//Fa riprodurre il particellare solo quando è a terra
        {
            SetColor(jumpParticles);
            SetColor(launchParticles);
            jumpParticles.Play();
        }
    }

    public void OnDoubleJumping()
    {
        audioSource.PlayOneShot(doubleJumpClip);
        doubleJumpParticles.Play();
    }

    public void OnDashingChanged(bool isDashing)
    {
        if (isDashing)
        {
            dashParticles.Play();
            dashRingTransform.up = new Vector3(Input.X, Input.Y);
            dashRingParticles.Play();
            audioSource.PlayOneShot(dashClip);
        }
        else dashParticles.Stop();
    }
    #endregion

    #region Miscellaneous
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (stateMachine.currentState == dashingState)
        {
            if (other.gameObject.TryGetComponent(out EnemyMelee melee))
            {
                melee.TakeDamage(dannoAlNemico);
                melee.GetComponent<CapsuleCollider2D>().isTrigger = true;
            }
            else if (other.gameObject.TryGetComponent(out EnemyRanged ranged))
            {
                ranged.TakeDamage(dannoAlNemico);
                ranged.GetComponent<CapsuleCollider2D>().isTrigger = true;
            }
        }

        if (other.gameObject.CompareTag("Limite")) TakeDamage(MaxHP);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out EnemyMelee melee)) melee.GetComponent<CapsuleCollider2D>().isTrigger = false;
        else if (other.gameObject.TryGetComponent(out EnemyRanged ranged)) ranged.GetComponent<CapsuleCollider2D>().isTrigger = false;
    }
    #endregion
}