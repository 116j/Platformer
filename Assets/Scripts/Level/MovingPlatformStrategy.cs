using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformStrategy : FillStrategy
{
    protected new int m_maxRoomWidth = 70;
    protected new int m_minRoomWidth = 20;

    public MovingPlatformStrategy(LevelTheme levelTheme, MovingPlatform movingPlatform) : base(levelTheme)
    {
        m_movingPlatform = movingPlatform;
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
