using UnityEngine;

public class RadialPlatform : MonoBehaviour, IPlayerEffector
{
    [SerializeField] private float speed = 3;
    [SerializeField] private float size = 2;

    private Transform trans;
    private Vector3 startPos, change, lastPos;

    private Vector3 Pos => trans.position;

    void Awake()
    {
        trans = transform;
        startPos = Pos;
    }

    void Update() => trans.position = startPos + new Vector3(Mathf.Cos(Time.time * speed), Mathf.Sin(Time.time * speed)) * size;

    private void FixedUpdate()
    {
        change = lastPos - Pos;
        lastPos = Pos;
    }

    public Vector2 EvaluateEffector()
    {
        return change.normalized * speed * Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;
        Gizmos.DrawWireSphere(transform.position, size);
    }
}