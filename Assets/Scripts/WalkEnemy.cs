using UnityEngine;

public class WalkEnemy : MonoBehaviour, IBoolReceiver
{
    [SerializeField]
    float m_walkSpeed = 1.5f;
    [SerializeField]
    float m_runSpeed = 2f;
    [SerializeField]
    protected DetectZone m_attackZone;
    [SerializeField]
    protected DetectZone m_detectZone;
    [SerializeField]
    GameObject[] m_checkpoints;

    Animator m_anim;
    Rigidbody2D m_rb;

    readonly int m_HashHorizontal = Animator.StringToHash("Horizontal");
    readonly int m_HashHit = Animator.StringToHash("Hit");
    readonly int m_HashDie = Animator.StringToHash("Die");
    readonly int m_HashAttack = Animator.StringToHash("Attack");
    readonly int m_HashAttackNum = Animator.StringToHash("AttackNum");

    bool m_dead = false;
    bool m_waiting = false;
    bool m_chasing = true;
    bool m_bounds = false;

    readonly float m_waitTime = 3f;
    readonly int m_attackCount = 3;

    int m_currentDir = 1;
    float m_speed = 0f;
    float m_waitTimer;
    int m_currentCheckpoint;


    private void Start()
    {
        m_anim = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    { 
        m_anim.SetBool(m_HashDie, m_dead);
        m_anim.SetFloat(m_HashHorizontal, m_speed);
    }

    protected virtual void FixedUpdate()
    {
        if (!m_dead)
        {
            m_rb.velocity = m_currentDir * m_speed * Vector2.right;

            if (m_attackZone.PlayerDetected)
            {
                Attack();
            }
            else if (m_detectZone.PlayerDetected&&!m_bounds)
            {
                Chase();
            }
            else
            {
                Potrol();
            }
        }
    }

    void Potrol()
    {
        m_bounds = false;
        if (!m_waiting)
        {
            m_chasing = false;
            m_speed = m_walkSpeed;
        }
        else
        {
            m_waitTimer += Time.deltaTime;
            if (m_waitTimer >= m_waitTime)
            {
                TurnAround();
                m_waiting = false;
            }
        }
    }

    void Chase()
    {
        if (Vector2.Dot(transform.right, (m_detectZone.PlayerLocation - transform.position).normalized) < 0f)
        {
            TurnAround();
        }
        m_waiting = false;
        m_speed = m_runSpeed;
        m_chasing = true;
    }

    void TurnAround()
    {
        m_currentDir *= -1;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + m_currentDir * 180f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!m_dead)
        {
            if (collision.CompareTag("enemyCheckpoint") && collision.gameObject.Equals(m_checkpoints[m_currentCheckpoint]))
            {
                m_currentCheckpoint = (m_currentCheckpoint + 1) % m_checkpoints.Length;
                if(!m_chasing)
                {
                    m_waitTimer = 0f;
                    m_rb.velocity = Vector2.zero;
                    m_speed = 0f;
                    m_waiting = true;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemyBounds"))
        {
            m_bounds = true;
            m_chasing = false;
            TurnAround();
        }
    }

    protected virtual void Attack()
    {
        m_speed = 0f;
        m_waiting = false;
        m_rb.velocity = Vector2.zero;
        m_anim.SetTrigger(m_HashAttack);
        m_anim.SetInteger(m_HashAttackNum, Random.Range(0, m_attackCount));
    }

    public void ReceiveBool(bool value)
    {
        m_rb.velocity = Vector2.zero;
        m_speed = 0f;
        if (value)
        {
            m_dead = true;
        }
        else
        {
            if (Vector2.Dot(transform.right, (m_detectZone.PlayerLocation - transform.position).normalized) < 0f)
            {
                TurnAround();
            }
            m_anim.SetTrigger(m_HashHit);
        }
    }
}
