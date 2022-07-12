using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int dannoArma = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy)) enemy.TakeDamage(dannoArma);
    }
}