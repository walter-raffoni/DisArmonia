using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Campi pubblici
    public Transform Target
    {
        get { return target; }
        set { target = value; }
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
    public int PuntiDanno
    {
        get { return puntiDanno; }
        set { puntiDanno = value; }
    }
    #endregion

    #region Campi privati
    private float velocitaProiettile, destroyTimeProiettile;
    private int puntiDanno;
    private Rigidbody2D rb;
    private Transform target;
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = (target.position - transform.position).normalized * velocitaProiettile;//Così non segue il giocatore se si muove ma prende solo l'ultima posizione al momento della creazione
    }

    void Update() => Destroy(gameObject, destroyTimeProiettile);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<Player>() != null)
        {
            other.GetComponentInParent<Player>().TakeDamage(puntiDanno);
            Destroy(gameObject);
        }
        else Destroy(gameObject);
    }
}