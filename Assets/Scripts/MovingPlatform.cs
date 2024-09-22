using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    float m_speed = 3f;
    [SerializeField]
    float m_waitTime = 2f;

    List<Vector3> m_checkpoints = new List<Vector3>();
    int m_currentCheckpoint;
    float m_waitTimer;

    bool m_waiting = false;
    bool m_start = false;

    float m_checkpointOffset = 0.21f;
    SpawnValues m_spawnValues;
    // Start is called before the first frame update
    void Awake()
    {
        m_spawnValues = transform.GetComponent<SpawnValues>();
    }

    private void Start()
    {
        m_checkpoints.Add(transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        // wait before move
        if (m_waiting)
        {
            m_waitTimer += Time.deltaTime;
            if (m_waitTimer >= m_waitTime)
            {
                m_waitTimer = 0f;
                m_waiting = false;
            }
        }
        else if (m_start)
        {
            transform.position = Vector2.MoveTowards(transform.position, m_checkpoints[m_currentCheckpoint], m_speed * Time.deltaTime);

            if (Vector3.Distance(m_checkpoints[m_currentCheckpoint], transform.position) < 0.02f)
            {
                m_waiting = true;
                m_currentCheckpoint = (m_currentCheckpoint + 1) % m_checkpoints.Count;
            }
        }
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

    public void SetWaitTime(float time)
    {
        m_waitTime = time;
    }

    public void SetSpeed(float speed)
    {
        m_speed = speed;
    }

    public void AddCheckpoint(Vector3 pos)
    {
        
        m_checkpoints.Add(new Vector3(pos.x, pos.y - m_checkpointOffset));
    }

    public void StartMovement()
    {
        m_start = true;
    }

}
