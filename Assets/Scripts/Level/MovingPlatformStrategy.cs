using UnityEngine;

public class MovingPlatformStrategy : FillStrategy
{
    protected int m_maxRoomSize = 50;
    protected int m_minRoomSize = 25;

    protected new int m_minTransitionWidth = 5;
    protected new int m_maxTransitionWidth = 10;
    protected int m_minTransitionHeight = 11;
    protected new int m_maxTransitionHeight = 25;

    float m_minWidth = 5;
    float m_maxWidth = 15;

    float m_minVerticalDist = 1;
    float m_minHorizontalDist = 5;

    AnimationCurve m_speed;

    public MovingPlatformStrategy(LevelTheme levelTheme, AnimationCurve speed) : base(levelTheme)
    {
        m_speed = speed;
    }

    public override Room FillRoom(Room prevRoom, FillStrategy transitionStrategy, bool isInitial)
    {
        prevRoom.GetNextTransition().Clear();
        Room transition = new Room(prevRoom.GetEndPosition(), prevRoom.GetEndPosition());
        prevRoom.AddTransition(transition);

        Vector3Int start = prevRoom.GetNextTransition().GetEndPosition();
        int width = Random.Range(m_minRoomSize, m_maxRoomSize);
        int height = Random.Range(m_minRoomSize, m_maxRoomSize);
        if (Random.value > 0.5)
        {
            height = -height;
        }
        Vector3Int end = new Vector3Int(start.x + width, start.y + height);
        //width of start straight section
        Room room = new Room(start, end, transition);

        MovingPlatform movingPlatform = m_levelTheme.m_movingPlatform.GetComponent<MovingPlatform>();
        Vector3 lastPoint = start + new Vector3(m_levelTheme.m_movingPlatform.GetWidth(), 1 - m_levelTheme.m_movingPlatform.GetHeight());
        float prev = 0;
        while (lastPoint.x < end.x - m_levelTheme.m_movingPlatform.GetWidth())
        {
            float value = Random.value;
            float speed = m_speed.Evaluate(LevelBuilder.Instance.LevelProgress());
            Vector3 first = lastPoint;
            MovingPlatform platform;
            if (Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y) > m_minVerticalDist && (end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x) > m_minHorizontalDist &&
                (value <= 0.65f && Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y) > (end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x) || value >= 0.75f)
                || Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y) > m_minVerticalDist && Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y) < m_maxWidth + m_minWidth && (end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x) < m_minHorizontalDist)
            {
                Vector3 second;
                if (Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y) < m_maxWidth + m_minWidth && (end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x) < m_minHorizontalDist)
                {
                    second = first + Vector3.up * Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y) * (height < 0 ? -1 : 1);
                    if (prev != 0)
                    {
                        platform = Object.Instantiate(movingPlatform, second, Quaternion.identity);
                        prev = 0;
                        platform.AddCheckpoint(first);
                    }
                    else
                    {
                        platform = Object.Instantiate(movingPlatform, first, Quaternion.identity);
                        prev = Mathf.Abs(second.y - first.y) / speed;
                        platform.AddCheckpoint(second);
                    }
                }
                else
                {
                    if (prev != 0)
                    {
                        int minN = Mathf.FloorToInt((prev + movingPlatform.GetWaitTime()) / (m_minWidth / speed + movingPlatform.GetWaitTime()));
                        int maxN = Mathf.FloorToInt((Mathf.Clamp(m_maxWidth, 0, Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y)) / speed + movingPlatform.GetWaitTime()) / (prev + movingPlatform.GetWaitTime()));
                        int n;
                        if (Random.value > 0.5f)
                        {
                            n = Random.Range(1, minN + 1);
                            second = first + Vector3.up * speed * (height < 0 ? -1 : 1) * (prev - (n - 1) * movingPlatform.GetWaitTime()) / n;
                        }
                        else
                        {
                            n = Random.Range(1, maxN + 1);
                            second = first + Vector3.up * speed * (height < 0 ? -1 : 1) * (n * prev + (n - 1) * movingPlatform.GetWaitTime());
                        }
                        if (n % 2 == 0)
                        {
                            platform = Object.Instantiate(movingPlatform, first, Quaternion.identity);
                            prev = Mathf.Abs(second.y - first.y) / speed;
                            platform.AddCheckpoint(second);
                        }
                        else
                        {
                            platform = Object.Instantiate(movingPlatform, second, Quaternion.identity);
                            prev = 0;
                            platform.AddCheckpoint(first);
                        }
                    }
                    else
                    {
                        second = first + Vector3.up * (height < 0 ? -1 : 1) * Mathf.Clamp(Random.Range(m_minWidth, m_maxWidth), 0, Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y));
                        platform = Object.Instantiate(movingPlatform, first, Quaternion.identity);
                        prev = Mathf.Abs(second.y - first.y) / speed;
                        platform.AddCheckpoint(second);
                    }
                }

                lastPoint = second;
            }
            else if (Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y) > m_minVerticalDist && (end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x) > m_minHorizontalDist
                && (value >= 0.65f && Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y) < (end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x) && value <= 0.5f)
                || Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y) < m_minVerticalDist && (end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x) > m_minHorizontalDist && (end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x) < m_maxWidth + m_minWidth)
            {
                Vector3 second;
                if (Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y) < m_minVerticalDist && (end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x) < m_maxWidth + m_minWidth)
                {
                    second = first + Vector3.right * (end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x);
                    if (prev != 0)
                    {
                        platform = Object.Instantiate(movingPlatform, second, Quaternion.identity);
                        prev = 0;
                        platform.AddCheckpoint(first);
                    }
                    else
                    {
                        platform = Object.Instantiate(movingPlatform, first, Quaternion.identity);
                        prev = (second.x - first.x) / speed;
                        platform.AddCheckpoint(second);
                    }
                }
                else
                {
                    if (prev != 0)
                    {
                        int minN = Mathf.FloorToInt((prev + movingPlatform.GetWaitTime()) / (m_minWidth / speed + movingPlatform.GetWaitTime()));
                        int maxN = Mathf.FloorToInt((Mathf.Clamp(m_maxWidth, 0, end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x) / speed + movingPlatform.GetWaitTime()) / (prev + movingPlatform.GetWaitTime()));
                        int n;
                        if (Random.value > 0.5f)
                        {
                            n = Random.Range(1, minN + 1);
                            second = first + Vector3.right * speed * (prev - (n - 1) * movingPlatform.GetWaitTime()) / n;
                        }
                        else
                        {
                            n = Random.Range(1, maxN + 1);
                            second = first + Vector3.right * speed * (n * prev + (n - 1) * movingPlatform.GetWaitTime());
                        }
                        if (n % 2 == 0)
                        {
                            platform = Object.Instantiate(movingPlatform, first, Quaternion.identity);
                            prev = (second.x - first.x) / speed;
                            platform.AddCheckpoint(second);
                        }
                        else
                        {
                            platform = Object.Instantiate(movingPlatform, second, Quaternion.identity);
                            prev = 0;
                            platform.AddCheckpoint(first);
                        }
                    }
                    else
                    {
                        second = first + Vector3.right * Mathf.Clamp(Random.Range(m_minWidth, m_maxWidth), 0, end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x);
                        platform = Object.Instantiate(movingPlatform, first, Quaternion.identity);
                        prev = (second.x - first.x) / speed;
                        platform.AddCheckpoint(second);
                    }
                }
                lastPoint = second;

            }
            else if (Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y) < m_maxWidth + m_minWidth || (end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x) < m_maxWidth + m_minWidth ||
                Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y) > m_minVerticalDist && (end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x) > m_minHorizontalDist & value <= 0.65f)
            {
                Vector3 second;
                //last 
                if (Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y) < m_maxWidth + m_minWidth || (end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x) < m_maxWidth + m_minWidth)
                {
                    second = first + new Vector3(end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x, Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y) * (height < 0 ? -1 : 1));
                    if (prev != 0)
                    {
                        platform = Object.Instantiate(movingPlatform, first, Quaternion.identity);
                        prev = 0;
                        platform.AddCheckpoint(second);
                    }
                    else
                    {
                        platform = Object.Instantiate(movingPlatform, first, Quaternion.identity);
                        prev = Mathf.Sqrt(Mathf.Pow(second.y - first.y, 2) + Mathf.Pow(second.x - first.x, 2)) / speed;
                        platform.AddCheckpoint(second);
                    }
                }
                else
                {
                    if (prev != 0)
                    {
                        int minN = Mathf.FloorToInt((prev + movingPlatform.GetWaitTime()) / (Mathf.Sqrt(Mathf.Pow(m_minWidth, 2) + Mathf.Pow(m_minWidth, 2)) / speed + movingPlatform.GetWaitTime()));
                        int maxN = Mathf.FloorToInt((Mathf.Clamp(Mathf.Sqrt(Mathf.Pow(m_maxWidth, 2) + Mathf.Pow(m_maxWidth, 2)), 0, Mathf.Min(Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y), end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x)) / speed
                            + movingPlatform.GetWaitTime()) / (prev + movingPlatform.GetWaitTime()));
                        int n;
                        if (Random.value > 0.5f)
                        {
                            n = Random.Range(1, minN + 1);
                            prev = (prev - (n - 1) * movingPlatform.GetWaitTime()) / n;
                        }
                        else
                        {
                            n = Random.Range(1, maxN + 1);
                            prev = n * prev + (n - 1) * movingPlatform.GetWaitTime();
                        }
                        float a = Random.Range(m_minWidth, prev * prev * speed * speed);
                        second = first + (Random.value > 0.5f ? new Vector3(Mathf.Sqrt(a), Mathf.Sqrt(prev * prev * speed * speed - a) * (height < 0 ? -1 : 1)) : new Vector3(Mathf.Sqrt(prev * prev * speed * speed - a), Mathf.Sqrt(a) * (height < 0 ? -1 : 1)));
                        if (n % 2 == 0)
                        {
                            platform = Object.Instantiate(movingPlatform, first, Quaternion.identity);
                            platform.AddCheckpoint(second);
                        }
                        else
                        {
                            platform = Object.Instantiate(movingPlatform, second, Quaternion.identity);
                            prev = 0;
                            platform.AddCheckpoint(first);
                        }
                    }
                    else
                    {
                        second = first +
                        new Vector3(Mathf.Clamp(Random.Range(m_minWidth / 2, m_maxWidth / 2), 0, end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x), (height < 0 ? -1 : 1) * Mathf.Clamp(Random.Range(m_minWidth / 2, m_maxWidth / 2), 0, Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y)));
                        platform = Object.Instantiate(movingPlatform, first, Quaternion.identity);
                        prev = Mathf.Sqrt(Mathf.Pow(second.y - first.y, 2) + Mathf.Pow(second.x - first.x, 2)) / speed;
                        platform.AddCheckpoint(second);
                    }
                }
                lastPoint = second;
            }
            else
            {
                float d;
                Vector3 fifth;
                if (prev != 0)
                {
                    int minN = Mathf.FloorToInt((prev + movingPlatform.GetWaitTime()) / (Mathf.Sqrt(Mathf.Pow(m_minWidth, 2) + Mathf.Pow(m_minWidth, 2)) / speed + movingPlatform.GetWaitTime()));
                    int maxN = Mathf.FloorToInt((Mathf.Clamp(Mathf.Sqrt(Mathf.Pow(m_maxWidth, 2) + Mathf.Pow(m_maxWidth, 2)), 0, Mathf.Min(Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y), end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x)) / speed
                        + movingPlatform.GetWaitTime()) / (prev + movingPlatform.GetWaitTime()));
                    int n;
                    if (Random.value > 0.5f)
                    {
                        n = Random.Range(1, minN + 1);
                        d = (prev - (n - 1) * movingPlatform.GetWaitTime()) / n * speed;
                    }
                    else
                    {
                        n = Random.Range(1, maxN + 1);
                        d = n * prev * speed + (n - 1) * movingPlatform.GetWaitTime() * speed;
                    }
                    Vector3 second = first + new Vector3(1, 1) * d / 4;
                    Vector3 third = first + new Vector3(1, 1) * d / 2;
                    Vector3 fourth = first + new Vector3(d * 3, d) / 4;
                    fifth = first + Vector3.right * d;
                    Vector3 sixth = first + new Vector3(d * 3, -d) / 4;
                    Vector3 seventh = first + new Vector3(d, -d) / 2;
                    Vector3 eighth = first + new Vector3(d, -d) / 4;
                    if (n % 2 == 0)
                    {
                        platform = Object.Instantiate(movingPlatform, first, Quaternion.identity);
                        prev = 4 * Mathf.Sqrt(d * d / 8) / speed;
                        platform.AddCheckpoint(second, false);
                        platform.AddCheckpoint(third, false);
                        platform.AddCheckpoint(fourth, false);
                        platform.AddCheckpoint(fifth);
                        platform.AddCheckpoint(sixth, false);
                        platform.AddCheckpoint(seventh, false);
                        platform.AddCheckpoint(eighth, false);
                    }
                    else
                    {
                        platform = Object.Instantiate(movingPlatform, fifth, Quaternion.identity);
                        prev = 0;
                        platform.AddCheckpoint(sixth, false);
                        platform.AddCheckpoint(seventh, false);
                        platform.AddCheckpoint(eighth, false);
                        platform.AddCheckpoint(first);
                        platform.AddCheckpoint(second, false);
                        platform.AddCheckpoint(third, false);
                        platform.AddCheckpoint(fourth, false);
                    }

                }
                else
                {
                    d = Mathf.Clamp(Random.Range(m_minWidth, m_maxWidth), 0, Mathf.Min(Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y), end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x));

                    Vector3 second = first + new Vector3(1, 1) * d / 4;
                    Vector3 third = first + new Vector3(1, 1) * d / 2;
                    Vector3 fourth = first + new Vector3(d * 3, d) / 4;
                    fifth = first + Vector3.right * d;
                    Vector3 sixth = first + new Vector3(d * 3, -d) / 4;
                    Vector3 seventh = first + new Vector3(d, -d) / 2;
                    Vector3 eighth = first + new Vector3(d, -d) / 4;

                    platform = Object.Instantiate(movingPlatform, first, Quaternion.identity);
                    prev = 4 * Mathf.Sqrt(d * d / 8) / speed;
                    platform.AddCheckpoint(second, false);
                    platform.AddCheckpoint(third, false);
                    platform.AddCheckpoint(fourth, false);
                    platform.AddCheckpoint(fifth);
                    platform.AddCheckpoint(sixth, false);
                    platform.AddCheckpoint(seventh, false);
                    platform.AddCheckpoint(eighth, false);
                }

                lastPoint = fifth;
            }

            platform.SetSpeed(speed);
            transition.AddEnviromentObject(platform.gameObject);
            lastPoint += Vector3.right * (m_levelTheme.m_movingPlatform.GetWidth() + 1);
        }
        // create bounds for player's fall
        room.AddEnviromentObject(CreateHorizontalBounds(start, lastPoint-Vector3.up, width, height));

        end = new Vector3Int(Mathf.CeilToInt(lastPoint.x) - 1, Mathf.CeilToInt(lastPoint.y) - 1);
        room.SetEndPosition(end);
        room.AddTransition(new Room(end, end));
        return room;
    }

    public override Room FillTransition(Room room, bool isInitial)
    {
        int width = Random.Range(m_minTransitionWidth, m_maxTransitionWidth);
        int height = Random.Range(m_minTransitionHeight, m_maxTransitionHeight);
        if (Random.value > 0.5)
        {
            height = -height;
        }
        Vector3Int end = new Vector3Int(room.GetEndPosition().x + width, room.GetEndPosition().y + height);
        Room transition = new Room(room.GetEndPosition(), end);

        MovingPlatform platform = Object.Instantiate(m_levelTheme.m_movingPlatform, room.GetEndPosition() + new Vector3(width * 1.0f / 2, 1 - m_levelTheme.m_movingPlatform.GetHeight()), Quaternion.identity).GetComponent<MovingPlatform>();
        platform.DisableAutoMovement();
        platform.AddCheckpoint(platform.transform.position + Vector3.up * height);
        transition.AddEnviromentObject(platform.gameObject);

        // create bounds for player's fall
        transition.AddEnviromentObject(CreateHorizontalBounds(room.GetEndPosition(),end,width,height));

        return transition;
    }
}
