using UnityEngine;

public class ExtendedAnimator : MonoBehaviour
{
    [Header("Main System")]
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform dashRingTransform;

    [Header("Miscellaneous Values")]
    [SerializeField, Range(0f, 3f)] float maxIdleSpeed = 2;

    [Header("Audio System")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] footsteps;
    [SerializeField] AudioClip dashClip;
    [SerializeField] AudioClip[] slideClips;
    [SerializeField] AudioClip doubleJumpClip;

    #region Campi privati
    private Player player;
    private Vector2 movement;
    private static readonly int GroundedKey = Animator.StringToHash("Grounded");
    private static readonly int IdleSpeedKey = Animator.StringToHash("IdleSpeed");
    private static readonly int StartedAttackKey = Animator.StringToHash("StartedAttack");
    //private static readonly int StartedAirAttackKey = Animator.StringToHash("StartedAirAttack");
    private static readonly int StartedBrutalAttackKey = Animator.StringToHash("StartedBrutalAttack");
    private static readonly int NextAttackKey = Animator.StringToHash("NextAttack");
    #endregion

    void Awake()
    {
        player = GetComponentInParent<Player>();

        player.OnGroundedChanged += OnLanded;
        player.OnStartAttackChanged += OnStartAttack;
        player.OnStartBrutalAttackChanged += OnStartBrutalAttack;
        player.OnJumping += OnJumping;
        player.OnDoubleJumping += OnDoubleJumping;
        player.OnDashingChanged += OnDashing;
    }

    void OnDestroy()
    {
        player.OnGroundedChanged -= OnLanded;
        player.OnStartAttackChanged -= OnStartAttack;
        player.OnStartBrutalAttackChanged -= OnStartBrutalAttack;
        player.OnJumping -= OnJumping;
        player.OnDoubleJumping -= OnDoubleJumping;
        player.OnDashingChanged -= OnDashing;
    }

    private void OnDoubleJumping()
    {
        audioSource.PlayOneShot(doubleJumpClip);
        player.doubleJumpParticles.Play();
    }

    private void OnDashing(bool isDashing)
    {
        if (isDashing)
        {
            player.dashParticles.Play();
            dashRingTransform.up = new Vector3(player.Input.X, player.Input.Y);
            player.dashRingParticles.Play();
            audioSource.PlayOneShot(dashClip);
        }
        else player.dashParticles.Stop();
    }

    private void OnJumping()
    {
        if (player.isGrounded)//Fa riprodurre il particellare solo quando è a terra
        {
            player.SetColor(player.jumpParticles);
            player.SetColor(player.launchParticles);
            player.jumpParticles.Play();
        }
    }

    private void OnStartAttack()
    {
        anim.SetTrigger(StartedAttackKey);

        if (player.isAttackOver == true)
        {
            anim.SetTrigger(NextAttackKey);
            player.isAttackOver = false;
        }

        //if (!player.isGrounded) anim.SetTrigger(StartedAirAttackKey);
    }

    private void OnStartBrutalAttack()
    {
        anim.SetTrigger(StartedBrutalAttackKey);
    }

    private void OnEndAttack()
    {
        anim.ResetTrigger(StartedAttackKey);
        player.isAttackOver = true;
    }

    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.attackPoint.position, player.attackRange, player.enemyLayers);//rileva nemici nel raggio dell'attacco

        //danneggia i nemici
        foreach (Collider2D enemy in hitEnemies)
        {
            player.stackDiSangue++;
            if (enemy.gameObject.TryGetComponent(out EnemyMelee melee)) melee.TakeDamage(player.dannoAlNemico);
            if (enemy.gameObject.TryGetComponent(out EnemyRanged ranged)) ranged.TakeDamage(player.dannoAlNemico);
        }
    }

    public void DealVerticalDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.verticalAttackPoint.position, player.verticalAttackRange, player.enemyLayers);//rileva nemici nel raggio dell'attacco

        //danneggia i nemici
        foreach (Collider2D enemy in hitEnemies)
        {
            player.stackDiSangue++;
            if (enemy.gameObject.TryGetComponent(out EnemyMelee melee)) melee.TakeDamage(player.dannoAlNemico);
            if (enemy.gameObject.TryGetComponent(out EnemyRanged ranged)) ranged.TakeDamage(player.dannoAlNemico);
        }
    }

    private void OnLanded(bool grounded)
    {
        if (grounded)
        {
            anim.SetTrigger(GroundedKey);
            audioSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
            player.moveParticles.Play();

            player.landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, player.maxFallSpeedParticles, movement.y);
            player.SetColor(player.landParticles);
            player.landParticles.Play();
        }
        else player.moveParticles.Stop();
    }

    void Update()
    {
        if (player == null) return;

        var inputPoint = Mathf.Abs(player.Input.X);

        if (player.Input.X != 0 &&
            (player.stateMachine.currentState == player.airborneState ||
            player.stateMachine.currentState == player.dashingState ||
            player.stateMachine.currentState == player.standingState)) transform.localScale = new Vector3(player.Input.X > 0 ? 1 : -1, 1, 1);//Ribalta lo sprite in orizzontale

        anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, maxIdleSpeed, inputPoint));//Fa aumentare la velocità dell'animazione quando corre

        player.DetectGroundColor();

        player.moveParticles.transform.localScale = Vector3.MoveTowards(player.moveParticles.transform.localScale, Vector3.one * inputPoint, 2 * Time.deltaTime);

        movement = player.RawMovement;//Il movimento del frame precedente vale di più
    }

    private void OnEnable() => player.moveParticles.Play();

    private void OnDisable() => player.moveParticles.Stop();
}