using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    private Vector2 startPosition;

    [Header("Camera")]
    [SerializeField] Camera roomCamera;

    [Header("Multipliers")]
    [SerializeField] float parallaxMultiplierX;
    [SerializeField] float parallaxMultiplierY;

    void Start() => startPosition = transform.position;

    void Update()
    {
        Vector2 distance = new Vector2(roomCamera.transform.position.x * parallaxMultiplierX, roomCamera.transform.position.y * parallaxMultiplierY);

        transform.position = startPosition + distance;
    }
}
