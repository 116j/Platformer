using UnityEngine;

public class MoveBounds : MonoBehaviour
{
    float m_cameraPreviousX;
    // Start is called before the first frame update
    void Start()
    {
        m_cameraPreviousX = Camera.main.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Camera.main.transform.position.x - m_cameraPreviousX) * Vector3.right;
        m_cameraPreviousX = Camera.main.transform.position.x;
    }
}
