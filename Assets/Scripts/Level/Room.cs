using System.Collections.Generic;
using UnityEngine;

public class Room
{
    protected Vector3Int m_enterPosition;
    protected Vector3Int m_exitPosition;

    List<Polygon> m_polygons = new List<Polygon>();
    List<GameObject> m_enviroment = new List<GameObject>();
    Room m_transition;

    int m_lastWidthPoint;

    readonly int m_minHeight = 5;
    readonly int m_minWidth = 10;

    public Room(Vector3Int end, Room transition)
    {
        m_exitPosition = end;
        m_transition = transition;
        m_enterPosition = m_transition.m_exitPosition;
        m_lastWidthPoint = m_enterPosition.x;

        MakePolygon(m_minHeight, m_enterPosition - m_minHeight * Vector3Int.up);
        if (transition.GetRoomHeight() > m_minHeight)
            m_polygons[0].AddTiles(transition.GetRoomHeight() - m_minHeight, m_minHeight, new Vector3Int(m_enterPosition.x, transition.m_enterPosition.y));
    }

    public Room(Vector3Int strat, Vector3Int end)
    {
        m_enterPosition = strat;
        m_exitPosition = end;
        m_lastWidthPoint = strat.x;
    }

    public Vector3Int GetEndPosition() => m_exitPosition;

    public Vector3Int GetStartPosition() => m_enterPosition;

    public List<Vector3Int> GetGround() => m_polygons[0].Ground();

    public int GetRoomHeight() => m_exitPosition.y - m_enterPosition.y;

    public void Clear()
    {
        m_transition?.Clear();
        foreach (Polygon poly in m_polygons)
        {
            poly.ClearTiles();
        }
        foreach (var obj in m_enviroment)
        {
            Object.Destroy(obj);
        }
    }

    public void MakePolygon(int width, Vector3Int startPos, bool addGround = true)
    {
        Polygon polygon = new Polygon();
        polygon.AddTiles(m_minHeight, width, startPos);
        if (addGround)
            polygon.AddGround(width, startPos + Vector3Int.up * m_minHeight);
        m_polygons.Add(polygon);
    }


    public void CreateElevationOrLowland(int height, int width, Vector3Int startPos)
    {
        if (height > 0)
        {
            m_polygons[0].AddTiles(m_minHeight, System.Math.Clamp(m_minWidth, 0, width), new Vector3Int(startPos.x, startPos.y - m_minHeight));
            m_polygons[0].AddTiles(height, System.Math.Clamp(m_minWidth, 0, width), startPos);
            m_polygons[0].AddTiles(m_minHeight, System.Math.Clamp(width - m_minWidth, 0, int.MaxValue), new Vector3Int(startPos.x + System.Math.Clamp(m_minWidth, 0, width), startPos.y + height - m_minHeight));
            m_lastWidthPoint = startPos.x + System.Math.Clamp(m_minWidth, 0, width);
        }
        else
        {
            m_polygons[0].AddTiles(m_minHeight, width, new Vector3Int(startPos.x, startPos.y + height - m_minHeight));
            m_polygons[0].AddTiles(-height, System.Math.Clamp(m_minWidth, m_minHeight, System.Math.Clamp(startPos.x - m_lastWidthPoint, m_minHeight, int.MaxValue)), new Vector3Int(startPos.x - System.Math.Clamp(m_minWidth, m_minHeight, System.Math.Clamp(startPos.x - m_lastWidthPoint, m_minHeight, int.MaxValue)), startPos.y + height - m_minHeight));
        }
        m_polygons[0].AddGround(width, startPos + Vector3Int.up * height);
        m_exitPosition = new Vector3Int(m_exitPosition.x, m_exitPosition.y + height);
    }

    public void AddTiles(int height, int width, Vector3Int startPos)
    {
        m_polygons[0].AddTiles(height, width, startPos - Vector3Int.up * height);
        m_polygons[0].AddGround(width, startPos);
    }

    public void CreateSlope(int height, int straightSection, Vector3Int startPos)
    {
        for (int j = 1; j <= height; j++)
        {
            for (int i = startPos.x + j; i < startPos.x + height * 2 + straightSection - j; i++)
            {
                m_polygons[0].AddTile(new Vector3Int(i, startPos.y + j));
            }
        }
        m_polygons[0].AddGround(straightSection - 2, new Vector3Int(startPos.x + height + 1, startPos.y + height));
        m_polygons[0].AddGround(m_minHeight, new Vector3Int(startPos.x + height * 2 + straightSection, startPos.y));

        m_polygons[0].AddTiles(m_minHeight, height * 2 + straightSection + m_minHeight, startPos - Vector3Int.up * m_minHeight);
    }

    public void CreateLedge(Vector3Int pos)
    {
        int height = System.Math.Abs(pos.y - (pos.y > m_enterPosition.y ? m_enterPosition.y : m_exitPosition.y)) + m_minHeight;
        Polygon polygon = new Polygon();
        for (int j = 0; j < height; j++)
        {
            polygon.AddTile(new Vector3Int(pos.x, pos.y - j));
        }

        m_polygons.Add(polygon);
    }

    public void DrawTiles(TilePlaceAnalog analog = null)
    {
        if (analog == null)
        {
            foreach (var poly in m_polygons)
            {
                poly.DrawTiles();
            }
        }
        else
        {
            foreach (var poly in m_polygons)
            {
                poly.DrawTilesWithAnalog(analog);
            }
        }
    }

    public void AddEnviromentObject(GameObject env)
    {
        m_enviroment.Add(env);
    }
}
