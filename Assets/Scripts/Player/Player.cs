using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
     * STANCE AGILE:
     * - Non sarà possibile accumulare cariche di sangue
     * - Movimento su asse X, velocità alta;
     * - Doppio salto, altezza alta;
     * - Attacco base, range alto, danno normale;
     * - [Abilità attiva] Dash multi-direzionale, range alto, danno basso;
     * - [Abilità passiva] Probabilità colpo critico normale;
     * - Trasformarsi non resetta il cooldown delle abilità
     */

public class Player : Character
{
    #region Campi modificabili
    [Header("Abilities")]
    public bool creepingAbility;//Walking
    public bool doubleJumpAbility, dashAbility;

    [Header("Stance System")]
    public Stance stance;
    public enum Stance { Agile, Brutale }

    [Header("Crouch System")]
    public int crouchSlowdownFrames = 50;
    public float sizeModifierCrouch = 0.5f;
    public float speedModifierCrouch = 0.1f;
    public float immediateSlowdownLimitCrouch = 0.1f;

    [Header("Collision System")]
    public LayerMask groundLayer;
    public float rayLengthDetection = 0.1f;

    [Header("Effectors System")]
    public float forceDecay = 1;

    [Header("Walk System")]
    public float slowingValue = 60;
    public float moveClamp = 13;
    public float speedingValue = 120;
    public float jumpTopBonus = 100;

    [Header("Gravity System")]
    public float fallClamp = -60;
    public float minFallSpeed = 80;
    public float maxFallSpeed = 160;
    [Range(0, -10)] public float gravityPower = -1.5f;

    [Header("Jump System")]
    public int jumpBuffer = 7;
    public float jumpHeight = 35;
    public float jumpTopLimit = 40;
    public int coyoteTimeLimit = 7;
    public float gravityModifierJumpEndedEarly = 3;

    [Header("Dash System")]
    public int dashLength = 6;
    public float dashPower = 30;
    public float horizontalMultiplierDashEnd = 0.25f;

    [Header("Attack System")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int dannoAlNemico = 2;
    #endregion

    #region Campi privati
    private ExtendedInputActions input;

    private RaycastHit2D[] hitsUp = new RaycastHit2D[1];
    private RaycastHit2D[] hitsDown = new RaycastHit2D[3];
    private RaycastHit2D[] hitsLeft = new RaycastHit2D[1];
    private RaycastHit2D[] hitsRight = new RaycastHit2D[1];

    private readonly List<IPlayerEffector> usedEffectors = new List<IPlayerEffector>();

    [HideInInspector] public bool endedJumpEarly = true;
    private float lastJumpPressed = float.MinValue;

    [HideInInspector] public float topPoint;//Diventa 1 in cima al salto
    [HideInInspector] public bool CanDoubleJump => canUseDoubleJump && !canUseCoyote;
    [HideInInspector] public bool CanUseCoyote => canUseCoyote && !isGrounded && timeLeftGrounded + coyoteTimeLimit > fixedFrame;
    [HideInInspector] public bool HasBufferedJump => ((isGrounded && !didBufferedJump) || isStuckInCorner) && lastJumpPressed + jumpBuffer > fixedFrame;
    #endregion

    #region Campi "pubblici"
    public FrameInput Input { get; private set; }
    public Vector2 RawMovement { get; private set; }
    public event Action<bool> OnGroundedChanged;
    public event Action<bool> OnDashingChanged;
    public event Action<bool> OnCrouchingChanged;
    public event Action OnStartAttackChanged;
    public event Action OnJumping, OnDoubleJumping;

    [HideInInspector] public BoxCollider2D coll;
    [HideInInspector] public int frameStartedCrouching, fixedFrame;
    [HideInInspector] public float crouchVelocity, timeLeftGrounded, frameClamp, fallSpeed, hasStartedDashing;
    [HideInInspector] public Vector2 defaultSizeCollider, defaultOffsetCollider, dashVelocity, lastPos, forceBuildup, lastPosition, velocity, speed;
    [HideInInspector] public bool isCrouching, isGrounded, isStuckInCorner, didBufferedJump, jumpToConsume, dashToConsume, hasHitUp, hasHitRight, hasHitLeft, canUseCoyote, canUseDoubleJump, canDash;

    public enum PlayerForce { Burst, Decay } //Burst: Aggiunta direttamente alla velocità di movimento del pg, da controllare con la decelerazione standard | Decay: Forza additiva gestita dal sistema di decadimento

    public bool CanStand
    {
        get
        {
            var col = Physics2D.OverlapBox((Vector2)transform.position + defaultOffsetCollider, defaultSizeCollider * (0.95f * transform.localScale.y), 0, groundLayer);
            return (col == null || col.isTrigger);
        }
    }
    #endregion

    #region FSM
    public StateMachine stateMachine;
    public StandingState standingState;
    public CrouchingState crouchingState;
    public AirborneState airborneState;
    public DashingState dashingState;
    public HorizontalAttackState horizontalAttackState;
    public VerticalAttackState verticalAttackState;
    public BrutalAttackState1 brutalAttackState1;
    #endregion

    public int stackDiSangue = 0;
    public float tempoAttacco;
    public float tempoAttaccoOriginale = 2;

    public bool isAttackOver = false;

    public Animator anim;

    void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponentInChildren<Animator>();
        input = GetComponent<ExtendedInputActions>();

        defaultSizeCollider = coll.size;
        defaultOffsetCollider = coll.offset;

        stateMachine = new StateMachine();
        standingState = new StandingState(this, stateMachine);
        crouchingState = new CrouchingState(this, stateMachine);
        airborneState = new AirborneState(this, stateMachine);
        dashingState = new DashingState(this, stateMachine);
        horizontalAttackState = new HorizontalAttackState(this, stateMachine);
        verticalAttackState = new VerticalAttackState(this, stateMachine);
        brutalAttackState1 = new BrutalAttackState1(this, stateMachine);

        stateMachine.Initialize(standingState);
    }

    private void Update()
    {
        stateMachine.currentState.HandleInput();
        if (!GameManager.instance.IsPaused) stateMachine.currentState.LogicUpdate();

        if (currentHP <= 0) SceneManager.LoadScene(1, LoadSceneMode.Single);//DEBUG: X MORTE

        if (stackDiSangue < 0) stackDiSangue = 0;
        else if (stackDiSangue > 4) stackDiSangue = 4;
    }

    void FixedUpdate() => stateMachine.currentState.PhysicsUpdate();

    #region Movimento di base
    public void BaseMovement()
    {
        fixedFrame++;
        frameClamp = moveClamp;

        velocity = (rb.position - lastPosition) / Time.fixedDeltaTime;//Calcola la velocità del pg
        lastPosition = rb.position;
    }
    #endregion

    #region Gestione degli input
    public void HandleInput()
    {
        Input = input.HandleInput();

        if (Input.PauseDown) GameManager.instance.PauseGame();//TODO: Spostare nel game manager

        if (Input.ReloadGameDown) SceneManager.LoadScene(1, LoadSceneMode.Single);//TODO: Spostare nel game manager

        if (!GameManager.instance.IsPaused)
        {
            if (Input.DashDown && Input.X != 0) dashToConsume = true;
            if (Input.JumpDown)
            {
                lastJumpPressed = fixedFrame;
                jumpToConsume = true;
            }

            //STANCE
            if (Input.CambiaStanceDown)
            {
                if (stance == Stance.Agile) CambiaStance(Stance.Brutale);
                else if (stance == Stance.Brutale) CambiaStance(Stance.Agile);
            }
        }
    }
    #endregion

    #region Stance
    public void CambiaStance(Stance stanceAttivata)
    {
        if (stanceAttivata == Stance.Agile)
        {
            stance = Stance.Agile;
            doubleJumpAbility = true;
            moveClamp = 13;
            dashAbility = true;
            dashToConsume = false;//Sennò dasha appena rientri nella stance dall'altra
        }

        if (stanceAttivata == Stance.Brutale)
        {
            stance = Stance.Brutale;
            doubleJumpAbility = false;
            moveClamp = 5;
            dashAbility = false;
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
            OnGroundedChanged?.Invoke(false);
        }
        else if (!isGrounded && hasHitDown)
        {
            canUseCoyote = true;//Si triggera solo quando tocca il terreno per la prima volta
            didBufferedJump = false;
            canUseDoubleJump = true;
            canDash = true;
            OnGroundedChanged?.Invoke(true);
            speed.y = 0;
        }

        isGrounded = hasHitDown;

        bool RunDetection(Vector2 direction, out RaycastHit2D[] hits)
        {
            hits = Physics2D.BoxCastAll(bounds.center, bounds.size, 0, direction, rayLengthDetection, groundLayer);

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
            if (creepingAbility) speed.x = Mathf.MoveTowards(speed.x, frameClamp * Input.X, speedingValue * Time.fixedDeltaTime);
            else speed.x += Input.X * speedingValue * Time.fixedDeltaTime;

            speed.x = Mathf.Clamp(speed.x, -frameClamp, frameClamp);//Fa il clamp con il massimo movimento del frame

            var topBonus = Mathf.Sign(Input.X) * jumpTopBonus * topPoint;//Applica il bonus in cima al salto
            speed.x += topBonus * Time.fixedDeltaTime;
        }
        else speed.x = Mathf.MoveTowards(speed.x, 0, slowingValue * Time.fixedDeltaTime);//Se non ci sono input rallenta il pg

        if (!isGrounded && (speed.x > 0 && hasHitRight || speed.x < 0 && hasHitLeft)) speed.x = 0;//Non aumentare la velocità in verticale, solo in orizzontale, per evitare che si appiccichi ai muri mentre è in aria
    }
    #endregion

    #region Gravità
    public void Gravity()
    {
        if (isGrounded)
        {
            if (Input.X == 0) return;

            //Pendenze
            speed.y = gravityPower;
            foreach (var hit in hitsDown)
            {
                if (hit.collider.isTrigger) continue;
                var slopePerp = Vector2.Perpendicular(hit.normal).normalized;
                var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (slopeAngle != 0)//Fa sì che vengta data priorità a cosa viene colpito davanti per una scivolata dalle pendenze migliore
                {
                    speed.y = speed.x * -slopePerp.y;
                    speed.y += gravityPower;
                    break;
                }
            }
        }
        else
        {
            var fallingSpeed = endedJumpEarly && speed.y > 0 ? fallSpeed * gravityModifierJumpEndedEarly : fallSpeed;//Aggiunge una forza che butta giù il pg mentre sale se il salto viene concluso prima del previsto

            speed.y -= fallingSpeed * Time.fixedDeltaTime;//Caduta

            if (speed.y < fallClamp) speed.y = fallClamp;//Fa il clamp della velocità
        }
    }

    public void TopPoint()
    {
        if (!isGrounded)
        {
            topPoint = Mathf.InverseLerp(jumpTopLimit, 0, Mathf.Abs(velocity.y));//Diventa sempre più forte man mano che ci sia avvicina alla cima
            fallSpeed = Mathf.Lerp(minFallSpeed, maxFallSpeed, topPoint);
        }
        else
        {
            //if (stateMachine.currentState != standingState && !crouching) stateMachine.ChangeState(standingState);//Senza questo controllo rimane sullo standing
            topPoint = 0;
        }
    }
    #endregion

    #region Movimento avanzato
    public void Move()//Viene fatto un cast dei limiti prima di muoversi in modo da evitare collisioni future
    {
        RawMovement = speed;//Per usare la velocità esternamente
        var move = RawMovement * Time.fixedDeltaTime;

        //Applica gli effector
        move += EvaluateEffectors();
        move += EvaluateForces();

        rb.MovePosition(rb.position + move);

        RunCornerPrevention();
    }
    #endregion

    #region Prevenzione incastro negli angoli
    void RunCornerPrevention()//Fa camminare e saltare anche sul bordo di una piattaforma
    {
        //Le linee sotto sono per quando i raggi non rilevano il terreno e il collider non entra nello spazio dell'oggetto in collisione, determina se il pg si muove o no. || TODO: Fixa il fatto che se sta su un angolo e salta di sotto senza muoversi sembra non triggerare il rilevamento del terreno quando lo tocca
        isStuckInCorner = !isGrounded && lastPos == rb.position && lastJumpPressed + 1 < fixedFrame;
        speed.y = isStuckInCorner ? 0 : speed.y;
        lastPos = rb.position;
    }
    #endregion

    #region Effector
    private Vector2 EvaluateEffectors()//Per più effetti di forza passivi, come piattaforme in movimento, sott'acqua e così via
    {
        var effectorDirection = Vector2.zero;
        effectorDirection += Process(hitsDown);//Lo fa ripetere per ogni direzione e anche per effector ad area come le zone di vento, ecc...

        usedEffectors.Clear();
        return effectorDirection;

        Vector2 Process(IEnumerable<RaycastHit2D> hits)
        {
            foreach (var hit2D in hits)
            {
                if (!hit2D.transform) return Vector2.zero;
                if (hit2D.transform.TryGetComponent(out IPlayerEffector effector))
                {
                    if (usedEffectors.Contains(effector)) continue;
                    usedEffectors.Add(effector);
                    return effector.EvaluateEffector();
                }
            }
            return Vector2.zero;
        }
    }
    #endregion

    #region Forze
    public void AddForce(Vector2 force, PlayerForce mode = PlayerForce.Burst, bool cancelMovement = true)
    {
        if (cancelMovement) speed = Vector2.zero;

        switch (mode)
        {
            case PlayerForce.Burst:
                speed += force;
                break;
            case PlayerForce.Decay:
                forceBuildup += force * Time.fixedDeltaTime;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private Vector2 EvaluateForces()
    {
        //Previene il rimbalzo
        if (hasHitLeft || hasHitRight) forceBuildup.x = 0;
        if (isGrounded || hasHitUp) forceBuildup.y = 0;

        var force = forceBuildup;

        forceBuildup = Vector2.MoveTowards(forceBuildup, Vector2.zero, forceDecay * Time.fixedDeltaTime);

        return force;
    }
    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        if (!coll) coll = GetComponent<BoxCollider2D>();

        Gizmos.color = Color.blue;
        var b = coll.bounds;
        b.Expand(rayLengthDetection);

        Gizmos.DrawWireCube(b.center, b.size);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    #endregion

    #region Invocazioni
    public void OnJumpingInvoke() => OnJumping?.Invoke();
    public void OnDoubleJumpingInvoke() => OnDoubleJumping?.Invoke();
    public void OnStartAttackChangedInvoke() => OnStartAttackChanged?.Invoke();
    public void OnCrouchingChangedInvoke(bool state) => OnCrouchingChanged?.Invoke(state);
    public void OnDashingChangedInvoke(bool state) => OnDashingChanged?.Invoke(state);
    #endregion
}