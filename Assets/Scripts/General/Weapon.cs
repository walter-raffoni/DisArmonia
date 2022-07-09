using UnityEngine;

public class Weapon : MonoBehaviour
{
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    public int dannoArma = 2;

    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy)) enemy.TakeDamage(dannoArma);
    }

    public void BaseAttack()
    {
        coll.enabled = true;
        sprite.enabled = true;
    }

    public void StopBaseAttack()
    {
        coll.enabled = false;
        sprite.enabled = false;
    }
}