using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellShooter : MonoBehaviour
{
    [SerializeField]
    float m_shootPower;
    [SerializeField]
    GameObject m_shell;
    [SerializeField]
    Transform m_shellSpawn;


    public void SpawnShell()
    {
        GameObject shell = Instantiate(m_shell, m_shellSpawn.position, m_shellSpawn.rotation);
        shell.name = m_shell.name;
        shell.transform.localScale = m_shellSpawn.localScale;
        shell.GetComponent<Rigidbody2D>().velocity = shell.transform.right *m_shootPower;
    }
}
