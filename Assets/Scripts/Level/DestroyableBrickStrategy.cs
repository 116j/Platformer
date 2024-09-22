using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestroyableBrickStrategy : FillStrategy
{
    protected new int m_maxRoomWidth = 70;
    protected new int m_minRoomWidth = 20;

    TilePlaceAnalog m_destroyableTile;

    public DestroyableBrickStrategy(LevelTheme levelTheme, TilePlaceAnalog destroyableTile) : base(levelTheme)
    {
        m_destroyableTile = destroyableTile;
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
