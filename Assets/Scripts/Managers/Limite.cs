using UnityEngine;

public class Limite : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out Player player)) player.TakeDamage(player.maxHP);
    }
}