using UnityEngine;

public class Clouds : MonoBehaviour
{
    [SerializeField]
    float m_speed = 0.1f;

    readonly float m_resetValue = 18f;
    float m_offset = 0f;

    ParallaxEffect m_effect;

    // Start is called before the first frame update
    void Start()
    {
        m_effect = GetComponent<ParallaxEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        m_offset += m_speed * Time.deltaTime;
        transform.position -= Vector3.right * m_offset;
        m_effect.SetStartPosition(m_offset);
        if (m_offset >= m_resetValue)
        {
            transform.position += Vector3.right * m_offset;
            m_offset = 0f;
            m_effect.SetStartPosition(0);
        }
    }
}
