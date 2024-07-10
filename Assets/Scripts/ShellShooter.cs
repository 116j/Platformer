using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellShooter : MonoBehaviour
{
    [SerializeField]
    string m_shootDirectionString;
    [SerializeField]
    float m_shootPower;
    [SerializeField]
    GameObject m_shell;
    [SerializeField]
    Transform m_shellSpawn;

    Animator m_anim;

    void Start()
    {
        m_anim = GetComponent<Animator>();
        if (!string.IsNullOrEmpty(m_shootDirectionString))
        {
            m_anim.SetBool(Animator.StringToHash(m_shootDirectionString), true);
        }
    }

    public void SpawnShell()
    {
        GameObject shell = Instantiate(m_shell, m_shellSpawn.position, m_shellSpawn.rotation);
        shell.transform.localScale = m_shellSpawn.localScale;
        shell.GetComponent<Rigidbody2D>().velocity = shell.transform.right *m_shootPower;
    }
}
