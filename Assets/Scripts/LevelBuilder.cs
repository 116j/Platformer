using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField]
    Transform m_startPosition;
    [SerializeField]
    Transform m_endPosition;

    [SerializeField]
    Vector2Int m_tilePositionMin;
    [SerializeField]
    Vector2Int m_tilePositionMax;

    TileEditor m_editor;   

    // Start is called before the first frame update
    void Start()
    {
        m_editor = transform.GetComponent<TileEditor>();     
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
