using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilStrategy : FillStrategy
{
    public CeilStrategy(LevelTheme levelTheme, GameObject checkpoint, AnimationCurve trapsCount, List<Trap> traps) : base(levelTheme, checkpoint, trapsCount ,traps)
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
