using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public UnityEvent damageEvent;

    [Header("Health System")]
    [SerializeField] int maxHP = 10;
    [SerializeField] GameObject barraVita;//SpriteRenderer

    [Header("Movement System")]
    [SerializeField] float speed = 0.75f;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    [Header("Attack System")]
    [SerializeField] protected Transform target;
    [SerializeField] protected int puntiDanno = 2;
    [SerializeField] protected LayerMask playerMask;

    [Header("Stance System")]
    [SerializeField] ParticleSystem particleStance;
    [SerializeField] Color coloreStanceAgile = Color.blue;
    [SerializeField] Color coloreStanceBrutale = Color.red;

    #region Campi pubblici
    public int StanceMaggioreDanno => stanceMaggioreDanno;
    public bool FacingRight => facingRight;
    public float Speed => speed;
    public Animator Anim => anim;
    public Transform Target => target;
    public Transform EndPoint => endPoint;
    public Transform StartPoint => startPoint;
    #endregion

    #region Campi privati
    private Animator anim;
    protected bool facingRight = true;
    private int currentHP, stanceMaggioreDanno;
    #endregion

    protected virtual void Awake()
    {
        Room room = GetComponentInParent<Room>();

        anim = GetComponent<Animator>();

        currentHP = maxHP;

        stanceMaggioreDanno = Random.Range(0, 2);//0: agile, 1: brutale

        var main = particleStance.main;
        if (stanceMaggioreDanno == 0)
        {
            room.TotalAgile++;
            if ((room.TotalBrutale - room.TotalAgile) != 0)
            {
                stanceMaggioreDanno = 1;
                room.TotalAgile--;
                room.TotalBrutale++;
                main.startColor = coloreStanceBrutale;
            }
            main.startColor = coloreStanceAgile;
        }
        else if (stanceMaggioreDanno == 1)
        {
            room.TotalBrutale++;
            if ((room.TotalBrutale - room.TotalAgile) != 0)
            {
                stanceMaggioreDanno = 0;
                room.TotalAgile++;
                room.TotalBrutale--;
                main.startColor = coloreStanceAgile;
            }
            main.startColor = coloreStanceBrutale;
        }
    }

    public void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    public void TakeDamage(int damageTaken)
    {
        currentHP -= damageTaken;

        float barValue = (float)currentHP / maxHP;
        barraVita.gameObject.transform.localScale = new Vector3(barValue, 0.2f, 1);

        if (currentHP <= 0)
        {
            Destroy(gameObject);
            target.GetComponent<Player>().CooldownDashAttuale = 0;
            target.GetComponent<Player>().JumpToConsume = true;
        }
        damageEvent.Invoke();
    }
}
