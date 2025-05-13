using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeStrategy : FillStrategy
{
    protected new int m_maxRoomWidth = 70;
    protected new int m_minRoomWidth = 20;

    public MazeStrategy(LevelTheme levelTheme ,AnimationCurve enemiesCount, AnimationCurve trapsCount) : base(levelTheme,enemiesCount, trapsCount)
    {
    }

    public override Room FillRoom(Room prevRoom, FillStrategy transitionStrategy,bool isInitial)
    {
        return base.FillRoom(prevRoom, transitionStrategy, isInitial);
    }

    public override Room FillTransition(Room room, bool isInitial)
    {
        return base.FillTransition(room, isInitial);
    }
}
