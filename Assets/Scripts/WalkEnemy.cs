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
    protected DetectZone m_groundZone;

    Animator m_anim;
    Rigidbody2D m_rb;
    TouchingCheck m_touchings;

    readonly int m_HashHorizontal = Animator.StringToHash("Horizontal");
    readonly int m_HashHit = Animator.StringToHash("Hit");
    readonly int m_HashDie = Animator.StringToHash("Die");
    readonly int m_HashAttack = Animator.StringToHash("Attack");
    readonly int m_HashAttackNum = Animator.StringToHash("AttackNum");

    bool m_dead = false;
    bool m_waiting = false;
    bool m_notAttacking = false;

    readonly float m_waitTime = 3f;
    readonly int m_attackCount = 3;
    readonly float m_attackCooldownTime = 1.5f;

    int m_currentDir = 1;
    float m_speed = 0f;
    float m_waitTimer;
    float m_attackCooldown;


    private void Start()
    {
        m_anim = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody2D>();
        m_touchings = GetComponent<TouchingCheck>();
    }

    protected virtual void Update()
    { 
        if(m_notAttacking)
        {
            m_attackCooldown += Time.deltaTime;
            if (m_attackCooldown > m_attackCooldownTime)
            {
                m_attackCooldown = 0f;
                m_notAttacking= false;
            }
        }
        m_anim.SetBool(m_HashDie, m_dead);
        m_anim.SetFloat(m_HashHorizontal, m_speed);
    }

    protected virtual void FixedUpdate()
    {
        if (!m_dead)
        {
            m_rb.velocity = m_currentDir * m_speed * Vector2.right;

            if (m_attackZone.TargetDetected)
            {
                Attack();
            }
            else if (m_detectZone.TargetDetected&&m_groundZone.TargetDetected&&!m_touchings.IsWalls())
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
        if (!m_waiting)
        {
            m_speed = m_walkSpeed;

            if (!m_groundZone.TargetDetected|| m_touchings.IsWalls())
            {
                m_speed = 0f;
                m_waitTimer = 0f;
                m_waiting = true;
            }
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
        m_waiting = false;
        m_speed = m_runSpeed;
    }

    void TurnAround()
    {
        m_currentDir *= -1;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + m_currentDir * 180f, 0f);
    }

    protected virtual void Attack()
    {
        if (m_notAttacking) return;
        m_speed = 0f;
        m_waiting = false;
        m_notAttacking = true;
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
            if ((m_detectZone.TargetLocation.x - transform.position.x)*m_currentDir < 0f)
            {
                TurnAround();
            }
            m_anim.SetTrigger(m_HashHit);
        }
    }
}
