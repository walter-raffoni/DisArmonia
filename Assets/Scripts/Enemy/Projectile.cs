using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private EnemyRanged enemy;

    public float destroyTime = 2;
    public float velocitaProiettile = 5;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponentInParent<EnemyRanged>();
    }

    void Update()
    {
        if (enemy.facingRight) rb.AddForce(transform.right * velocitaProiettile, ForceMode2D.Impulse);
        else if (!enemy.facingRight) rb.AddForce(-transform.right * velocitaProiettile, ForceMode2D.Impulse);

        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player)) player.TakeDamage(1);
    }
}
