using UnityEngine;

public class TouchingCheck : MonoBehaviour
{
    [SerializeField]
    ContactFilter2D m_groundCastFilter;
    [SerializeField]
    ContactFilter2D m_wallCastFilter;

    Collider2D m_col;

    //distance to detect wall
    readonly float m_wallHitDist = 0.1f;
    //distance to detect ground
    readonly float m_groundHitDist = 0.05f;

    RaycastHit2D[] m_colHits = new RaycastHit2D[5];

    // Start is called before the first frame update
    void Start()
    {
        m_col = GetComponent<Collider2D>();
    }

    public bool IsGrounded()
    {
        return m_col.Cast(-transform.up, m_groundCastFilter, m_colHits, m_groundHitDist) > 0;
    }

    public bool IsWalls()
    {
        return m_col.Cast(transform.right, m_wallCastFilter, m_colHits, m_wallHitDist) > 0;
    }
}
