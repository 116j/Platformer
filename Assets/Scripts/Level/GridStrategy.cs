using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridStrategy : FillStrategy
{
    protected new int m_maxRoomWidth = 70;
    protected new int m_minRoomWidth = 20;

    readonly int m_minWidth = 1;
    readonly int m_maxWidth = 5;
    readonly int m_maxVerticalDist = 5;
    readonly int m_minVerticalDist = 2;
    readonly int m_minHorizontalDist = 2;
    readonly int m_maxHorizontalDist = 11;

    readonly int m_verticalOffset = 6;

    public GridStrategy(LevelTheme levelTheme) : base(levelTheme)
    {
    }

    public override Room FillRoom(Room transition)
    {
        return base.FillRoom(transition);
    }

    public override Room FillTransition(Room previousRoom)
    {
        return base.FillTransition(previousRoom);
    }
}
