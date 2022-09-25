using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public float stunDuration = 1f;
    public Vector2 stunDirection;
    public float stunForceMultiplier;

    [Header("Health System")]
    [SerializeField] int maxHP = 10;

    [Header("Movement System")]
    [SerializeField] float slowingValue = 60;
    [SerializeField] float moveClamp = 13;
    [SerializeField] float speedingValue = 120;
    [SerializeField] float forceDecay = 1;//Effectors
    [SerializeField] float speedAgile = 13f;
    [SerializeField] float speedBrutal = 5f;

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
    [SerializeField] float gravityModifierJumpEndedEarly = 3;

    [Header("Collision System")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float rayLengthDetection = 0.1f;

    [Header("Dash System")]
    [SerializeField] float cooldownDash = 2;
    [SerializeField] int dashLength = 6;
    [SerializeField] float dashPower = 30;
    [SerializeField] float horizontalMultiplierDashEnd = 0.25f;

    [Header("General Attack System")]
    [SerializeField] int dannoAlNemico = 2;
    [SerializeField] LayerMask enemyLayers;

    [Header("Horizontal Attack System")]
    [SerializeField] Transform attackPoint;
    [SerializeField] Transform verticalAttackPoint;
    [SerializeField] float attackRangeAgile = 0.5f;
    [SerializeField] float attackRangeBrutale = 0.42f;
    [SerializeField] float tempoAttaccoOrizAgile1 = 0.75f;
    [SerializeField] float tempoAttaccoOrizAgile2 = 0.75f;
    [SerializeField] float tempoAttaccoOrizAgile3 = 0.75f;
    [SerializeField] float tempoAttaccoOrizBrutale1 = 0.75f;
    [SerializeField] float tempoAttaccoOrizBrutale2 = 0.75f;
    [SerializeField] float tempoAttaccoOrizBrutale3 = 0.75f;
    [SerializeField] float tempoAttaccoBrutale = 0.75f;
    [SerializeField] float cooldownAttaccoPotente = 1.5f;

    [Header("Vertical Attack System")]
    [SerializeField] float verticalAttackRange = 0.5f;
    [SerializeField] float rimbalzoVerticale = 2.5f;

    [Header("Powerful Attack System")]
    [SerializeField] float probabilitaCritico = 25;
    [SerializeField] int dannoCriticoAggiuntivo = 2;
    [SerializeField] int dannoAlNemicoAggiuntivoPerStanceGiusta = 1;

    [Header("Invulnerability System")]
    [SerializeField] float tempoInvulnerabilitaDopoCollisioneConNemico = 1;
    [SerializeField] float timeBetweenAlphaChange = .4f;
    [SerializeField] float alphaValue0;
    [SerializeField] float alphaValue1;
    [SerializeField] int alphaCycles = 3;

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
    [SerializeField] ParticleSystem agileIdleParticle;
    [SerializeField] ParticleSystem brutalIdleParticle;
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
    public bool EnemyTouched => enemyTouched;
    public bool HasBufferedJump => ((isGrounded && !didBufferedJump) || isStuckInCorner) && lastJumpPressed + jumpBuffer > fixedFrame;
    public bool CanDash
    {
        get { return canDash; }
        set { canDash = value; }
    }
    public bool IsInvulnerable
    {
        get { return isInvulnerable; }
        set { isInvulnerable = value; }
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
    public bool DidBufferedJump
    {
        get { return didBufferedJump; }
        set { didBufferedJump = value; }
    }
    public bool CanUseDoubleJump
    {
        get { return canUseDoubleJump; }
        set { canUseDoubleJump = value; }
    }

    public float JumpHeight => jumpHeight;
    public float CooldownDash => cooldownDash;
    public float CooldownAttaccoPotente => cooldownAttaccoPotente;
    public float JumpTopLimit => jumpTopLimit;
    public float CooldownStanceAttuale => cooldownStanceAttuale;
    public float MinFallSpeed
    {
        get { return minFallSpeed; }
        set { minFallSpeed = value; }
    }
    public float MaxFallSpeed
    {
        get { return maxFallSpeed; }
        set { maxFallSpeed = value; }
    }
    public float TempoAttaccoBrutale => tempoAttaccoBrutale;
    public float TempoAttaccoOrizAgile1 => tempoAttaccoOrizAgile1;
    public float TempoAttaccoOrizAgile2 => tempoAttaccoOrizAgile2;
    public float TempoAttaccoOrizAgile3 => tempoAttaccoOrizAgile3;
    public float MaxFallSpeedParticles => maxFallSpeedParticles;
    public float TempoAttaccoOrizBrutale1 => tempoAttaccoOrizBrutale1;
    public float TempoAttaccoOrizBrutale2 => tempoAttaccoOrizBrutale2;
    public float TempoAttaccoOrizBrutale3 => tempoAttaccoOrizBrutale3;
    public float TopPoint
    {
        get { return topPoint; }
        set { topPoint = value; }
    }
    public float FallSpeed
    {
        get { return fallSpeed; }
        set { fallSpeed = value; }
    }
    public float TimeLeftGrounded
    {
        get { return timeLeftGrounded; }
        set { timeLeftGrounded = value; }
    }

    public float CooldownDashAttuale
    {
        get { return cooldownDashAttuale; }
        set { cooldownDashAttuale = value; }
    }
    public float CooldownAttaccoPotenteAttuale
    {
        get { return cooldownAttaccoPotenteAttuale; }
        set { cooldownAttaccoPotenteAttuale = value; }
    }
    public float TempoAttaccoAttuale
    {
        get { return tempoAttaccoAttuale; }
        set { tempoAttaccoAttuale = value; }
    }

    public TipoStance Stance => stance;
    public enum TipoStance { Agile, Brutale }

    public ParticleSystem MoveParticles => moveParticles;
    public ParticleSystem DashParticles => dashParticles;
    public ParticleSystem LandParticles => landParticles;
    public ParticleSystem JumpParticles => jumpParticles;
    public ParticleSystem LaunchParticles => launchParticles;
    public ParticleSystem DashRingParticles => dashRingParticles;
    public ParticleSystem DoubleJumpParticles => doubleJumpParticles;

    public AudioSource AudioSource => audioSource;

    public AudioClip DashClip => dashClip;
    public AudioClip DoubleJumpClip => doubleJumpClip;
    public AudioClip[] Footsteps => footsteps;

    public Transform DashRingTransform => dashRingTransform;

    public CapsuleCollider2D Coll => coll;
    #endregion

    #region Campi privati
    private Animator anim;

    public Rigidbody2D rb;

    private TipoStance stance;

    private SpriteRenderer sr;

    private TrailRenderer trail;

    private CapsuleCollider2D coll;

    private ExtendedInputActions input;

    private int currentHP, comboAttacco, stackDiSangue, fixedFrame;

    private bool enemyTouched, doubleJumpAbility, dashAbility, isGrounded, isStuckInCorner, didBufferedJump, jumpToConsume, dashToConsume, hasHitUp, hasHitRight, hasHitLeft, canUseDoubleJump, canDash, isInvulnerable;
    private bool endedJumpEarly = true;

    private RaycastHit2D[] hitsUp = new RaycastHit2D[1];
    private RaycastHit2D[] hitsDown = new RaycastHit2D[3];
    private RaycastHit2D[] hitsLeft = new RaycastHit2D[1];
    private RaycastHit2D[] hitsRight = new RaycastHit2D[1];

    private float tempoAttaccoAttuale, cooldownAttaccoPotenteAttuale, cooldownStanceAttuale, timeLeftGrounded, frameClamp, fallSpeed, hasStartedDashing, cooldownDashAttuale, attackRange, topPoint;//TopPoint diventa 1 in cima al salto
    private float lastJumpPressed = float.MinValue;

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
    public DamagedState damagedState;
    #endregion

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        input = GetComponent<ExtendedInputActions>();

        currentHP = maxHP;

        stateMachine = new StateMachine();
        standingState = new StandingState(this, stateMachine);
        airborneState = new AirborneState(this, stateMachine);
        dashingState = new DashingState(this, stateMachine);
        horizontalAttackState = new HorizontalAttackState(this, stateMachine);
        verticalAttackState = new VerticalAttackState(this, stateMachine);
        brutalAttackState = new BrutalAttackState(this, stateMachine);
        damagedState = new DamagedState(this, stateMachine);

        stateMachine.Initialize(standingState);
    }

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        CambiaStance(TipoStance.Agile);
    }

    private void Update()
    {
        if (!GameManager.instance.IsPaused)
        {
            stateMachine.currentState.HandleInput();
            stateMachine.currentState.LogicUpdate();

            //Extended animator
            var inputPoint = Mathf.Abs(Input.X);

            DetectGroundColor();

            moveParticles.transform.localScale = Vector3.MoveTowards(moveParticles.transform.localScale, Vector3.one * inputPoint, 2 * Time.deltaTime);

            if (cooldownStanceAttuale >= 0) cooldownStanceAttuale -= Time.deltaTime;
            if (cooldownDashAttuale >= 0) cooldownDashAttuale -= Time.deltaTime;
            if (cooldownAttaccoPotenteAttuale >= 0) cooldownAttaccoPotenteAttuale -= Time.deltaTime;
        }
    }

    void FixedUpdate() => stateMachine.currentState.PhysicsUpdate();

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

    #region Movimento
    #region Movimento base
    public void BaseMovement()
    {
        fixedFrame++;
        frameClamp = moveClamp;

        Velocity = (rb.position - LastPosition) / Time.fixedDeltaTime;//Calcola la velocit� del pg
        LastPosition = rb.position;
    }

    public void HorizontalMovement()
    {
        if (Input.X != 0)
        {
            //Imposta la velocit� di movimento orizzontale
            Speed.x += Input.X * speedingValue * Time.fixedDeltaTime;

            Speed.x = Mathf.Clamp(Speed.x, -frameClamp, frameClamp);//Fa il clamp con il massimo movimento del frame

            var topBonus = Mathf.Sign(Input.X) * jumpTopBonus * topPoint;//Applica il bonus in cima al salto
            Speed.x += topBonus * Time.fixedDeltaTime;
        }
        else Speed.x = Mathf.MoveTowards(Speed.x, 0, slowingValue * Time.fixedDeltaTime);//Se non ci sono input rallenta il pg

        if (!isGrounded && (Speed.x > 0 && hasHitRight || Speed.x < 0 && hasHitLeft)) Speed.x = 0;//Non aumentare la velocit� in verticale, solo in orizzontale, per evitare che si appiccichi ai muri mentre � in aria
    }

    public void Move()//Viene fatto un cast dei limiti prima di muoversi in modo da evitare collisioni future
    {
        RawMovement = Speed;//Per usare la velocit� esternamente
        var move = RawMovement * Time.fixedDeltaTime;

        //Applica gli effector
        move += EvaluateForces();

        rb.MovePosition(rb.position + move);

        RunCornerPrevention();
    }

    void RunCornerPrevention()//Fa camminare e saltare anche sul bordo di una piattaforma
    {
        //Le linee sotto sono per quando i raggi non rilevano il terreno e il collider non entra nello spazio dell'oggetto in collisione, determina se il pg si muove o no.
        isStuckInCorner = !isGrounded && LastPos == rb.position && lastJumpPressed + 1 < fixedFrame;
        Speed.y = isStuckInCorner ? 0 : Speed.y;
        LastPos = rb.position;
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

    #region Movimento verticale
    public void Gravity()
    {
        if (!isGrounded)
        {
            var fallingSpeed = endedJumpEarly && Speed.y > 0 ? fallSpeed * gravityModifierJumpEndedEarly : fallSpeed;//Aggiunge una forza che butta gi� il pg mentre sale se il salto viene concluso prima del previsto

            Speed.y -= fallingSpeed * Time.fixedDeltaTime;//Caduta

            if (Speed.y < fallClamp) Speed.y = fallClamp;//Fa il clamp della velocit�
        }
    }

    public void IsTopPoint()
    {
        if (!isGrounded)
        {
            topPoint = Mathf.InverseLerp(jumpTopLimit, 0, Mathf.Abs(Velocity.y));//Diventa sempre pi� forte man mano che ci sia avvicina alla cima
            fallSpeed = Mathf.Lerp(minFallSpeed, maxFallSpeed, topPoint);
        }
        else
        {
            if (stateMachine.currentState == airborneState || stateMachine.currentState == verticalAttackState) stateMachine.ChangeState(standingState);
            if (stateMachine.currentState == standingState) topPoint = 0;
        }
    }

    public void CanJump()
    {
        //Ha premuto il tasto di salto? || Se c'� un buffer per il salto sufficiente || � a terra
        if (jumpToConsume || HasBufferedJump || !isGrounded) stateMachine.ChangeState(airborneState);
    }

    public void Jump()
    {
        if (doubleJumpAbility)
        {
            //Controlla se c'� un salto da poter consumare e pu� effettivamente fare il doppio salto
            if (jumpToConsume && canUseDoubleJump)
            {
                Speed.y = jumpHeight;
                canUseDoubleJump = false;
                endedJumpEarly = false;
                jumpToConsume = false;
                DoubleJumpEffect();
            }
        }

        // Termina il salto se il pulsante viene rilasciato prima
        if (!isGrounded && !Input.JumpHeld && !endedJumpEarly && Velocity.y > 0) endedJumpEarly = true;

        if (hasHitUp && Speed.y > 0) Speed.y = 0;
    }
    #endregion

    #region Movimento orizzontale
    public void CanDashNow()
    {
        if (dashToConsume && canDash && Input.X != 0 && dashAbility && cooldownDashAttuale <= 0) stateMachine.ChangeState(dashingState);
    }

    public void Dash()
    {
        if (dashToConsume && canDash)
        {
            var vel = new Vector2(Input.X, isGrounded && Input.Y < 0 ? 0 : Input.Y).normalized;
            if (vel == Vector2.zero) { dashToConsume = false; return; }
            DashVelocity = vel * dashPower;
            canDash = false;
            hasStartedDashing = fixedFrame;

            ForceBuildup = Vector2.zero;//Toglie ogni forza aggiunta dall'esterno
        }

        Speed.x = DashVelocity.x;
        Speed.y = DashVelocity.y;

        //Annulla la corsa quando il tempo � finita o � stata raggiunta la distanza di sicurezza mssima
        if (hasStartedDashing + dashLength < fixedFrame)
        {
            if (Speed.y > 0) Speed.y = 0;
            Speed.x *= horizontalMultiplierDashEnd;
            if (isGrounded)
            {
                canDash = true;
                stateMachine.ChangeState(standingState);
            }
            else stateMachine.ChangeState(airborneState);//cos� non passa allo stato di standing per un attimo anche se in aria
        }
        dashToConsume = false;
    }
    #endregion
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
            moveClamp = speedAgile;
            dashAbility = true;
            attackRange = attackRangeAgile;
            dashToConsume = false;//Senn� dasha appena rientri nella stance dall'altra
            if (stackDiSangue > 0) Heal(stackDiSangue);
            CambiaStack(0);
            brutalIdleParticle.Stop();
            agileIdleParticle.Play();
        }

        if (stanceAttivata == TipoStance.Brutale)
        {
            cooldownStanceAttuale = cooldownStance;
            colliderAgile.gameObject.SetActive(false);
            colliderBrutale.gameObject.SetActive(true);
            coll = colliderBrutale;
            stance = TipoStance.Brutale;
            doubleJumpAbility = false;
            moveClamp = speedBrutal;
            dashAbility = false;
            attackRange = attackRangeBrutale;
            brutalIdleParticle.Play();
            agileIdleParticle.Stop();
        }
    }
    #endregion

    #region Stack di sangue
    public void CambiaStack(int numeroStack)
    {
        if (numeroStack == 0) stackDiSangue = numeroStack;
        else stackDiSangue = stackDiSangue + numeroStack;

        //Effetti secondari stack sete di sangue (maggiore attacco, probabilit� critico)
        if (stackDiSangue < 0) stackDiSangue = 0;
        else if (stackDiSangue == 0)
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
        else if (stackDiSangue > 3) stackDiSangue = 3;

        GameManager.instance.BarraStackDiSangue.value = stackDiSangue;
    }
    #endregion

    #region Collisioni
    public void CollisionsChecks()//controlli dei raycast che vengono usati per informazioni pre-collisione
    {
        var bounds = coll.bounds;//genera range dei raggi.

        //Terreno
        var hasHitDown = RunDetection(Vector2.down, out hitsDown);
        hasHitUp = RunDetection(Vector2.up, out hitsUp);
        hasHitLeft = RunDetection(Vector2.left, out hitsLeft);
        hasHitRight = RunDetection(Vector2.right, out hitsRight);

        if (isGrounded && !hasHitDown) timeLeftGrounded = fixedFrame;//si triggera solo quando lascia il terreno la prima volta
        else if (!isGrounded && hasHitDown)
        {
            didBufferedJump = false;
            canUseDoubleJump = true;
            canDash = true;
            Speed.y = 0;
        }

        isGrounded = hasHitDown;

        bool RunDetection(Vector2 direction, out RaycastHit2D[] hits)
        {
            hits = Physics2D.CapsuleCastAll(bounds.center, bounds.size, CapsuleDirection2D.Vertical, 0, direction, rayLengthDetection, groundLayer);//evita bug strani tipo che si appiccica alle piattaforme di lato

            foreach (var hit in hits)
            {
                if (hit.collider && !hit.collider.isTrigger) return true;
            }
            return false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (stateMachine.currentState == airborneState && other.gameObject.TryGetComponent(out Enemy enemy))
        {
            enemyTouched = true;
            dashToConsume = false;
        }

        if (other.gameObject.CompareTag("Limite")) SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            enemyTouched = true;

            if (stateMachine.currentState == dashingState)
            {
                enemy.GetComponent<CapsuleCollider2D>().isTrigger = true;//permette di trapassare il nemico quando dasha
                enemy.TakeDamage(dannoAlNemico);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        enemyTouched = false;

        if (other.gameObject.TryGetComponent(out Enemy enemy)) dashToConsume = false;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        enemyTouched = false;

        if (other.gameObject.TryGetComponent(out Enemy enemy)) enemy.GetComponent<CapsuleCollider2D>().isTrigger = false;
    }
    #endregion

    #region Vita
    #region Cura
    private void Heal(int healAmount)
    {
        if (currentHP == maxHP) return;
        currentHP += healAmount;

        GameManager.instance.BarraVita.value = currentHP;

        if (currentHP > maxHP) currentHP = maxHP;
    }
    #endregion

    #region Danno
    public void TakeDamage(int damageTaken, int direction)
    {
        if (isInvulnerable) return;
        else
        {
            StartCoroutine("Invulnerable");
            if (currentHP <= 0) SceneManager.LoadScene(1, LoadSceneMode.Single);
            else currentHP -= damageTaken;

            GameManager.instance.BarraVita.value = currentHP;
            stateMachine.ChangeState(damagedState);
            Stun(direction);

        }
    }

    public void DealDamage()
    {
        float randValue = Random.value * 100;

        if (randValue > 100) randValue = 100;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);//rileva nemici nel raggio dell'attacco

        //danneggia i nemici
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (randValue < probabilitaCritico)
            {
                if (stance == TipoStance.Brutale) CambiaStack(1);

                if (enemyCollider.gameObject.TryGetComponent(out Enemy enemy))
                {
                    //0: agile, 1: brutale
                    if (stance == TipoStance.Agile && enemy.StanceMaggioreDanno == 0) enemy.TakeDamage(dannoAlNemico + dannoCriticoAggiuntivo + dannoAlNemicoAggiuntivoPerStanceGiusta);
                    else if (stance == TipoStance.Brutale && enemy.StanceMaggioreDanno == 1) enemy.TakeDamage(dannoAlNemico + dannoCriticoAggiuntivo + dannoAlNemicoAggiuntivoPerStanceGiusta);
                    else enemy.TakeDamage(dannoAlNemico + dannoCriticoAggiuntivo);
                }
            }
            else
            {
                if (stance == TipoStance.Brutale) CambiaStack(1);

                if (enemyCollider.gameObject.TryGetComponent(out Enemy enemy))
                {
                    if (stance == TipoStance.Agile && enemy.StanceMaggioreDanno == 0) enemy.TakeDamage(dannoAlNemico + dannoAlNemicoAggiuntivoPerStanceGiusta);
                    else if (stance == TipoStance.Brutale && enemy.StanceMaggioreDanno == 1) enemy.TakeDamage(dannoAlNemico + dannoAlNemicoAggiuntivoPerStanceGiusta);
                    else enemy.TakeDamage(dannoAlNemico);
                }
            }
        }
    }

    public void DealVerticalDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(verticalAttackPoint.position, verticalAttackRange, enemyLayers);//rileva nemici nel raggio dell'attacco

        //danneggia i nemici
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            CambiaStack(1);//non serve il controllo sulla stance perch� tanto questo attacco � fattibile solo in una stance

            if (enemyCollider.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(dannoAlNemico);
                Speed.y = JumpHeight;//serve per il rimbalzo del pg quando colpisce
            }
        }
    }

    //Timer totale invulnerabilit� | timer tra trasparenze | valore trasparenza 0 | valore trasparenza 1
    IEnumerator Invulnerable()
    {
        bool flag = true;
        Color tempColor = sr.color;
        isInvulnerable = true;
        for (int i = 0; i < alphaCycles; i++)
        {
            tempColor.a = flag ? alphaValue0 : alphaValue1;
            sr.color = tempColor;
            yield return new WaitForSeconds(tempoInvulnerabilitaDopoCollisioneConNemico / alphaCycles / 2);
            flag = !flag;
            tempColor.a = flag ? alphaValue0 : alphaValue1;
            sr.color = tempColor;
            yield return new WaitForSeconds(tempoInvulnerabilitaDopoCollisioneConNemico / alphaCycles / 2);
        }
        tempColor.a = 1;
        sr.color = tempColor;
        isInvulnerable = false;
        yield return null;
    }
    #endregion
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

    private void OnEnable() => moveParticles.Play();

    private void OnDisable() => moveParticles.Stop();

    public void GroundedEffect()
    {
        audioSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
        moveParticles.Play();

        landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, maxFallSpeedParticles, RawMovement.y);
        SetColor(landParticles);
        landParticles.Play();
    }

    public void DoubleJumpEffect()
    {
        audioSource.PlayOneShot(doubleJumpClip);
        doubleJumpParticles.Play();
    }

    public void JumpEffect()
    {
        SetColor(jumpParticles);
        SetColor(launchParticles);
        jumpParticles.Play();
    }

    public void DashEffect()
    {
        dashParticles.Play();
        dashRingTransform.up = new Vector3(Input.X, Input.Y);
        dashRingParticles.Play();
        audioSource.PlayOneShot(dashClip);
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

    public void Stun(int direction)
    {
        //Vector2 _stunDirection = direction == -1 ? Vector2.left : Vector2.right;
        Vector2 _stunDirection = new Vector2(stunDirection.x * direction, stunDirection.y).normalized * stunForceMultiplier;
        rb.velocity = Vector2.zero;
        rb.AddForce(_stunDirection, ForceMode2D.Impulse);
        //Debug.Log(stunDirection.magnitude.ToString());
        if (stance == TipoStance.Brutale)
        {
            anim.Play("Brutale_Damaged");
        }
        else
        {
            anim.Play("Agile_Damaged");
        }
    }
}