using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeStrategy : FillStrategy
{
    protected new int m_maxRoomWidth = 70;
    protected new int m_minRoomWidth = 20;

    public MazeStrategy(LevelTheme levelTheme, GameObject cat ,AnimationCurve enemiesCount, AnimationCurve trapsCount) : base(levelTheme, cat,enemiesCount, trapsCount)
    {
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
