using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyableBrickStrategy : FillStrategy
{
    protected int m_maxRoomSize = 50;
    protected int m_minRoomSize = 20;

    protected new int m_maxTransitionHeight = 30;
    protected int m_minTransitionHeight = 11;
    protected int m_transitionWidth = 4;

    int m_tunnelTimerZone = 9;
    int m_tunnelMinZone = 6;
    int m_tunnelSafeZone = 4;

    int m_minStepWidth = 2;
    int m_maxStepWidth = 5;
    int m_maxStarirsOffsetX = 4;
    int m_minStarirsOffset = 1;

    DestroyableBrick m_brick;

    public DestroyableBrickStrategy(LevelTheme levelTheme, DestroyableBrick destroyableBrick) : base(levelTheme)
    {
        m_brick = destroyableBrick;
    }

    public override Room FillRoom(Room prevRoom, FillStrategy transitionStrategy)
    {
        prevRoom.GetNextTransition().Clear(m_editor);
        Room transition = new Room(prevRoom.GetEndPosition(), prevRoom.GetEndPosition());

        Vector3Int start = prevRoom.GetEndPosition();
        int width = Random.Range(m_minRoomSize, m_maxRoomSize);
        int height = Random.Range(-m_minRoomSize, m_minRoomSize);

        Vector3Int end = new Vector3Int(start.x + width, start.y + height);
        Room room = new Room(start, end, transition);
        int fillType = Random.Range(0,4);

        switch (fillType)
        {
            case 0:
                CollapseStaircase(room);
                if (height != 0)
                {
                    CreateSideBound(room, true);
                    CreateSideBound(room, false);
                }
                break;
            case 1:
                ResonanceCorridor(room, height);
                CreateSideBound(room, height < 0);
                break;

            case 2:
                CollapseTunnel(room);
                break;

            case 3:
                WaveOfCollapse(room, height);
                CreateSideBound(room, height < 0);
                break;
        }

        end = room.GetEndPosition();
        start = room.GetStartPosition();
        room.AddEnviromentObject(CreateHorizontalBounds(start, end, end.x - start.x, end.y - start.y));
        room.AddTransition(new Room(end, end));

        prevRoom.AddTransition(transition);

        return room;
    }

    void WaveOfCollapse(Room room, int roomHeight)
    {
        Vector3Int currentPos = room.GetStartPosition() + Vector3Int.left;
        Vector3Int end = room.GetEndPosition();
        List<DestroyableBrick> group;

        while (currentPos.x < end.x)
        {
            int maxWidth = GetMaxTileWidthGap(currentPos, end);
            int tilesCount = Random.Range(1, maxWidth + 1);
            int height = Mathf.Min(Random.Range(GetTileHeightGapForWidth(currentPos, end, tilesCount + 1), GetTileHeightGapForWidth(currentPos, end, maxWidth)), tilesCount + 1);
            group = new List<DestroyableBrick>(tilesCount);
            Vector3Int offset = Vector3Int.zero;
            for (int i = 1; i <= tilesCount; i++)
            {
                offset += new Vector3Int(1, ((height - offset.y) * roomHeight > 0 ? 1 : -1) / (tilesCount + 1 - i));
                CreateBrick(room, currentPos + offset, BrickBehaviour.Timer, group);
            }
            currentPos += new Vector3Int(tilesCount + 1, height * roomHeight > 0 ? 1 : -1);
            if (currentPos.x < end.x)
            {
                CreateBrick(room, currentPos, BrickBehaviour.None);
            }
        }
        room.SetEndPosition(currentPos);
    }

    void ResonanceCorridor(Room room, int roomHeight)
    {
        Vector3Int currentPos = room.GetStartPosition();
        Vector3Int end = room.GetEndPosition();
        int lineOffset = GetJumpHeight(2);

        while (currentPos.x < end.x)
        {
            CreateBrick(room, currentPos, BrickBehaviour.OnExit, new List<DestroyableBrick>(1));
            currentPos += new Vector3Int(1, Mathf.Min(GetTileHeightGapForWidth(currentPos, end, 1), 1) * (roomHeight > 0 ? 1 : -1));
            CreateBrick(room, currentPos + Vector3Int.down * lineOffset, BrickBehaviour.Timer, new List<DestroyableBrick>(1));
            if (currentPos.x != end.x)
            {
                currentPos += new Vector3Int(1, Mathf.Min(GetTileHeightGapForWidth(currentPos, end, 1), 1) * (roomHeight > 0 ? 1 : -1));
            }
            else
            {
                currentPos += new Vector3Int(1, 0);
            }
        }
        room.SetStartPosition(room.GetStartPosition() + Vector3Int.down * lineOffset);
        room.SetEndPosition(currentPos);
    }

    void CollapseStaircase(Room room)
    {
        Vector3Int currentPos = room.GetStartPosition();
        Vector3Int end = room.GetEndPosition();
        List<DestroyableBrick> group;

        int verticalGap = GetJumpHeight(3);
        int stepWidth = Random.Range(m_minStepWidth, m_maxStepWidth);
        int offsetX = Random.Range(m_minStarirsOffset, m_maxStarirsOffsetX);
        int offsetY = Random.Range(m_minStarirsOffset, GetJumpHeight(offsetX) + 1);
        int level = 0;

        while (currentPos.x < end.x)
        {
            switch (level)
            {
                case 0:
                    for (int i = 0; i < stepWidth; i++)
                    {
                        CreateBrick(room, currentPos + Vector3Int.right * i, BrickBehaviour.OnExit, new List<DestroyableBrick>(1));
                    }
                    currentPos += new Vector3Int(stepWidth + offsetX, verticalGap + offsetY);
                    break;
                case 1:
                    group = new List<DestroyableBrick>(stepWidth);
                    for (int i = 0; i < stepWidth; i++)
                    {
                        CreateBrick(room, currentPos + Vector3Int.right * i, BrickBehaviour.Timer, group);
                    }
                    currentPos += new Vector3Int(stepWidth + offsetX, -verticalGap);
                    break;
                case 2:
                    for (int i = 0; i < stepWidth; i++)
                    {
                        CreateBrick(room, currentPos + Vector3Int.right * i, BrickBehaviour.OnEnter, new List<DestroyableBrick>(1));
                    }
                    currentPos += new Vector3Int(stepWidth + offsetX, -offsetY);
                    break;
            }

            level = (level + 1) % 3;
        }
        room.SetEndPosition(currentPos);
    }

    void CollapseTunnel(Room room)
    {
        Vector3Int currentPos = room.GetStartPosition();
        Vector3Int end = room.GetEndPosition();
        List<DestroyableBrick> group;

        while (currentPos.x < end.x)
        {
            int timerZone = Random.Range(m_tunnelMinZone, m_tunnelTimerZone);
            group = new List<DestroyableBrick>(timerZone);
            for (int i = 0; i < timerZone; i++)
            {
                CreateBrick(room, currentPos + Vector3Int.right * i, BrickBehaviour.Timer, group);
            }
            currentPos += Vector3Int.right * timerZone;
            if (currentPos.x < end.x)
            {
                int safeZone = Random.Range(m_tunnelSafeZone, m_tunnelMinZone);
                for (int i = 0; i < safeZone; i++)
                {
                    CreateBrick(room, currentPos + Vector3Int.right * i, BrickBehaviour.None);
                }
                currentPos += Vector3Int.right * safeZone;
            }
        }

        room.SetEndPosition(currentPos);
    }

    void CreateBrick(Room room, Vector3Int pos, BrickBehaviour behaviour, List<DestroyableBrick> group = null)
    {
        DestroyableBrick brick = Object.Instantiate(m_brick, pos, Quaternion.identity);
        brick.SetBrickBehaviour(behaviour, m_levelTheme.m_themeNum, m_levelTheme.m_destroyableTile, group);
        room.CreatePlatform(pos, 1);
        room.AddEnviromentObject(brick.gameObject);
    }

    public override Room FillTransition(Room room)
    {
        int width = m_transitionWidth;
        int height = Random.Range(m_minTransitionHeight, m_maxTransitionHeight);
        Vector3Int start = room.GetEndPosition();
        Vector3Int end = new Vector3Int(start.x + width, start.y + height);
        Room transition = new Room(start, end);
        transition.DontFillTiles();

        Vector3Int lastPoint = start + Vector3Int.right;
        int vertOffset = Random.Range(m_playerJumpHeight/2+1,GetJumpHeight(1));
        bool posOffset = true;
        do
        {
            CreateBrick(transition, lastPoint, BrickBehaviour.Timer, new List<DestroyableBrick>(1));
            lastPoint += new Vector3Int((posOffset ? 1 : -1), vertOffset);
            posOffset = !posOffset;
        }
        while (lastPoint.y < end.y);
        // create bounds for player's fall
        transition.AddEnviromentObject(CreateHorizontalBounds(start, end, width + 1, height));
        return transition;
    }
}
