using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendingEnemy : WalkEnemy
{
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
            if (m_protectCooldown >= m_protectCooldownTime||!m_attackZone.TargetDetected)
            {
                m_protectCooldown = 0;
                m_protecting = false;
                m_damagable.Invinsible = false;
            }
        }
        m_anim.SetBool(m_HashProtect,m_protecting);
        base.Update();
    }

    protected override void FixedUpdate()
    {
        if(!m_protecting)
        {
            base.FixedUpdate();
        }
    }

    public override void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);
        m_protecting = true;
        m_damagable.Invinsible = true;
        m_waiting = false;
    }
}
