using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnviromentObject : MonoBehaviour, IMetrics
{
    BoxCollider2D m_col;

    private void Awake()
    {
        m_col = GetComponent<BoxCollider2D>();
    }

    public float GetHeight()
    {
        return m_col.size.y;
    }

    public float GetRightBorder()
    {
        return m_col.size.x / 2;
    }

    public float GetLeftBorder()
    {
        return -m_col.size.x / 2;
    }

    public void SetOffset()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + m_col.size.y / 2 + 1);
    }

    public Vector3 GetOffset()
    {
        return new Vector3(0, m_col.size.y / 2+1);
    }

    public float GetWidth()
    {
        return m_col.size.x;
    }

}
