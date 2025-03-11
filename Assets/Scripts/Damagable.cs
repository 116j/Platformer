using UnityEngine;
using UnityEngine.Events;

public class Damagable : MonoBehaviour
{
    [SerializeField]
    int m_maxHealth = 4;
    [SerializeField]
    int m_health = 4;
    [SerializeField]
    float m_recoverTime = 2f;
    [SerializeField]
    UnityEvent<int> m_receiver;

    bool m_dead = false;
    bool m_recovering = false;

    float m_recoverTimer = 0f;
    float m_freezeTime = 0f;

    public bool Invinsible { get; set; } = false;
    public bool Freezed { get; set; } = false;

    private void Update()
    {
        if (m_recovering)
        {
            m_recoverTimer += Time.deltaTime;
            if (m_recoverTimer >= m_recoverTime)
            {
                m_recovering = false;
                m_recoverTimer = 0f;
                Invinsible = false;
            }
        }
        else if (Freezed)
        {
            m_freezeTime -= Time.deltaTime;
            if (m_freezeTime <= 0)
            {
                Freezed = false;
                GetComponent<PlayerInput>().LockInput();
            }
        }
    }

    public void ApplyDamage(int damage)
    {
        if (m_dead) return;

        m_recovering = true;
        m_health -= damage;

        m_receiver.Invoke(-damage);
        if (m_health <= 0)
        {
            m_dead = true;
            m_receiver.Invoke(0);
        }
    }

    public void Freeze(float time)
    {
        Freezed = true;
        GetComponent<PlayerInput>().LockInput();
        m_freezeTime = time;
    }

    public void ApplyHealth(int healPoints)
    {
        if (m_health >= m_maxHealth)
            return;

        m_health += healPoints;
        m_receiver.Invoke(healPoints);
        if (m_health <= 0)
        {
            m_dead = false;
            m_receiver.Invoke(0);
        }
    }

    public void IncreaseHealth()
    {
        m_maxHealth++;
        ApplyHealth(1);
    }
}
