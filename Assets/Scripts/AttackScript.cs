using UnityEngine;

public class AttackScript : MonoBehaviour
{
    [SerializeField]
    string m_animAttackParameter;
    [SerializeField]
    float m_attackCooldownTime = 1.5f;

    public bool EnableAttack { get; set; } = true;

    Animator m_anim;
    Rigidbody2D m_rb;

    bool m_canAttack = true;
    float m_attackCooldown;
    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_canAttack && EnableAttack)
        {
            m_anim.SetTrigger(m_animAttackParameter);
        }
        else if (!m_canAttack)
        {
            m_attackCooldown += Time.deltaTime;
            if (m_attackCooldown >= m_attackCooldownTime)
            {
                m_attackCooldown = 0;
                m_canAttack = true;
            }
        }
    }

    public void AttackMoveForward()
    {
        if (m_rb != null)
            m_rb.velocity = transform.right * 5f;
    }  
    public void AttackMoveBackwards()
    {
        if (m_rb != null)
            transform.position-=transform.right*3;
    }

    public void AttackStop()
    {
        if (m_rb != null)
            m_rb.velocity = Vector2.zero;
    }

    public void ResetAttack()
    {
        m_canAttack = false;
        m_anim.ResetTrigger(m_animAttackParameter);
    }
}
