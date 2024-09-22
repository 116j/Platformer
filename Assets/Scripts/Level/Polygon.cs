using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon
{
    List<Vector3Int> m_tilePositions = new List<Vector3Int>();
    List<Vector3Int> m_ground = new List<Vector3Int>();


    public void DrawTiles()
    {
        TileEditor.Instance.SetTiles(m_tilePositions);
    }

    public void DrawTilesWithAnalog(TilePlaceAnalog analog)
    {
        TileEditor.Instance.SetTiles(m_tilePositions, analog);
    }

    public void ClearTiles()
    {
        TileEditor.Instance.ClearTiles(m_tilePositions);
    }

    public void AddTiles(int height, int width, Vector3Int startPosition)
    {
        for (int i = 1; i <= height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3Int pos = new Vector3Int(startPosition.x + j, startPosition.y + i);
                if (!m_tilePositions.Contains(pos))
                {
                    m_tilePositions.Add(pos);
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

    public List<Vector3Int> Ground()=>m_ground;

    public bool ContainsTile(Vector3Int tilePos)
    {
        return m_tilePositions.Contains(tilePos);
    }

}
