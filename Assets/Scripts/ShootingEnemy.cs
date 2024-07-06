using UnityEngine;

public class ShootingEnemy : WalkEnemy
{
    [SerializeField]
    DetectZone m_shootZone;
    [SerializeField]
    GameObject m_arrow;
    [SerializeField]
    Transform m_arrowSpawnLocation;

    readonly int m_HashShoot = Animator.StringToHash("Shoot");
    readonly int m_HashShootNum = Animator.StringToHash("ShootNum");

    bool m_canShoot = true;

    readonly float m_shootCooldownTime = 2f;
    readonly int m_shootNum = 2;
    readonly float m_shootPower = 7f;

    float m_shootCooldown;

    protected override void Update()
    {
        if (!m_canShoot)
        {
            m_shootCooldown += Time.deltaTime;
            if (m_shootCooldown > m_shootCooldownTime)
            {
                m_shootCooldown = 0f;
                m_canShoot = true;
            }
        }

        base.Update();
    }

    protected override void FixedUpdate()
    {
        if (m_shootZone.TargetDetected && !m_detectZone.TargetDetected)
        {
            Shoot();
        }
        else
        {
            base.FixedUpdate();
        }
    }

    void Shoot()
    {
        m_speed = 0f;
        if (!m_canShoot) return;

        m_canShoot = false;
        m_anim.SetTrigger(m_HashShoot);
        m_anim.SetInteger(m_HashShootNum, Random.Range(0, m_shootNum));
    }

    public void SpawnArrow()
    {
        GameObject arrow = Instantiate(m_arrow, m_arrowSpawnLocation.position, m_arrowSpawnLocation.rotation);
        arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(m_shootPower * m_currentDir, 0f);
    }
}
