using UnityEngine;

public class DefendingEnemy : WalkEnemy
{
    [SerializeField]
    DetectZone m_playerAttackZone;

    Damagable m_damagable;

    readonly int m_HashProtect = Animator.StringToHash("Protect");

    bool m_protecting = false;
    float m_protectCooldownTime = 5f;
    float m_protectCooldown;

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
            if (m_protectCooldown >= m_protectCooldownTime || !m_playerAttackZone.TargetDetected)
            {
                m_anim.SetBool(m_HashProtect, false);
                m_protectCooldown = 0;
                m_protecting = false;
                m_damagable.Invinsible = false;
            }
        }

        base.Update();
    }

    protected override void FixedUpdate()
    {
        if (!m_protecting)
        {
            base.FixedUpdate();
        }
    }

    public override void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);
        m_attackScript.EnableAttack = false;
        m_rb.velocity = Vector2.zero;
        m_protecting = true;
        m_anim.SetBool(m_HashProtect, true);
        m_damagable.Invinsible = true;
        m_waiting = false;
    }
}
