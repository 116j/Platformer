using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField]
    float m_parallaxMultiplier;

    Transform m_cam;

    Vector3 m_lastCameraPosition;
    float m_textureUnitSizeX;
    Vector3 m_deltaCamMove;

    void Start()
    {
        m_cam = Camera.main.transform;
        m_lastCameraPosition = m_cam.position;

        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        m_textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        m_deltaCamMove = m_cam.position - m_lastCameraPosition;
        transform.position += m_deltaCamMove.x * m_parallaxMultiplier * Vector3.left;
        m_lastCameraPosition = m_cam.position;

        if (Mathf.Abs(m_cam.position.x - transform.position.x) >= m_textureUnitSizeX)
        {
            float offsetX = (m_cam.position.x - transform.position.x) % m_textureUnitSizeX;
            transform.position = new Vector3(m_cam.position.x + offsetX, transform.position.y);
        }

    }
}
