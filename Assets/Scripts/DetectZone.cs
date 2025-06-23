using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DetectZone : MonoBehaviour
{
    [SerializeField]
    protected string m_tag;
    public bool TargetDetected { get; private set; }
    public Vector3 TargetLocation { get; protected set; }
    public Vector3 RightBorder => m_col.bounds.max;
    public Vector3 LeftBorder => m_col.bounds.min;

    BoxCollider2D m_col;

    private void Start()
    {
        m_col = GetComponent<BoxCollider2D>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(m_tag))
        {
            TargetDetected = true;
            TargetLocation = collision.ClosestPoint(transform.position);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(m_tag))
        {
            TargetLocation = collision.ClosestPoint(transform.position);
            TargetDetected = true;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(m_tag))
            TargetDetected = false;
    }
}
