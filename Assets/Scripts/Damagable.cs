using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Damagable : MonoBehaviour
{
    [SerializeField]
    float m_health = 6f;
    [SerializeField]
    float m_recoverTime = 2f;
    [SerializeField]
    MonoBehaviour m_receiver;

    bool m_dead = false;
    bool m_recovering = false;

    float m_recoverTimer = 0f;

    private void Update()
    {
        if (m_recovering)
        {
            m_recoverTimer += Time.deltaTime;
            if (m_recoverTime >= m_recoverTimer)
            {
                m_recovering = false;
                m_recoverTimer = 0f;
            }
        }
    }


    public void ApplyDamage(float damage)
    {
        if (m_dead || m_recovering) return;

        m_recovering = true;
        m_health -= damage;
        if (m_health <= 0)
        {
            (m_receiver as IBoolReceiver).ReceiveBool(true);
            m_dead = true;
        }

        (m_receiver as IBoolReceiver).ReceiveBool(false);
    }
}
