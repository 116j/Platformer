using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DetectZone : MonoBehaviour
{
    [SerializeField]
    string m_tag;
    public bool TargetDetected { get; private set; }
    public Vector3 TargetLocation { get; private set; }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(m_tag))
        {
            TargetDetected = true;
            TargetLocation = collision.transform.position;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(m_tag))
        {
            TargetLocation = collision.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(m_tag))
            TargetDetected = false;
    }
}
