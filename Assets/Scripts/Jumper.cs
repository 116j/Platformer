using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : Trap
{
    [SerializeField]
    float m_jumpPower = 15f;
    GameObject m_target;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_anim.SetTrigger("Attack");
            m_target = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_anim.ResetTrigger("Attack");
            m_target = null;
        }
    }

    public void Jump()
    {
        m_target.GetComponent<PlayerController>().Jump();
        m_target.GetComponent<Rigidbody2D>().AddForce(transform.up * m_jumpPower, ForceMode2D.Impulse);
    }
}
