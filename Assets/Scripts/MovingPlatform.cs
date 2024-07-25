using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    float m_speed = 3f;

    Vector3[] m_checkpoints;
    int m_currentCheckpoint;
    // Start is called before the first frame update
    void Start()
    {
        m_checkpoints = new Vector3[transform.childCount];
        for (int i = 0; i < m_checkpoints.Length; i++)
        {
            m_checkpoints[i] = transform.GetChild(i).position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(m_checkpoints[m_currentCheckpoint], transform.position) < 0.02f)
        {
            m_currentCheckpoint = (m_currentCheckpoint + 1) % m_checkpoints.Length;
        }

        transform.position = Vector2.MoveTowards(transform.position, m_checkpoints[m_currentCheckpoint], m_speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

}
