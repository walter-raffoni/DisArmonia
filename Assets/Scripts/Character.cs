using UnityEngine;

public class Character : MonoBehaviour, IVita<int>
{
    [HideInInspector] public Rigidbody2D rb;

    [Header("Health System")]
    [SerializeField] int maxHP = 10;
    [HideInInspector] public int currentHP;

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