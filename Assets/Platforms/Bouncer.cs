using UnityEngine;

public class Bouncer : MonoBehaviour
{
    [SerializeField] private float bounceForce = 70;

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.TryGetComponent(out Player player)) player.AddForce(transform.up.normalized * bounceForce);
    }
}