using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Campi pubblici
    public Transform Target
    {
        get { return target; }
        set { target = value; }
    }
    public int PuntiDanno
    {
        get { return puntiDanno; }
        set { puntiDanno = value; }
    }
    public float VelocitaProiettile
    {
        get { return velocitaProiettile; }
        set { velocitaProiettile = value; }
    }
    public float DestroyTimeProiettile
    {
        get { return destroyTimeProiettile; }
        set { destroyTimeProiettile = value; }
    }
    #endregion

    #region Campi privati
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CircleCollider2D coll;
    private Transform target;
    private int puntiDanno;
    public float velocitaProiettile, destroyTimeProiettile;
    public ParticleSystem flame, smoke;
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<CircleCollider2D>();

        rb.velocity = (target.position - transform.position).normalized * velocitaProiettile;//Così non segue il giocatore se si muove ma prende solo l'ultima posizione al momento della creazione
        Destroy(gameObject, destroyTimeProiettile);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name.ToString());
        if (other.CompareTag("Player"))
        {
            int direction = transform.position.x > other.transform.position.x ? -1 : 1;
            other.GetComponentInParent<Player>().TakeDamage(puntiDanno, direction);
        }
        CollisionTrailActive();
    }

    void CollisionTrailActive()
    {
        rb.velocity = Vector2.zero;
        sr.enabled = false;
        coll.enabled = false;
        this.enabled = false;
        var emission_0 = flame.emission;
        emission_0.enabled = false;
        var emission_1 = smoke.emission;
        emission_1.enabled = false;
        Destroy(gameObject, 1.5f);
    }
}