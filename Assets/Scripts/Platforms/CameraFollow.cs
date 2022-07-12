using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothTime = 0.5f;
    [SerializeField] private float xMin, xMax;

    private float yLock;
    private Vector3 currentVelocity;

    void Start() => yLock = transform.position.y;

    void Update()
    {
        if (!player) return;

        var target = new Vector3(Mathf.Clamp(player.position.x, xMin, xMax), yLock, -10);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref currentVelocity, smoothTime);
    }
}