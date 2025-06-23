using System.Collections.Generic;
using UnityEngine;

public enum BrickBehaviour
{
    None,
    Timer,
    OnExit,
    OnEnter
}

public class DestroyableBrick : MonoBehaviour
{
    [SerializeField]
    Trap m_brickPrefab;

    BrickBehaviour m_behaviour;

    SpriteRenderer m_renderer;
    Collider2D m_col;
    List<DestroyableBrick> m_group;

    float m_timer;
    float m_destroyTime = 0.3f;
    int m_brickNum;
    bool m_destroyed = false;

    private void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        m_col = GetComponent<Collider2D>();
    }

    public void SetBrickBehaviour(BrickBehaviour b, int tileNum, Sprite tile, List<DestroyableBrick> group)
    {
        m_behaviour = b;
        m_brickNum = tileNum;
        m_group = group;
        m_renderer.sprite = tile;

        m_group?.Add(this);
    }

    private void Update()
    {
        if (m_behaviour == BrickBehaviour.Timer && m_timer > 0 && !m_destroyed)
        {
            m_timer -= Time.deltaTime;
            if (m_timer <= 0)
            {
                m_timer = 0;
                DestroyBrick();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (m_behaviour == BrickBehaviour.OnEnter && !m_destroyed)
            {
                DestroyBrick();
            }
            else if (m_behaviour == BrickBehaviour.Timer && !m_destroyed)
            {
                m_timer = m_destroyTime;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (m_behaviour == BrickBehaviour.OnExit && !m_destroyed)
            {
                DestroyBrick();
            }
        }
    }

    void DestroyBrick()
    {
        foreach (var brick in m_group)
        {
            if (!brick.m_destroyed)
            {
                brick.m_destroyed = true;
                brick.m_renderer.enabled = false;
                brick.m_col.isTrigger = true;
                Trap destroyable = Instantiate(brick.m_brickPrefab, brick.transform.position, Quaternion.identity);
                destroyable.SetTrap(brick.m_brickNum);
            }
        }
    }

    public void Restart()
    {
        m_col.isTrigger = false;
        m_renderer.enabled = true;
        m_timer = 0;
        m_destroyed = false;
    }
}
