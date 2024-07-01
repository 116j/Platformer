using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    Camera m_cam;
    Transform m_followTarget;

    public Vector3 StartPosition { get; private set; }
    float m_offset = 0f;
    float m_startZ;
    //Distance between camera and obj start pos
    Vector3 CameraMoveSinceStart => m_cam.transform.position - StartPosition;
    // z distance between obj and target
    float ZDistanceFromTarget => transform.position.z - m_followTarget.position.z;
    // If obj is in front of target, use near clip plane, if is behind - use far clip plane
    float ClippingPlane => m_cam.transform.position.z + (ZDistanceFromTarget > 0 ? m_cam.farClipPlane : m_cam.nearClipPlane);
    //The further the obj from target, the faster it will move
    float PallaraxFactor => Mathf.Abs(ZDistanceFromTarget) / ClippingPlane;
    // Start is called before the first frame update
    void Start()
    {
        m_cam = Camera.main;
        m_followTarget = GameObject.FindGameObjectWithTag("Player").transform;

        StartPosition = transform.position;
        m_startZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = StartPosition - Vector3.right*m_offset + CameraMoveSinceStart * PallaraxFactor;
        transform.position = new Vector3(newPosition.x, transform.position.y, m_startZ);
    }

    public void SetStartPosition(float offset)
    {
        m_offset = offset;
    }
}
