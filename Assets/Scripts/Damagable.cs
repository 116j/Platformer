using UnityEngine;
using UnityEngine.Events;

public class Damagable : MonoBehaviour
{
    [SerializeField]
    int m_health = 6;
    [SerializeField]
    float m_recoverTime = 2f;
    [SerializeField]
    UnityEvent<int> m_receiver;

    bool m_dead = false;
    bool m_recovering = false;

    float m_recoverTimer = 0f;

    public bool Invinsible { get; set; } = false;

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


    public void ApplyDamage(int damage)
    {
        if (m_dead || m_recovering) return;

        m_recovering = true;
        m_health -= damage;

        m_receiver.Invoke(damage);
        if (m_health <= 0)
        {
            m_dead = true;
            m_receiver.Invoke(0);
        }
    }
}
