using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private BoxCollider2D coll;
    private Player player;
    private float timeToUnlock = float.MinValue;
    [SerializeField] private float fallThroughUnlockTime = 0.25f;

    private void Awake() => coll = GetComponent<BoxCollider2D>();

    private void Update()
    {
        if (player == null) return;
        if (player.Input.Y < 0) timeToUnlock = Time.time + fallThroughUnlockTime;

        coll.enabled = player.RawMovement.y <= 0 && Time.time >= timeToUnlock;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player controller)) player = controller;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player controller)) player = null;
    }
}