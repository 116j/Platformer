using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeStrategy : FillStrategy
{
    protected new int m_maxRoomWidth = 70;
    protected new int m_minRoomWidth = 20;

    public MazeStrategy(LevelTheme levelTheme, GameObject checkpoint ,AnimationCurve enemiesCount, AnimationCurve trapsCount, WalkEnemy[] enemies, List<Trap> traps) : base(levelTheme, checkpoint,enemiesCount, trapsCount ,enemies, traps)
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
