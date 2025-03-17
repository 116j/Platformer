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

    bool m_closeAttack = false;
    float m_attackChance = 0.3f;
    float m_attackZoneOffsetX = 0.81308f;
    float m_baseAttackZoneOffsetX = 2.784698f;
    float m_closeAttackCooldown = 5f;
    float m_closeAttackCooldownTimer;
    bool m_canCloseAttack = true;

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

            if (!m_canCloseAttack)
            {
                m_closeAttackCooldownTimer += Time.fixedDeltaTime;
                if (m_closeAttackCooldownTimer >= m_closeAttackCooldown)
                {
                    m_closeAttackCooldownTimer = 0f;
                    m_canCloseAttack = true;
                }
            }

            if (m_roarZone.TargetDetected&&!m_roarRecovering)
            {
                m_attackScript.EnableAttack = false;
                m_anim.SetTrigger(m_HashRoar);
                m_roarRecovering = true;
            }
            else if (m_canCloseAttack&&!m_closeAttack&&Random.value <= m_attackChance)
            {
                m_closeAttack = true;
                m_col.isTrigger = true;
                var zone = m_attackZone.GetComponent<BoxCollider2D>();
                zone.offset = new Vector2(m_attackZoneOffsetX, zone.offset.y);
            }
            else if (m_closeAttack && m_attackZone.TargetDetected&&
                (m_currentDir==1?
                (m_attackZone.TargetLocation.x- m_attackZone.GetComponent<BoxCollider2D>().bounds.max.x): 
                (m_attackZone.GetComponent<BoxCollider2D>().bounds.min.x-m_attackZone.TargetLocation.x)) <= 0.1f)
            {
                if (!m_attackScript.EnableAttack)
                {
                    m_attackScript.EnableAttack = true;
                    m_speed = 0f;
                    m_rb.velocity = Vector2.zero;
                    m_waiting = false;
                }
                m_anim.SetInteger(m_HashAttackNum, 5);
            }
            else if (m_closeAttack && m_attackZone.TargetDetected)
            {
                Chase();
            }
            else
            {
                base.FixedUpdate(); 
            }
        }
    }

    public void ResetColliders()
    {
        m_canCloseAttack = false;
        m_col.isTrigger = false;
        m_closeAttack = false;
        var zone = m_attackZone.GetComponent<BoxCollider2D>();
        zone.offset = new Vector2(m_baseAttackZoneOffsetX, zone.offset.y);
    }
}
