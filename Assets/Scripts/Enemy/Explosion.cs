using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float lifetime = 2f;

    void Start() => Destroy(gameObject, lifetime);
}
