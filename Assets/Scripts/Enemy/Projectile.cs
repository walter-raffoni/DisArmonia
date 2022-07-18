using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private EnemyRanged enemy;

    [SerializeField] float destroyTime = 2;
    [SerializeField] float velocitaProiettile = 5;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponentInParent<EnemyRanged>();
    }

    void Update()
    {
        if (enemy.FacingRight) rb.AddForce(transform.right * velocitaProiettile, ForceMode2D.Impulse);
        else if (!enemy.FacingRight) rb.AddForce(-transform.right * velocitaProiettile, ForceMode2D.Impulse);

        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponentInParent<Player>() != null) other.gameObject.GetComponentInParent<Player>().TakeDamage(1);
    }
}
