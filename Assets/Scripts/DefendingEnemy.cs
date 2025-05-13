using UnityEngine;

public class DefendingEnemy : WalkEnemy
{
    [SerializeField]
    DetectZone m_playerAttackZone;

    Damagable m_damagable;

    readonly int m_HashProtect = Animator.StringToHash("Protect");

    bool m_protecting = false;
    float m_protectCooldownTime = 3f;
    float m_protectCooldown;

    float m_protectDelay = 0.3f;
    float m_protectDelayCooldown;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_damagable = GetComponent<Damagable>();
    }

    protected override void Update()
    {
        if (m_protecting)
        {
            m_protectCooldown += Time.deltaTime;
            if (m_protectCooldown >= m_protectCooldownTime || !m_playerAttackZone.TargetDetected|| m_attackZone.TargetDetected)
            {
                DisableProtection();
            }
        }

        base.Update();
    }

    void DisableProtection()
    {
        m_anim.SetBool(m_HashProtect, false);
        m_protectCooldown = 0;
        m_protecting = false;
        m_damagable.Invinsible = false;
    }

    void EnableProtection()
    {
        m_attackScript.EnableAttack = false;
        m_rb.velocity = Vector2.zero;
        m_protecting = true;
        m_anim.SetBool(m_HashProtect, true);
        m_damagable.Invinsible = true;
        m_waiting = false;
    }

    protected override void FixedUpdate()
    {
        if (!m_protecting)
        {
            if (m_playerAttackZone.TargetDetected&&!m_attackZone.TargetDetected)
            {
                m_protectDelayCooldown += Time.fixedDeltaTime;
                if (m_protectDelayCooldown >= m_protectDelay)
                {
                    m_protectDelayCooldown = 0;
                    EnableProtection();
                }
            }
            else
            {
                m_protectDelayCooldown = 0;
                base.FixedUpdate();
            }
        }
    }

    public override void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);
        if (damage < 0)
        {
            EnableProtection();
        }
    }
}
