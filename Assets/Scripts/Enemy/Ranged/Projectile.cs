using UnityEngine;

public class Projectile : MonoBehaviour
{
    private EnemyRanged enemyRanged;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyRanged = GetComponentInParent<EnemyRanged>();
    }

    void Update()
    {
        Vector3 newPos = Vector3.MoveTowards(transform.position, enemyRanged.Target.position, enemyRanged.VelocitaProiettile * Time.fixedDeltaTime);
        transform.position = newPos;

        Destroy(gameObject, enemyRanged.DestroyTimeProiettile);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponentInParent<Player>() != null)
        {
            other.gameObject.GetComponentInParent<Player>().TakeDamage(1);
            Destroy(gameObject);
        }
    }
}