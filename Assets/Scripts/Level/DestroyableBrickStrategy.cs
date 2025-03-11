using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestroyableBrickStrategy : FillStrategy
{
    protected new int m_maxRoomWidth = 50;
    protected new int m_minRoomWidth = 20;

    protected new int m_maxTransitionHeight = 30;

    TilePlaceAnalog m_destroyableTile;

    public DestroyableBrickStrategy(LevelTheme levelTheme, TilePlaceAnalog destroyableTile) : base(levelTheme)
    {
        m_destroyableTile = destroyableTile;
    }

    public override Room FillRoom(Room prevRoom, FillStrategy transitionStrategy)
    {
        return base.FillRoom(prevRoom, transitionStrategy);
    }

    public override Room FillTransition(Room room)
    {
        return base.FillTransition(room);
    }


}
