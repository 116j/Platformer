using UnityEngine;

public class WalkEnemy : MonoBehaviour, ISpawnChance
{
    [SerializeField]
    float m_walkSpeed = 1.5f;
    [SerializeField]
    bool m_canRun = true;
    [SerializeField]
    float m_runSpeed = 2f;
    [SerializeField]
    AttackScript m_attackScript;
    [SerializeField]
    protected DetectZone m_attackZone;
    [SerializeField]
    protected DetectZone m_detectZone;
    [SerializeField]
    protected DetectZone m_groundZone;
    [SerializeField]
    AnimationCurve m_spawnChance;

    protected Animator m_anim;
    protected Rigidbody2D m_rb;
    protected TouchingCheck m_touchings;
    Collider2D m_col;

    readonly int m_HashHorizontal = Animator.StringToHash("Horizontal");
    readonly int m_HashHit = Animator.StringToHash("Hit");
    readonly int m_HashDie = Animator.StringToHash("Die");
    readonly int m_HashCanMove = Animator.StringToHash("CanMove");
    readonly int m_HashAttackNum = Animator.StringToHash("AttackNum");

    bool m_dead = false;
    protected bool m_waiting = false;

    readonly float m_waitTime = 3f;
    readonly int m_attackCount = 3;

    protected int m_currentDir = 1;
    protected float m_speed = 0f;
    float m_waitTimer;

    protected virtual void Start()
    {
        m_anim = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody2D>();
        m_touchings = GetComponent<TouchingCheck>();
        m_col = GetComponent<Collider2D>();
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
            // if the target is in the attack zone - enable attack, stop moving
            if (m_attackZone.TargetDetected)
            {
                if (!m_attackScript.EnableAttack)
                {
                    m_attackScript.EnableAttack = true;
                    m_speed = 0f;
                    m_rb.velocity = Vector2.zero;
                    m_waiting = false;
                }
                m_anim.SetInteger(m_HashAttackNum, Random.Range(0, m_attackCount));
                return;
            }
            // if the target is in the detect zone, not touching walls and touching ground, and can move - chase the target
            else if (m_detectZone.TargetDetected && m_groundZone.TargetDetected && !m_touchings.IsWalls() && m_anim.GetBool(m_HashCanMove))
            {
                Chase();
            }
            else if (m_anim.GetBool(m_HashCanMove))
            {
                Potrol();
            }
            m_rb.velocity = m_currentDir * m_speed * Vector2.right;
        }
    }

    void Potrol()
    {
        // disable attack
        m_attackScript.EnableAttack = false;

        if (!m_waiting)
        {
            m_speed = m_walkSpeed;
            // if cant move further - stop
            if (!m_groundZone.TargetDetected || m_touchings.IsWalls())
            {
                m_speed = 0f;
                m_waitTimer = 0f;
                m_waiting = true;
            }
        }
        else
        {
            m_waitTimer += Time.deltaTime;
            // if wait time is over - turn around and move
            if (m_waitTimer >= m_waitTime)
            {
                TurnAround();
                m_waiting = false;
            }
        }
    }

    void Chase()
    {
        // if the target is behind - turn around
        if ((m_detectZone.TargetLocation.x - transform.position.x) * m_currentDir < 0f)
        {
            TurnAround();
        }
        m_attackScript.EnableAttack = false;
        m_waiting = false;
        m_speed = m_canRun ? m_runSpeed : m_walkSpeed;
    }

    void TurnAround()
    {
        m_currentDir *= -1;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + m_currentDir * 180f, 0f);
    }

    public virtual void ReceiveDamage(int damage)
    {
        m_rb.velocity = Vector2.zero;
        m_speed = 0f;
        if (damage == 0)
        {
            m_dead = true;
            m_col.isTrigger = true;
        }
        else
        {
            if ((m_detectZone.TargetLocation.x - transform.position.x) * m_currentDir < 0f)
            {
                TurnAround();
            }
            m_anim.SetTrigger(m_HashHit);
        }
    }

    public float GetSpawnChance()
    {
        return m_spawnChance.Evaluate(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlayedTime);
    }
}
