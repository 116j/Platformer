using UnityEngine;

public class GridStrategy : FillStrategy
{
    protected int m_maxRoomSize = 70;
    protected int m_minRoomSize = 20;

    protected new int m_minTransitionWidth = 4;
    protected new int m_maxTransitionWidth = 10;
    protected int m_minTransitionHeight = 11;
    protected new int m_maxTransitionHeight = 25;

    readonly int m_minWidth = 1;
    readonly int m_maxWidth = 6;
    readonly int m_minDist = 3;

    int m_maxAttempts = 3;

    public GridStrategy(LevelTheme levelTheme) : base(levelTheme)
    {
    }

    public override Room FillRoom(Room prevRoom, FillStrategy transitionStrategy)
    {
        Room transition = new Room(prevRoom.GetEndPosition(), prevRoom.GetEndPosition());

        int width = Random.Range(m_minRoomSize, m_maxRoomSize);
        int height = Random.Range(m_minRoomSize, m_maxRoomSize);
        if (Random.value > 0.7f)
        {
            height = -height;
        }
        Vector3Int start = prevRoom.GetEndPosition();
        Vector3Int end = new Vector3Int(start.x + width, start.y + height);
        Room room = new Room(start, end, transition);
        int attempts = 0;

        while (!MakeGrid(room))
        {
            attempts++;
            if (attempts >= m_maxAttempts)
                return null;
        }
        // create bounds for player's fall
        BoxCollider2D bounds = new GameObject().AddComponent<BoxCollider2D>();
        bounds.gameObject.transform.position = new Vector3(start.x, (height > 0 ? start.y : end.y) - m_roomHeight);
        bounds.isTrigger = true;
        bounds.gameObject.tag = "bounds";
        bounds.size = new Vector2(width + 2, 0.5f);
        bounds.offset = new Vector2((width + 2) / 2, -0.5f);
        room.AddEnviromentObject(bounds.gameObject);

        if (height < 0)
        {
            BoxCollider2D boundsL = new GameObject().AddComponent<BoxCollider2D>();
            boundsL.gameObject.transform.position = new Vector3(start.x + 1, start.y - room.GetTransitionLeftHeight() - m_roomHeight + 1);
            boundsL.isTrigger = true;
            boundsL.gameObject.tag = "bounds";
            boundsL.size = new Vector2(m_minStraightSection, boundsL.gameObject.transform.position.y - end.y + m_roomHeight);
            boundsL.offset = new Vector2(-boundsL.size.x / 2, -boundsL.size.y / 2);
            room.AddEnviromentObject(boundsL.gameObject);
        }
        else
        {
            BoxCollider2D boundsR = new GameObject().AddComponent<BoxCollider2D>();
            boundsR.gameObject.transform.position = new Vector3(end.x, end.y - room.GetTransitionRightHeight() - m_roomHeight + 1);
            boundsR.isTrigger = true;
            boundsR.gameObject.tag = "bounds";
            boundsR.size = new Vector2(m_minStraightSection, boundsR.gameObject.transform.position.y - start.y + m_roomHeight);
            boundsR.offset = new Vector2(boundsR.size.x / 2, -boundsR.size.y / 2);
            room.AddEnviromentObject(boundsR.gameObject);
        }

        prevRoom.GetNextTransition().Clear();
        prevRoom.AddTransition(transition);
        room.AddTransition(new Room(end, end));
        room.DrawTiles();
        AddLandscape(room, int.MaxValue, false);
        return room;
    }

    public override Room FillTransition(Room room)
    {
        int width = Random.Range(m_minTransitionWidth, m_maxTransitionWidth);
        int height = Random.Range(m_minTransitionHeight, m_maxTransitionHeight);
        Vector3Int start = room.GetEndPosition();
        Vector3Int end = new Vector3Int(start.x + width, start.y + height);
        Room transition = new Room(start, end);

        Vector3Int lastPoint = start + Vector3Int.right;
        int platformWidth = (width - 2) / 2/*Mathf.Clamp(Random.Range(m_minWidth, m_minDist+1), m_minWidth, (width - 2) / 2)*/;
        int horOffset = width - 2 - platformWidth * 2;
        int vertOffset = Random.Range(m_minDist, GetJumpHeight(horOffset));
        bool posOffset = true;
        do
        {
            transition.CreatePlatform(lastPoint, platformWidth);
            lastPoint += new Vector3Int((posOffset ? 1 : -1) * (platformWidth + horOffset), vertOffset);
            posOffset = !posOffset;
        }
        while (lastPoint.y < end.y);
        // create bounds for player's fall
        BoxCollider2D bounds = new GameObject().AddComponent<BoxCollider2D>();
        bounds.gameObject.transform.position = new Vector3(start.x, (height > 0 ? start.y : end.y) - m_roomHeight);
        bounds.isTrigger = true;
        bounds.gameObject.tag = "bounds";
        bounds.size = new Vector2(width + 2, 0.5f);
        bounds.offset = new Vector2((width + 2) / 2, -0.5f);
        transition.AddEnviromentObject(bounds.gameObject);

        transition.DrawTiles();
        AddLandscape(transition, int.MaxValue, false);
        return transition;
    }

    bool MakeGrid(Room room)
    {
        Vector3Int start = room.GetStartPosition();
        Vector3Int end = room.GetEndPosition();
        Vector3Int lastPoint = start;
        Vector3Int lastOffset = start;
        int attempts = 0;
        int maxAttempts = 100;
        int lastWidth = 0;
        int w1 = 0;
        int platformsOffsetY = 0;
        while ((lastPoint.x + lastWidth <= end.x - 3 || end.x - lastPoint.x - lastWidth <= m_playerJumpWidth && Mathf.Abs(lastPoint.y - end.y) > GetJumpHeight(end.x - lastPoint.x - lastWidth)) &&
           (lastOffset.x + w1 <= end.x - 3 || end.x - lastOffset.x - w1 <= m_playerJumpWidth && Mathf.Abs(lastOffset.y - end.y) > GetJumpHeight(end.x - lastOffset.x - w1)))
        {
            int offsetY = (lastPoint.y > end.y ? -1 : 1) * Random.Range(m_minDist, m_playerJumpHeight);
            int x = lastPoint.x + lastWidth + (int)(Mathf.Abs(offsetY) * 1.0f / Mathf.Max(1, Mathf.Abs(end.y - lastPoint.y)) * (end.x - lastPoint.x - lastWidth));
            int offsetX;
            if (x - lastPoint.x - lastWidth < GetJumpWidth(offsetY))
                offsetX = Random.Range(-(lastPoint.y < end.y ? GetJumpWidth(offsetY) : m_playerJumpWidth), (lastPoint.y < end.y ? GetJumpWidth(offsetY) : m_playerJumpWidth));
            else
                offsetX = Random.Range((lastPoint.y < end.y ? GetJumpWidth(offsetY) : m_playerJumpWidth) - x + lastPoint.x + lastWidth + m_minWidth, -lastPoint.x + x - (lastPoint.y < end.y ? GetJumpWidth(offsetY) : m_playerJumpWidth) - m_minWidth);
            offsetX = Mathf.Clamp(offsetX, start.x - x + m_minWidth, end.x - x - m_minWidth * 2);
            // offsetX = Mathf.Clamp(offsetX ,- lastPoint.x + x - GetJumpWidth(offsetY) - m_minWidth, GetJumpWidth(offsetY) - x + lastPoint.x + lastWidth);

            Vector3Int pos = CheckSurroundings(room, new Vector3Int(x + offsetX, lastPoint.y + offsetY), end);
            if (Mathf.Abs(lastPoint.x + lastWidth - pos.x) > GetJumpWidth(offsetY) ||
                !AvailablePlatform(room, lastPoint.y > lastOffset.y && end.y - start.y > 0 ? lastPoint.y : lastOffset.y, pos, GetMaxWidth(room, pos), ref lastWidth, end) ||
            pos.x > end.x - m_minWidth * 2 || pos.x <= start.x)
            {
                attempts++;
                if (attempts >= maxAttempts)
                    return false;
                continue;
            }

            lastWidth = Mathf.Clamp(lastWidth, m_minWidth, end.x - pos.x - m_minWidth);
            lastPoint = pos;
            room.CreatePlatform(lastPoint, lastWidth);

            bool offset = (offsetX * (end.y - start.y) >= 0 ? -1 : 1) * platformsOffsetY > 0;
            platformsOffsetY = (offsetX * (end.y - start.y) >= 0 ? -1 : 1) * Random.Range(m_minDist - 1, m_playerJumpHeight);
            int offsetX1 = offsetX > 0 ? Random.Range(offset ? Mathf.Clamp(-m_minDist + 1, lastOffset.x + w1 - lastPoint.x + m_minWidth, 0) : -m_minDist + 1, GetJumpWidth(platformsOffsetY) + lastWidth) :
                Random.Range(offset ? Mathf.Clamp(-GetJumpWidth(platformsOffsetY) - m_minWidth, lastOffset.x + w1 - lastPoint.x + m_minWidth, 0) : -GetJumpWidth(platformsOffsetY) - m_minWidth, m_minDist);
            offsetX1 = Mathf.Clamp(offsetX1, start.x - lastPoint.x + m_minWidth, end.x - lastPoint.x - m_minWidth * 2);
            pos = CheckSurroundings(room, new Vector3Int(lastPoint.x + offsetX1, lastPoint.y + platformsOffsetY), end);
            if (!AvailablePlatform(room, lastPoint.y > lastOffset.y && end.y - start.y > 0 ? lastPoint.y : lastOffset.y, pos, GetMaxWidth(room, pos), ref w1, end) ||
                pos.x > end.x - m_minWidth || pos.x <= start.x)
            {
                platformsOffsetY = lastOffset.y;
                continue;
            }
            w1 = Mathf.Clamp(w1, m_minWidth, end.x - pos.x - m_minWidth);
            room.CreatePlatform(pos, w1);
            lastOffset = pos;
        }
        return true;
    }

    bool AvailablePlatform(Room room, int lastPointY, Vector3Int currentPoint, int maxWidth, ref int currentWidth, Vector3Int end)
    {
        if (maxWidth < m_minWidth)
            return false;

        for (int i = 0; i < m_playerJumpHeight; i++)
        {
            if (room.PositionIsUsed(new Vector3Int(currentPoint.x, currentPoint.y - i)) &&
            !room.PositionIsUsed(new Vector3Int(currentPoint.x - 1, currentPoint.y - i)) &&
            !room.PositionIsUsed(new Vector3Int(currentPoint.x + 1, currentPoint.y - i)))
                return false;

            if (room.PositionIsUsed(new Vector3Int(currentPoint.x, currentPoint.y + i)))
            {
                for (int j = 1; j < maxWidth; j++)
                {
                    if (!room.PositionIsUsed(new Vector3Int(currentPoint.x + j, currentPoint.y + i)))
                    {
                        if (currentPoint.x + j + 1 > end.x - m_minWidth || j + 1 == Mathf.Clamp(maxWidth, 0, end.x - currentPoint.x) - 1)
                            return false;

                        currentWidth = Random.Range(j + 1, Mathf.Clamp(maxWidth, 0, end.x - currentPoint.x));
                        return true;
                    }
                }
                return false;
            }

            for (int j = 0; j < maxWidth; j++)
            {
                if (room.PositionIsUsed(new Vector3Int(currentPoint.x + j, currentPoint.y - i)))
                {
                    for (int k = 1; k <= maxWidth - j; k++)
                    {
                        if (!room.PositionIsUsed(new Vector3Int(currentPoint.x + j + k, currentPoint.y - i)))
                        {
                            currentWidth = Random.Range(m_minWidth, Mathf.Clamp(j + k, 0, end.x - currentPoint.x));
                            return true;
                        }
                    }
                }
            }

        }
        currentWidth = Random.Range(m_minWidth, maxWidth);
        return true;
    }

    int GetMaxWidth(Room room, Vector3Int pos)
    {
        for (int i = 0; i < m_maxWidth; i++)
        {
            if (room.PositionIsUsed(new Vector3Int(pos.x + i + 1, pos.y + 1)) ||
                room.PositionIsUsed(new Vector3Int(pos.x + i + 1, pos.y - 1)))
            {
                return i;
            }
            for (int j = 0; j < m_minDist; j++)
            {
                if (room.PositionIsUsed(new Vector3Int(pos.x + i + j, pos.y)) ||
                    room.PositionIsUsed(new Vector3Int(pos.x + i, pos.y - j)) ||
                    room.PositionIsUsed(new Vector3Int(pos.x + i, pos.y + j)))
                {
                    return i;
                }
            }
        }

        return m_maxWidth;
    }

    Vector3Int CheckSurroundings(Room room, Vector3Int pos, Vector3Int end)
    {
        bool suitable = false;

        if (room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y - 1)) &&
            room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y + 1)) ||
            room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y + 1)) &&
            room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y - 1)))
            return end;

        while (!suitable)
        {
            suitable = true;
            if (((room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y + 1)) ||
                room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y + 2)) ||
                room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y - 2)) ||
            room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y - 1))) &&
            !room.PositionIsUsed(new Vector3Int(pos.x + 2, pos.y - 1)) &&
            !room.PositionIsUsed(new Vector3Int(pos.x + 2, pos.y - 2)) &&
            !room.PositionIsUsed(new Vector3Int(pos.x + 2, pos.y + 2)) &&
            !room.PositionIsUsed(new Vector3Int(pos.x + 2, pos.y + 1))))
            {
                pos += Vector3Int.right;
                if (!CheckSurroundings(Vector3Int.right, room, pos))
                    return end;
                suitable = false;
                continue;
            }
            else if (((room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y + 1)) ||
                room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y + 2)) ||
                room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y + 2)) ||
            room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y + 1))) &&
           !room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y - 2)) &&
           !room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y - 3)) &&
           !room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y - 3)) &&
            !room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y - 2))))
            {
                pos += Vector3Int.down;
                if (!CheckSurroundings(Vector3Int.down, room, pos))
                    return end;
                suitable = false;
                continue;
            }
            else if (((room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y - 1)) ||
                room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y - 2)) ||
                room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y - 2)) ||
                room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y - 1))) &&
                !room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y + 2)) &&
                !room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y + 3)) &&
                !room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y + 3)) &&
            !room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y + 2))))
            {
                pos += Vector3Int.up;
                if (!CheckSurroundings(Vector3Int.up, room, pos))
                    return end;
                suitable = false;
                continue;
            }
            else if (((room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y + 1)) ||
                room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y + 2)) ||
                room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y - 2)) ||
                room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y - 1))) &&
                !room.PositionIsUsed(new Vector3Int(pos.x - 2, pos.y + 1)) &&
                !room.PositionIsUsed(new Vector3Int(pos.x - 2, pos.y + 2)) &&
                !room.PositionIsUsed(new Vector3Int(pos.x - 2, pos.y - 2)) &&
            !room.PositionIsUsed(new Vector3Int(pos.x - 2, pos.y - 1))))
            {
                pos += Vector3Int.left;
                if (!CheckSurroundings(Vector3Int.left, room, pos))
                    return end;
                suitable = false;
                continue;
            }

            for (int i = 0; i < m_minDist; i++)
            {

                if (room.PositionIsUsed(new Vector3Int(pos.x - i, pos.y)))
                {
                    if (!CheckSurroundings(Vector3Int.right, room, pos + Vector3Int.right * (m_minDist - i)))
                    {
                        if (!CheckSurroundings(Vector3Int.up, room, pos + Vector3Int.up * (m_minDist - i)))
                        {
                            if (!CheckSurroundings(Vector3Int.down, room, pos + Vector3Int.down * (m_minDist - i)))
                            {
                                return end;
                            }
                            else
                            {
                                pos += Vector3Int.down * (m_minDist - i);
                            }
                        }
                        else
                        {
                            pos += Vector3Int.up * (m_minDist - i);
                        }
                    }
                    else
                    {
                        pos += Vector3Int.right * (m_minDist - i);
                    }
                    suitable = false;
                    break;
                }
                if (room.PositionIsUsed(new Vector3Int(pos.x + i, pos.y)))
                {
                    if (!CheckSurroundings(Vector3Int.left, room, pos + Vector3Int.left * (m_minDist - i)))
                    {
                        if (!CheckSurroundings(Vector3Int.up, room, pos + Vector3Int.up * (m_minDist - i)))
                        {
                            if (!CheckSurroundings(Vector3Int.down, room, pos + Vector3Int.down * (m_minDist - i)))
                            {
                                return end;
                            }
                            else
                            {
                                pos += Vector3Int.down * (m_minDist - i);
                            }
                        }
                        else
                        {
                            pos += Vector3Int.up * (m_minDist - i);
                        }
                    }
                    else
                    {
                        pos += Vector3Int.left * (m_minDist - i);
                    }
                    suitable = false;
                    break;
                }
                if (room.PositionIsUsed(new Vector3Int(pos.x, pos.y - i)))
                {
                    if (!CheckSurroundings(Vector3Int.up, room, pos + Vector3Int.up * (m_minDist - i)))
                    {
                        if (!CheckSurroundings(Vector3Int.left, room, pos + Vector3Int.left * (m_minDist - i)))
                        {
                            if (!CheckSurroundings(Vector3Int.right, room, pos + Vector3Int.right * (m_minDist - i)))
                            {
                                return end;
                            }
                            else
                            {
                                pos += Vector3Int.right * (m_minDist - i);
                            }
                        }
                        else
                        {
                            pos += Vector3Int.left * (m_minDist - i);
                        }
                    }
                    else
                    {
                        pos += Vector3Int.up * (m_minDist - i);
                    }
                    suitable = false;
                    break;
                }
                if (room.PositionIsUsed(new Vector3Int(pos.x, pos.y + i)))
                {
                    if (!CheckSurroundings(Vector3Int.down, room, pos + Vector3Int.down * (m_minDist - i)))
                    {
                        if (!CheckSurroundings(Vector3Int.right, room, pos + Vector3Int.right * (m_minDist - i)))
                        {
                            if (!CheckSurroundings(Vector3Int.left, room, pos + Vector3Int.left * (m_minDist - i)))
                            {
                                return end;
                            }
                            else
                            {
                                pos += Vector3Int.left * (m_minDist - i);
                            }
                        }
                        else
                        {
                            pos += Vector3Int.right * (m_minDist - i);
                        }
                    }
                    else
                    {
                        pos += Vector3Int.down * (m_minDist - i);
                    }
                    suitable = false;
                    break;
                }
            }
        }

        return pos;
    }

    bool CheckSurroundings(Vector3Int dir, Room room, Vector3Int pos)
    {
        if (dir == Vector3Int.right)
        {
            if ((room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y + 1)) ||
                room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y + 2))) &&
                (room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y - 1)) ||
                room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y - 2))))
                return false;

            for (int i = 0; i < m_minDist; i++)
            {
                if (room.PositionIsUsed(new Vector3Int(pos.x + i, pos.y)) ||
                    room.PositionIsUsed(new Vector3Int(pos.x, pos.y - i)) &&
                    room.PositionIsUsed(new Vector3Int(pos.x, pos.y + 2 * m_minDist - i - 1)) ||
                    room.PositionIsUsed(new Vector3Int(pos.x, pos.y + i)) &&
                    room.PositionIsUsed(new Vector3Int(pos.x, pos.y - 2 * m_minDist + i + 1)))
                    return false;
            }

        }
        else if (dir == Vector3Int.left)
        {
            if ((room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y - 1)) ||
                room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y - 2))) &&
            (room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y + 1)) ||
            room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y + 2))))
                return false;

            for (int i = 0; i < m_minDist; i++)
            {
                if (room.PositionIsUsed(new Vector3Int(pos.x - i, pos.y)) ||
                    room.PositionIsUsed(new Vector3Int(pos.x, pos.y - i)) &&
                    room.PositionIsUsed(new Vector3Int(pos.x, pos.y + 2 * m_minDist - i - 1)) ||
                    room.PositionIsUsed(new Vector3Int(pos.x, pos.y + i)) &&
                    room.PositionIsUsed(new Vector3Int(pos.x, pos.y - 2 * m_minDist + i + 1)))
                    return false;
            }
        }
        else if (dir == Vector3Int.up)
        {
            if ((room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y + 1)) ||
                room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y + 2))) &&
            (room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y + 1)) ||
            room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y + 2))))
                return false;

            for (int i = 0; i < m_minDist; i++)
            {
                if (room.PositionIsUsed(new Vector3Int(pos.x, pos.y + i)) ||
                    room.PositionIsUsed(new Vector3Int(pos.x - i, pos.y)) &&
                    room.PositionIsUsed(new Vector3Int(pos.x + 2 * m_minDist - i - 1, pos.y)) ||
                    room.PositionIsUsed(new Vector3Int(pos.x + i, pos.y)) &&
                    room.PositionIsUsed(new Vector3Int(pos.x - 2 * m_minDist + i + 1, pos.y)))
                    return false;
            }
        }
        else if (dir == Vector3Int.down)
        {
            if ((room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y - 1)) ||
                room.PositionIsUsed(new Vector3Int(pos.x - 1, pos.y - 2))) &&
                (room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y - 1)) ||
                room.PositionIsUsed(new Vector3Int(pos.x + 1, pos.y - 2))))
                return false;

            for (int i = 0; i < m_minDist; i++)
            {
                if (room.PositionIsUsed(new Vector3Int(pos.x, pos.y - i)) ||
                   room.PositionIsUsed(new Vector3Int(pos.x - i, pos.y)) &&
                   room.PositionIsUsed(new Vector3Int(pos.x + 2 * m_minDist - i - 1, pos.y)) ||
                   room.PositionIsUsed(new Vector3Int(pos.x + i, pos.y)) &&
                   room.PositionIsUsed(new Vector3Int(pos.x - 2 * m_minDist + i + 1, pos.y)))
                    return false;
            }
        }

        return true;
    }
}


