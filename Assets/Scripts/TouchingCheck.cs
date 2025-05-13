using UnityEngine;

public class TouchingCheck : MonoBehaviour
{
    [SerializeField]
    ContactFilter2D m_groundCastFilter;
    [SerializeField]
    ContactFilter2D m_wallCastFilter;
    [SerializeField]
    ContactFilter2D m_slopeCastFilter;
    [SerializeField]
    ContactFilter2D m_stuckCastFilter;

    Collider2D m_col;

    //distance to detect wall
    readonly float m_wallHitDist = 0.1f;
    //distance to detect ground
    readonly float m_groundHitDist = 0.05f;
    //distance to detect slope
    readonly float m_slopeHitDist = 0.2f;
    readonly float m_stuckHitDist = 0.6f;

    RaycastHit2D[] m_rayHits = new RaycastHit2D[5];
    Collider2D[] m_colHits = new Collider2D[5];

    // Start is called before the first frame update
    void Start()
    {
        m_col = GetComponent<Collider2D>();
    }

    public bool IsWallStuckLeft()
    {
        return m_col.Cast(transform.up, m_stuckCastFilter, m_rayHits, m_stuckHitDist) > 0&&
            m_col.Cast(-transform.right, m_stuckCastFilter, m_rayHits, m_stuckHitDist) > 0 &&
            m_col.Cast(transform.up, m_stuckCastFilter, m_rayHits, m_stuckHitDist) > 0 &&
            Physics2D.OverlapBoxNonAlloc(transform.position + Vector3.right * 2, m_col.bounds.size, transform.eulerAngles.z, m_colHits, m_stuckCastFilter.layerMask.value) == 0;
    }
    public bool IsWallStuckRight()
    {
        return m_col.Cast(transform.up, m_stuckCastFilter, m_rayHits, m_stuckHitDist) > 0&&
            m_col.Cast(-transform.right, m_stuckCastFilter, m_rayHits, m_stuckHitDist) > 0 &&
           m_col.Cast(transform.up, m_stuckCastFilter, m_rayHits, m_stuckHitDist) > 0 &&
           Physics2D.OverlapBoxNonAlloc(transform.position - Vector3.right * 2, m_col.bounds.size, transform.eulerAngles.z, m_colHits, m_stuckCastFilter.layerMask.value) == 0;
    }
    public bool IsWallStuckUp()
    {
        return m_col.Cast(transform.up, m_stuckCastFilter, m_rayHits, m_stuckHitDist) > 0&&
            m_col.Cast(-transform.right, m_stuckCastFilter, m_rayHits, m_stuckHitDist) > 0 &&
           m_col.Cast(transform.up, m_stuckCastFilter, m_rayHits, m_stuckHitDist) > 0 &&
           Physics2D.OverlapBoxNonAlloc(transform.position - Vector3.up * 2, m_col.bounds.size, transform.eulerAngles.z, m_colHits, m_stuckCastFilter.layerMask.value) == 0;
    }

    public bool IsGroundStuck()
    {
        return m_col.Cast(-transform.up, m_stuckCastFilter, m_rayHits, m_stuckHitDist) > 0 &&
            m_col.Cast(transform.right, m_stuckCastFilter, m_rayHits, m_wallHitDist) > 0 &&
            m_col.Cast(-transform.right, m_stuckCastFilter, m_rayHits, m_wallHitDist) > 0&&
            Physics2D.OverlapBoxNonAlloc(transform.position + Vector3.up * 0.6f, m_col.bounds.size, transform.eulerAngles.z, m_colHits, m_stuckCastFilter.layerMask.value) == 0; ;
    }

    public bool IsGrounded()
    {
        return m_col.Cast(-transform.up, m_groundCastFilter, m_rayHits, m_groundHitDist) > 0;
    }

    public bool IsWalls()
    {
        return m_col.Cast(transform.right, m_wallCastFilter, m_rayHits, m_wallHitDist) > 0;
    }

    public bool IsSlopeUp()
    {
        return m_col.Cast(transform.right, m_slopeCastFilter, m_rayHits, m_slopeHitDist) > 0 &&
        m_col.Cast(-transform.up, m_groundCastFilter, m_rayHits, m_groundHitDist) > 0;
    }

    public bool IsSlopeDown()
    {
        return m_col.Cast(-transform.right, m_slopeCastFilter, m_rayHits, m_slopeHitDist) > 0 &&
        !Physics2D.Raycast(new Vector2(m_col.bounds.max.x, m_col.bounds.min.y), -transform.up, m_groundHitDist, m_groundCastFilter.layerMask) &&
        m_col.Cast(-transform.up, m_groundCastFilter, m_rayHits, m_groundHitDist) > 0;
    }
}
