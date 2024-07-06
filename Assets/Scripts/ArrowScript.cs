using UnityEngine;

public class ArrowScript : AttackListener
{
    readonly float m_lifeTime = 5f;

    float m_life = 0f;

    // Update is called once per frame
    void Update()
    {
        m_life += Time.deltaTime;
        if (m_life >= m_lifeTime)
        {
            Destroy(gameObject);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Player"))
            Destroy(gameObject);
    }
}
