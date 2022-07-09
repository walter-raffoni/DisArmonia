using UnityEngine;

public class Character : MonoBehaviour, IVita<int>
{
    [Header("Health System")]
    [SerializeField] int maxHP = 10;
    [HideInInspector] public int currentHP;

    [Header("Weapon System")]
    [SerializeField] GameObject weapon;
    [HideInInspector] public Rigidbody2D rb;

    private void Start()
    {
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damageTaken)
    {
        if (currentHP <= 0) return;
        else currentHP -= damageTaken;
    }
}
