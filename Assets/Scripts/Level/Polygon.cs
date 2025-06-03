using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Polygon
{
    HashSet<Vector3Int> m_tilePositions = new HashSet<Vector3Int>();
    HashSet<Vector3Int> m_ground = new HashSet<Vector3Int>();
    Vector3Int m_start;


    public Polygon(Vector3Int start)
    {
        m_start = start;
    }


    public void DrawTiles(TileEditor editor, System.Action<HashSet<Vector3Int>> callback, bool isInitial)
    {
        editor.SetTiles(m_tilePositions,()=>callback?.Invoke(m_ground),isInitial);
    }

    public void ClearTiles(TileEditor editor, bool async)
    {
        editor.ClearTiles(m_tilePositions, async);
    }
    /// <summary>
    /// add tiles from startPosition to (startPosition.x+width,startPosition.y-height)
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <param name="startPosition"></param>
    public void AddTiles(int height, int width, Vector3Int startPosition, bool ground = true)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3Int pos = new Vector3Int(startPosition.x + j, startPosition.y - i);
                if (!m_tilePositions.Contains(pos))
                {
                    m_tilePositions.Add(pos);
                }
                if (ground && m_start.y < pos.y && m_start.x == pos.x)
                {
                    Debug.Log(startPosition);
                }
            }
        }
    }

    public void AddTile(Vector3Int tilePos)
    {
        if (!m_tilePositions.Contains(tilePos))
        {
            m_tilePositions.Add(tilePos);
        }
    }

    public void AddGround(int width, Vector3Int startPosition)
    {
        for (int i = 0; i < width; i++)
        {
            Vector3Int pos = new Vector3Int(startPosition.x + i, startPosition.y);
            if (!m_ground.Contains(pos))
            {
                m_ground.Add(pos);
            }
        }
    }

    public HashSet<Vector3Int> Ground() => m_ground;

    public bool ContainsTile(Vector3Int tilePos)
    {
        return m_tilePositions.Contains(tilePos);
    }

}
