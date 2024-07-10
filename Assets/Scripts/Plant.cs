using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    protected Animator m_anim;
    Rigidbody2D m_rb;
    AttackScript m_attack;

    protected readonly int m_HashFalling = Animator.StringToHash("Falling");
    protected readonly int m_HashGrew = Animator.StringToHash("Grew");

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_anim = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody2D>();
        m_attack = GetComponent<AttackScript>();
        m_attack.EnableAttack = false;

        m_rb.velocity = new Vector2(0f, -7f);
        m_anim.SetBool(m_HashFalling, true);
    }

    void Update()
    {
        if (!m_attack.EnableAttack&&m_anim.GetBool(m_HashGrew))
        {
            m_attack.EnableAttack = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ground"))
        {
            m_anim.SetBool(m_HashFalling, false);
            m_rb.velocity = Vector2.zero;
        }
    }
}
