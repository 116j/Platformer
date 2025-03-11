using UnityEngine;

public class Clouds : MonoBehaviour
{
    [SerializeField]
    float m_speed = 0.1f;

    // Update is called once per frame
    void Update()
    {
        transform.position -= m_speed * Time.deltaTime * Vector3.right;

    }
}
