using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DetectZone : MonoBehaviour
{
    [SerializeField]
    protected string m_tag;
    public bool TargetDetected { get; private set; }
    public Vector3 TargetLocation { get; protected set; }


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
