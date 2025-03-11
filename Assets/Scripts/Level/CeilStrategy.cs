using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CeilStrategy : FillStrategy
{
    protected new int m_maxRoomWidth = 40;
    protected new int m_minRoomWidth = 15;

    protected new int m_minElevationHeight = 2;
    protected new int m_maxElevationHeight = 20;

    public CeilStrategy(LevelTheme levelTheme) : base(levelTheme)
    {
    }
    /// <summary>
    /// Creates elevations and lowlands for the room, adds ceil and places traps on it, adds landscape and draws tiles
    /// </summary>
    /// <param name="prevRoom"></param>
    /// <param name="transitionStrategy"></param>
    /// <returns></returns>
    public override Room FillRoom(Room prevRoom, FillStrategy transitionStrategy)
    {
        Vector3Int start = prevRoom.GetTransition().GetEndPosition();
        Vector3Int end = new Vector3Int(start.x + Random.Range(m_minRoomWidth, m_maxRoomWidth), start.y);
        //width of start straight section
        int startWidth = Random.Range(m_minStraightSection, end.x - start.x);
        Room room = new Room(end, startWidth, prevRoom.GetTransition());
        // no slopes
        m_slopeChance = 1f;
        CreateElevations(room, start + startWidth * Vector3Int.right, false);
        room.AddTransition(transitionStrategy.FillTransition(room));
        MakeCeil(room);
        return room;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="room"></param>
    void MakeCeil(Room room)
    {
        Trap trap = m_levelTheme.m_ceilTraps[Random.Range(0, m_levelTheme.m_ceilTraps.Length)];
        trap.SetTrapNum();
        int offset = Mathf.CeilToInt(trap.GetHeight());

        var ground = room.GetGround();
        //width for ceil
        int width = 0;
        // width for traps placement
        int groundWidth = 0;
        // ceil start
        Vector3Int start = room.GetStartPosition() + Vector3Int.right;
        // trap placement start
        int groundStart = start.x;
        //height of the ceil
        int height = room.GetRoomHighestPoint() - start.y + m_minStraightSection;
        // if new room is lower than previous - set ceil height higher so palayer can't jump on it
        if (room.GetTransitionHeight() < 0)
        {
            height = Mathf.Clamp(room.GetTransition().GetTransitionRightHeight() + m_minStraightSection, height, int.MaxValue);
        }
        // ceil polygon
        room.MakePolygon(0, start, false);
        for (int i = 1; i < ground.Count - 1; i++)
        {
            if (ground[i].y == start.y && ground[i].x == start.x + width)
            {
                width++;
                groundWidth++;
            }
            //if ground straight section is over 
            else
            {
                if (start.y > ground[i].y)
                {
                    width += offset;
                }
                else
                {
                    groundWidth = groundWidth - offset;
                    width = width - offset;
                    // if elevation is higher than player's jump - add space for platform
                    if (ground[i].y - start.y > m_playerJumpHeight)
                    {
                        groundWidth--;
                        width--;
                    }
                }

                if (width > 1)
                {
                    room.AddTiles(height, Mathf.Clamp(width, 0, room.GetEndPosition().x - start.x), start + Vector3Int.up * (height + offset), false);
                    foreach (var t in AddCeilTraps(trap, groundWidth, new Vector3Int(groundStart, start.y + offset)))
                    {
                        room.AddEnviromentObject(t.gameObject);
                    }
                    width = 1;
                    height += start.y - ground[i].y;
                    // if lowland - offset ground
                    if (start.y > ground[i].y)
                    {
                        i += offset;
                        start = ground[i];
                    }
                    //if elevation - add extra width in the left
                    else
                    {
                        width += offset + 1;
                        start = ground[i] - Vector3Int.right * (offset + 1);
                    }
                }
                else
                {
                    width = ground[i].x - start.x;
                    height += start.y - ground[i].y;
                    start.y = ground[i].y;
                }
                groundWidth = 1;

                if (i >= ground.Count - 1)
                    break;
                groundStart = ground[i].x;
                trap = m_levelTheme.m_ceilTraps[Random.Range(0, m_levelTheme.m_ceilTraps.Length)];
                trap.SetTrapNum();
            }
        }
        room.AddTiles(height, Mathf.Clamp(width, 0, room.GetEndPosition().x - start.x), start + Vector3Int.up * (height + offset), false);
        foreach (var t in AddCeilTraps(trap, groundWidth, new Vector3Int(groundStart, start.y + offset)))
        {
            room.AddEnviromentObject(t.gameObject);
        }
        room.DrawTiles();
        AddLandscape(room, offset, true);
    }
    /// <summary>
    /// Adds a series of traps with offsets
    /// </summary>
    /// <param name="trap"></param>
    /// <param name="width"></param>
    /// <param name="start"></param>
    /// <returns></returns>
    List<Trap> AddCeilTraps(Trap trap, int width, Vector3Int start)
    {
        List<Trap> traps = new List<Trap>();
        float offset = Random.Range(1.5f*m_playerWidth, 2 * m_playerWidth);
        float leftBorder = start.x - trap.GetLeftBorder()+offset;
        float rightBorder = start.x + width - trap.GetRightBorder();

        for (float x = leftBorder; x <= rightBorder;)
        {
            traps.Add(Object.Instantiate(trap, new Vector3(x, start.y), Quaternion.identity));
            x += trap.GetRightBorder() + offset;
        }

        return traps;
    }
}
