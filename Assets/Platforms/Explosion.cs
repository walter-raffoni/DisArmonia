using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float growSpeed = 1;
    [SerializeField] private float growSize = 1;
    [SerializeField] private float explosionForce = 50;

    void Update()
    {
        var scale = (Mathf.Sin(Time.time * growSpeed) + 2) * growSize;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            var dir = other.transform.position - transform.position;
            player.AddForce(dir.normalized * explosionForce, Player.PlayerForce.Decay);
        }
    }
}