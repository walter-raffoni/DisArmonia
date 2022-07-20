using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    private Vector2 startPosition;

    [Header("View")]
    [SerializeField] Player playerView;

    [Header("Multipliers")]
    [SerializeField] float parallaxMultiplierX;
    [SerializeField] float parallaxMultiplierY;

    void Start() => startPosition = transform.position;

    void Update()
    {
        Vector2 distance = new Vector2(playerView.transform.position.x * parallaxMultiplierX, playerView.transform.position.y * parallaxMultiplierY);

        transform.position = startPosition + distance;
    }
}
