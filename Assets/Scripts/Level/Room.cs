using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room
{
    protected Vector3Int m_startPosition;
    protected Vector3Int m_endPosition;

    List<Polygon> m_polygons = new List<Polygon>();
    List<GameObject> m_enviroment = new List<GameObject>();
    Room m_prevTransition;
    Room m_nextTransition;

    //last point of additional width in lowland, elevation or beginning 
    Vector3Int m_lastWidthPoint;
    // last start of elevation ot lowland
    int m_lastElevationPoint;
    // lowest y point of the level
    int m_lowestPoint;
    int m_cameraBoundsStart;
    int m_roomHeight = 0;

    int m_transitionLeftPoint;
    int m_transitionRightPoint;
    readonly int m_minHeight = 6;
    readonly int m_minWidth = 12;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="end"></param>
    /// <param name="startwWidth">width of start straight section</param>
    /// <param name="transition">transition between this and previous rooms</param>
    public Room(Vector3Int end, int startwWidth, Room transition)
    {
        m_endPosition = end;
        m_prevTransition = transition;
        m_startPosition = m_prevTransition.m_endPosition;
        m_lastWidthPoint = m_startPosition - Vector3Int.up * m_minHeight;
        m_roomHeight = m_minHeight;
        m_lastElevationPoint = m_startPosition.x;
        m_cameraBoundsStart = m_lowestPoint = m_startPosition.y - m_roomHeight;

        MakePolygon(startwWidth, m_startPosition);
        // adds tiles so the end of the level can't be seen if transition is increasing
        if (GetTransitionHeight() > 0)
        {
            m_polygons[0].AddTiles(m_prevTransition.m_transitionRightPoint, Mathf.Min(startwWidth, m_minWidth), m_startPosition - Vector3Int.up * m_minHeight);
            m_roomHeight += m_prevTransition.m_transitionRightPoint;
            m_lastWidthPoint += new Vector3Int(Mathf.Min(startwWidth, m_minWidth), -m_prevTransition.m_transitionRightPoint);
            m_lowestPoint = m_startPosition.y - m_roomHeight;
        }
    }

    public Room(Vector3Int start, Vector3Int end, Room transition = null)
    {
        m_prevTransition = transition;
        m_startPosition = start;
        m_endPosition = end;
        m_lastElevationPoint = start.x;
        m_roomHeight = Mathf.Abs(end.y - start.y) + m_minHeight;
        m_transitionLeftPoint = m_transitionRightPoint = Mathf.Abs(start.y - end.y);
        m_lowestPoint = m_cameraBoundsStart = (end.y > start.y ? end.y : start.y) - m_roomHeight;
        m_lastWidthPoint = start - Vector3Int.up * m_minHeight;
    }

    public Vector3Int GetEndPosition() => m_endPosition;

    public Vector3Int GetStartPosition() => m_startPosition;
    /// <summary>
    /// if the point is lower than the lowest point - change the lowest point, the camera bounds start and add the room height
    /// </summary>
    /// <param name="point"></param>
    void SetLowestPoint(int point)
    {
        if (m_lowestPoint > point - m_minHeight)
        {
            m_roomHeight += m_lowestPoint - point + m_minHeight;
            m_lowestPoint = point - m_minHeight;
        }

        if (m_cameraBoundsStart > point - m_minHeight)
        {
            m_cameraBoundsStart = point - m_minHeight;
        }
    }
    /// <summary>
    /// if the point is higher than the highest point - increase the room height
    /// </summary>
    /// <param name="point"></param>
    void SetHighestPoint(int point)
    {
        if (m_lowestPoint + m_roomHeight < point)
        {
            m_roomHeight = point - m_lowestPoint;
        }
    }

    int SetTransitionSidePoint(Vector3Int end, Vector3Int pos, int value)
    {
        return end.x - pos.x <= m_minWidth && value == 0 ? Mathf.Abs(end.y - pos.y) :
            end.x - pos.x > m_minWidth && value != 0 ? 0 : value;
    }

    public void SetEndPosition(Vector3Int end)
    {
        m_transitionLeftPoint = SetTransitionSidePoint(end, m_startPosition, m_transitionLeftPoint);

        m_endPosition = end;

        SetLowestPoint(end.y);
        SetHighestPoint(end.y);
    }


    public void SetStartPosition(Vector3Int start)
    {
        m_transitionRightPoint = SetTransitionSidePoint(m_endPosition, start, m_transitionRightPoint);

        m_startPosition = start;

        SetLowestPoint(start.y);
        SetHighestPoint(start.y);
    }

    public Room GetNextTransition() => m_nextTransition;

    public Room GetPreviousTransition() => m_prevTransition;

    public List<Vector3Int> GetGround() => m_polygons.Count == 1 ? m_polygons[0].Ground().ToList() : m_polygons.SelectMany(p => p.Ground()).ToList();

    public int GetTransitionRightHeight() => m_transitionRightPoint;

    public int GetTransitionLeftHeight() => m_transitionLeftPoint;

    public int GetTransitionHeight() => m_prevTransition.m_endPosition.y - m_prevTransition.m_startPosition.y;

    public int GetRoomHighestPoint() => m_lowestPoint + m_roomHeight;

    public int GetRoomCameraHeight() => m_roomHeight - m_cameraBoundsStart + m_lowestPoint;

    public void SetCameraBounds()
    {
        CameraBounds.Instance.SetHeight(new Vector3(m_startPosition.x, GetRoomHighestPoint()), GetRoomCameraHeight());
    }

    public void SetTransitionCameraBounds()
    {
        m_prevTransition.SetCameraBounds();
    }
    /// <summary>
    /// Deletes tiles and enviroment objects
    /// </summary>
    public void Clear(TileEditor editor, bool async = false)
    {
        m_nextTransition?.Clear(editor, async);
        foreach (var obj in m_enviroment)
        {
            Object.Destroy(obj);
        }
        foreach (Polygon poly in m_polygons)
        {
            poly.ClearTiles(editor, async);
        }
    }

    public void ClearGrid()
    {
        m_polygons.Clear();
    }

    public void Restart()
    {
        m_nextTransition?.Restart();
        WalkEnemy enemy;
        MovingPlatform platform;
        Cat cat;
        DestroyableBrick brick;
        foreach (var obj in m_enviroment)
        {
            if (obj != null)
            {
                if (obj.TryGetComponent(out enemy))
                {
                    enemy.Reset();
                }
                if (obj.TryGetComponent(out platform))
                {
                    platform.Restart();
                }
                if (obj.TryGetComponent(out cat))
                {
                    cat.Reset();
                }
                if (obj.TryGetComponent(out brick))
                {
                    brick.Restart();
                }
            }
        }
    }
    /// <summary>
    /// Creates polygon
    /// </summary>
    /// <param name="width"></param>
    /// <param name="startPos"></param>
    /// <param name="addGround">if want to add landscape - default = true </param>
    public void MakePolygon(int width, Vector3Int startPos, bool addGround = true)
    {
        Polygon polygon = new Polygon(m_startPosition);
        m_lowestPoint = startPos.y - m_minHeight;
        polygon.AddTiles(m_minHeight, width, startPos);
        if (addGround)
            polygon.AddGround(width, startPos);
        m_polygons.Add(polygon);
    }

    /// <summary>
    /// Add tiles for elevation or lowland
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <param name="startPos"></param>
    public void CreateElevationOrLowland(int height, int width, Vector3Int startPos)
    {
        if (height >= 0)
        {
            SetHighestPoint(startPos.y + height);
            // add height
            int w = Mathf.Min(m_minWidth, width);
            m_polygons[0].AddTiles(height + m_minHeight, w, startPos + height * Vector3Int.up);
            // add width
            m_polygons[0].AddTiles(m_minHeight, Mathf.Max(width - m_minWidth, 0), new Vector3Int(startPos.x + w, startPos.y + height));
            m_lastWidthPoint = new Vector3Int(startPos.x + w, startPos.y - m_minHeight);
        }
        else
        {
            SetLowestPoint(startPos.y + height);
            // add lowland tiles
            m_polygons[0].AddTiles(m_minHeight, width, new Vector3Int(startPos.x, startPos.y + height));
            //add tiles between previous lowland or elevation and new lowland
            int w;
            //if previous lowland or elevation width's last point is the same as start, add tiles below previous lowland or elevation
            if (startPos.x == m_lastWidthPoint.x)
            {
                w = Mathf.Min(m_minWidth, startPos.x - m_lastElevationPoint);
            }
            // if previous lowland or elevation width's last point is farther then start, add tiles below previous lowland or elevation and bettween start and lowland or elevation width's last point
            else
            {
                w = Mathf.Min(m_minWidth, startPos.x - m_lastWidthPoint.x);
                m_polygons[0].AddTiles(-height, w, new Vector3Int(startPos.x - w, startPos.y - m_minHeight));
                w = Mathf.Min(m_minWidth - w, m_lastWidthPoint.x - m_lastElevationPoint);
            }

            m_polygons[0].AddTiles(m_lastWidthPoint.y - startPos.y - height + m_minHeight, w, new Vector3Int(m_lastWidthPoint.x - w, m_lastWidthPoint.y));
        }
        m_polygons[0].AddGround(width, startPos + Vector3Int.up * height);
        m_lastElevationPoint = startPos.x;
        m_endPosition = new Vector3Int(m_endPosition.x, m_endPosition.y + height);
    }
    /// <summary>
    /// Adds tiles
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <param name="startPos"></param>
    /// <param name="addGround">if add landscape on tiles - default = true </param>
    public void AddTiles(int height, int width, Vector3Int startPos, bool addGround = true)
    {
        m_polygons[m_polygons.Count - 1].AddTiles(height, width, startPos, addGround);
        if (addGround)
        {
            m_polygons[m_polygons.Count - 1].AddGround(width, startPos);
            SetLowestPoint(startPos.y + height);
            SetHighestPoint(startPos.y + height);
        }
    }
    /// <summary>
    /// Creates slope
    /// </summary>
    /// <param name="height">slope height</param>
    /// <param name="straightSection">slope width</param>
    /// <param name="startPos"></param>
    public void CreateSlope(int height, int straightSection, Vector3Int startPos)
    {
        for (int j = 1; j <= height; j++)
        {
            for (int i = startPos.x + j; i <= startPos.x + height * 2 + straightSection - j + 1; i++)
            {
                m_polygons[0].AddTile(new Vector3Int(i, startPos.y + j));
            }
        }
        SetHighestPoint(startPos.y + height);
        m_polygons[0].AddGround(1, startPos);
        m_polygons[0].AddGround(straightSection, new Vector3Int(startPos.x + height + 1, startPos.y + height));
        m_polygons[0].AddGround(m_minHeight, new Vector3Int(startPos.x + height * 2 + straightSection + 1, startPos.y));

        m_polygons[0].AddTiles(m_minHeight, height * 2 + straightSection + 1 + m_minHeight, startPos);
    }
    /// <summary>
    /// Create ledge for transition
    /// </summary>
    /// <param name="pos"></param>
    public void CreateLedge(Vector3Int pos)
    {
        int height = pos.y - (m_endPosition.y > m_startPosition.y ? m_startPosition.y : m_endPosition.y) + m_minHeight;
        Polygon polygon = new Polygon(m_startPosition);
        for (int j = 0; j < height; j++)
        {
            polygon.AddTile(new Vector3Int(pos.x, pos.y - j));
        }

        if (m_endPosition.x - pos.x <= m_minWidth && m_transitionLeftPoint != Mathf.Abs(m_startPosition.y - m_endPosition.y))
        {
            m_transitionRightPoint = Mathf.Abs(m_endPosition.y - pos.y);
        }
        if (pos.x - m_startPosition.x >= m_minWidth && m_transitionLeftPoint != Mathf.Abs(m_startPosition.y - m_endPosition.y))
        {
            m_transitionLeftPoint = Mathf.Abs(pos.y - m_startPosition.y);
        }

        polygon.AddGround(1, pos);
        m_polygons.Add(polygon);
    }

    public void CreatePlatform(Vector3Int pos, int width)
    {
        Polygon polygon = new Polygon(m_startPosition);
        polygon.AddTiles(1, width, pos);
        polygon.AddGround(width, pos);

        m_transitionRightPoint = SetTransitionSidePoint(m_endPosition, pos, m_transitionRightPoint);
        m_transitionLeftPoint = SetTransitionSidePoint(pos, m_startPosition, m_transitionLeftPoint);

        SetHighestPoint(pos.y);
        SetLowestPoint(pos.y);

        m_polygons.Add(polygon);
    }

    public bool PositionIsUsed(Vector3Int pos)
    {
        for (int i = 0; i < m_polygons.Count; ++i)
        {
            if (m_polygons[i].ContainsTile(pos))
                return true;
        }
        return false;
    }

    public void DrawTiles(TileEditor editor, System.Action<HashSet<Vector3Int>> callback, bool isInitial = false)
    {
        foreach (var poly in m_polygons)
        {
            poly.DrawTiles(editor, callback, isInitial);
        }
    }

    public void AddEnviromentObject(GameObject obj)
    {
        m_enviroment.Add(obj);
    }
    /// <summary>
    /// Adds transition between this and next levels and adds tiles if transition is decreasing so the end of the level can't be seen
    /// </summary>
    /// <param name="transition"></param>
    public void AddTransition(Room transition, bool addExtraTiles = true)
    {
        m_nextTransition = transition;

        if (!addExtraTiles || transition.m_endPosition.y >= m_endPosition.y)
            return;
        int w = Mathf.Min(m_minWidth, m_endPosition.x - m_lastElevationPoint);
        m_polygons[0].AddTiles(m_nextTransition.m_transitionLeftPoint, w, new Vector3Int(m_endPosition.x - w, m_endPosition.y - m_minHeight));
    }
}