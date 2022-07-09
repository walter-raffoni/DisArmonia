using UnityEngine;

public class Enemy : Character
{
    public GameObject target;
    public Transform startPoint, endPoint;
    public float speed, speedAttacco;
    public int statoNemico, attributoNemico;
    public ParticleSystem particellare;
    public LayerMask layerOstacoli;
    public Vector3 offsetRilevazione;
    public int puntiDanno = 2;
    public GameObject projectilePrefab;
    public GameObject firePoint;
    [HideInInspector] public bool facingRight = true;

    public void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    public void Spara() => GameObject.Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation, transform);
}