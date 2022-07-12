using UnityEngine;

public class PatrolPlatform : MonoBehaviour, IPlayerEffector
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2[] points;
    [SerializeField] private float speed = 1;
    [SerializeField] private bool looped;

    private int index;
    private bool ascending;
    private Vector2 startPos, change, lastPos;
    private Vector2 Pos => rb.position;

    private void Awake() => startPos = rb.position;

    private void FixedUpdate()
    {
        var target = points[index] + startPos;
        var newPos = Vector2.MoveTowards(Pos, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Pos == target)
        {
            index = ascending ? index + 1 : index - 1;
            if (index >= points.Length)
            {
                if (looped) index = 0;
                else
                {
                    ascending = false;
                    index--;
                }
            }
            else if (index < 0)
            {
                ascending = true;
                index = 1;
            }
        }
        change = lastPos - newPos;
        lastPos = newPos;
    }

    public Vector2 EvaluateEffector()
    {
        return -change; //* _speed;
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;
        var curPos = (Vector2)transform.position;
        var previous = curPos + points[0];
        for (var i = 0; i < points.Length; i++)
        {
            var p = points[i] + curPos;
            Gizmos.DrawWireSphere(p, 0.2f);
            Gizmos.DrawLine(previous, p);

            previous = p;

            if (looped && i == points.Length - 1) Gizmos.DrawLine(p, curPos + points[0]);
        }
    }
}