using UnityEngine;

public class ExtendedAnimator : MonoBehaviour
{
    [Header("Main System")]
    public Animator animator;
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform dashRingTransform;

    [Header("Miscellaneous Values")]
    [SerializeField, Range(0f, 3f)] float maxIdleSpeed = 2;
    [SerializeField] Vector2 scaleModifierCrouch = new Vector2(1, 0.7f);

    [Header("Particle System")]
    [SerializeField] ParticleSystem moveParticles;
    [SerializeField] ParticleSystem dashParticles;
    [SerializeField] ParticleSystem dashRingParticles;
    [SerializeField] ParticleSystem launchParticles;
    [SerializeField] ParticleSystem jumpParticles;
    [SerializeField] ParticleSystem doubleJumpParticles;
    [SerializeField] ParticleSystem landParticles;
    [SerializeField] float maxFallSpeedParticles = -40;

    [Header("Audio System")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] footsteps;
    [SerializeField] AudioClip dashClip;
    [SerializeField] AudioClip[] slideClips;
    [SerializeField] AudioClip doubleJumpClip;

    #region Campi privati
    private Player player;
    private Vector2 movement, spriteSizeDefault;
    private ParticleSystem.MinMaxGradient currentGradient;
    private static readonly int JumpKey = Animator.StringToHash("Jump");
    private static readonly int GroundedKey = Animator.StringToHash("Grounded");
    private static readonly int IdleSpeedKey = Animator.StringToHash("IdleSpeed");
    private static readonly int StartedAttackKey = Animator.StringToHash("StartedAttack");
    private static readonly int StartedAirAttackKey = Animator.StringToHash("StartedAirAttack");
    private static readonly int StartedBrutalAttackKey = Animator.StringToHash("StartedBrutalAttack");
    private static readonly int NextAttackKey = Animator.StringToHash("NextAttack");
    #endregion

    void Awake()
    {
        player = GetComponentInParent<Player>();

        spriteSizeDefault = playerSprite.size;

        player.OnGroundedChanged += OnLanded;
        player.OnStartAttackChanged += OnStartAttack;
        player.OnStartBrutalAttackChanged += OnStartBrutalAttack;
        player.OnJumping += OnJumping;
        player.OnDoubleJumping += OnDoubleJumping;
        player.OnDashingChanged += OnDashing;
        player.OnCrouchingChanged += OnCrouching;
    }

    void OnDestroy()
    {
        player.OnGroundedChanged -= OnLanded;
        player.OnStartAttackChanged -= OnStartAttack;
        player.OnStartBrutalAttackChanged -= OnStartBrutalAttack;
        player.OnJumping -= OnJumping;
        player.OnDoubleJumping -= OnDoubleJumping;
        player.OnDashingChanged -= OnDashing;
        player.OnCrouchingChanged -= OnCrouching;
    }

    private void OnDoubleJumping()
    {
        audioSource.PlayOneShot(doubleJumpClip);
        doubleJumpParticles.Play();
    }

    private void OnDashing(bool isDashing)
    {
        if (isDashing)
        {
            dashParticles.Play();
            dashRingTransform.up = new Vector3(player.Input.X, player.Input.Y);
            dashRingParticles.Play();
            audioSource.PlayOneShot(dashClip);
        }
        else dashParticles.Stop();
    }

    private void OnJumping()
    {
        animator.SetTrigger(JumpKey);
        animator.ResetTrigger(GroundedKey);

        if (player.isGrounded)//Fa riprodurre il particellare solo quando è a terra
        {
            SetColor(jumpParticles);
            SetColor(launchParticles);
            jumpParticles.Play();
        }
    }

    private void OnStartAttack()
    {
        animator.SetTrigger(StartedAttackKey);

        if (player.isAttackOver == true)
        {
            animator.SetTrigger(NextAttackKey);
            player.isAttackOver = false;
        }

        if (!player.isGrounded) animator.SetTrigger(StartedAirAttackKey);
    }

    private void OnStartBrutalAttack()
    {
        animator.SetTrigger(StartedBrutalAttackKey);
    }

    private void OnEndAttack()
    {
        animator.ResetTrigger(StartedAttackKey);
        player.isAttackOver = true;
    }

    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.attackPoint.position, player.attackRange, player.enemyLayers);//rileva nemici nel raggio dell'attacco

        //danneggia i nemici
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.GetType() == typeof(BoxCollider2D)) enemy.gameObject.GetComponent<Enemy>().TakeDamage(player.dannoAlNemico);
        }
    }

    public void DealVerticalDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.verticalAttackPoint.position, player.verticalAttackRange, player.enemyLayers);//rileva nemici nel raggio dell'attacco

        //danneggia i nemici
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.GetType() == typeof(BoxCollider2D)) enemy.gameObject.GetComponent<Enemy>().TakeDamage(player.dannoAlNemico);
        }
    }

    private void OnLanded(bool grounded)
    {
        if (grounded)
        {
            animator.SetTrigger(GroundedKey);
            audioSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
            moveParticles.Play();

            landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, maxFallSpeedParticles, movement.y);
            SetColor(landParticles);
            landParticles.Play();
        }
        else moveParticles.Stop();
    }

    private void OnCrouching(bool crouching)
    {
        if (crouching)
        {
            playerSprite.size = spriteSizeDefault * scaleModifierCrouch;
            audioSource.PlayOneShot(slideClips[Random.Range(0, slideClips.Length)], Mathf.InverseLerp(0, 5, Mathf.Abs(movement.x)));
        }
        else playerSprite.size = spriteSizeDefault;
    }

    void Update()
    {
        if (player == null) return;

        var inputPoint = Mathf.Abs(player.Input.X);

        if (player.Input.X != 0) transform.localScale = new Vector3(player.Input.X > 0 ? 1 : -1, 1, 1);//Ribalta lo sprite in orizzontale

        animator.SetFloat(IdleSpeedKey, Mathf.Lerp(1, maxIdleSpeed, inputPoint));//Fa aumentare la velocità dell'animazione quando corre

        DetectGroundColor();

        moveParticles.transform.localScale = Vector3.MoveTowards(moveParticles.transform.localScale, Vector3.one * inputPoint, 2 * Time.deltaTime);

        movement = player.RawMovement;//Il movimento del frame precedente vale di più
    }

    void DetectGroundColor()
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

    void SetColor(ParticleSystem ps)
    {
        var main = ps.main;
        main.startColor = currentGradient;
    }

    private void OnEnable() => moveParticles.Play();

    private void OnDisable() => moveParticles.Stop();
}