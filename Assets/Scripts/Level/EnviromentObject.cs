using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnviromentObject : MonoBehaviour, IMetrics
{
    Collider2D m_col;

    private void Awake()
    {
        m_col = GetComponent<Collider2D>();
    }

    public float GetHeight()
    {
        return m_col.bounds.size.y;
    }

    public float GetRightBorder()
    {
        return m_col.bounds.size.x / 2;
    }

    public float GetLeftBorder()
    {
        return -m_col.bounds.size.x / 2;
    }

    public void SetOffset()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + m_col.bounds.size.y / 2 + 1);
    }

    public Vector3 GetOffset()
    {
        return new Vector3(-m_col.bounds.size.x / 2, m_col.bounds.size.y / 2);
    }

    public float GetWidth()
    {
        return m_col.bounds.size.x;
    }

}
