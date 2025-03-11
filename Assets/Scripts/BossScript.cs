using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : WalkEnemy
{
    [SerializeField]
    DetectZone m_roarZone;

    readonly int m_HashRoar = Animator.StringToHash("Roar");

    float m_roarRecoverTime = 5f;
    float m_roarTimer;
    bool m_roarRecovering = false;

    protected override void FixedUpdate()
    {
        if (!m_dead)
        {
            if (m_roarRecovering)
            {
                m_roarTimer += Time.fixedDeltaTime;
                if(m_roarTimer >= m_roarRecoverTime)
                {
                    m_roarTimer = 0;
                    m_roarRecovering=false;
                }
            }

            if (m_roarZone.TargetDetected&&!m_roarRecovering)
            {
                m_attackScript.EnableAttack = false;
                m_anim.SetTrigger(m_HashRoar);
                m_roarRecovering = true;
            }
            else
            {
                base.FixedUpdate();
            }
        }
    }
}
