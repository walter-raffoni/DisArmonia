using UnityEngine;

public class Enemy : Character
{
    [Header("Movement System")]
    public float speed;
    public Transform startPoint;
    public Transform endPoint;
    public LayerMask layerOstacoli;
    public Vector3 offsetRilevazione;
    [HideInInspector] public bool facingRight = true;

    [Header("Attack System")]
    public GameObject target;
    public float speedAttacco;
    public int puntiDanno = 2;
    public GameObject projectilePrefab;
    public GameObject firePoint;

    [Header("Stance System")]
    public int stanceMaggioreDanno;
    public ParticleSystem particleStance;
    public Color coloreStanceAgile = Color.blue;
    public Color coloreStanceBrutale = Color.red;

    [Header("Animation System")]
    public Animator anim;

    public void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    public void Spara() => Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation, transform);
}