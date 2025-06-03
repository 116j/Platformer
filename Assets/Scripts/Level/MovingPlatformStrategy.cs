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

    enum Trajectory { Horizontal, Vertical, Diagonal, Circular }


    public MovingPlatformStrategy(LevelTheme levelTheme, AnimationCurve speed) : base(levelTheme)
    {
        m_speed = speed;
    }

    public override Room FillRoom(Room prevRoom, FillStrategy transitionStrategy)
    {
        Room transition = new Room(prevRoom.GetEndPosition(), prevRoom.GetEndPosition());

        Vector3Int start = prevRoom.GetEndPosition();
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
            float speed = m_speed.Evaluate(m_lvlBuilder.LevelProgress());
            Vector3 first = lastPoint;
            Vector3 second;
            MovingPlatform platform = movingPlatform;

            float hSpace = end.x - lastPoint.x + m_levelTheme.m_movingPlatform.GetOffset().x;  // 
            float vSpace = Mathf.Abs(end.y + 1 - m_levelTheme.m_movingPlatform.GetHeight() - lastPoint.y); // доступ по Y

            Trajectory currentTrajectory;
            if (vSpace > m_minVerticalDist
                && hSpace > m_minHorizontalDist)
            {
                float weightH = hSpace;                           // горизонталь Ч чем шире, тем чаще
                float weightV = vSpace;                           // вертикаль  Ч чем выше, тем чаще
                float weightD = Mathf.Min(vSpace, hSpace);         // диагональ Ч чем Ђквадратнееї, тем чаще
                float weightC = (height + vSpace) * 0.5f;           // кругова€ Ч растЄт с размерами комнаты

                float pick = Random.value * (weightV + weightH + weightD + weightC);
                if (pick < weightH)
                {
                    currentTrajectory = Trajectory.Horizontal;
                }
                else if (pick < weightH + weightV)
                {
                    currentTrajectory = Trajectory.Vertical;
                }
                else if (pick < weightV + weightH + weightD)
                {
                    currentTrajectory = Trajectory.Diagonal;
                }
                else
                {
                    currentTrajectory = Trajectory.Circular;
                }
            }
            else
            {
                if (vSpace > m_minVerticalDist
                && vSpace < m_maxWidth
                && hSpace < m_minHorizontalDist)
                {
                    currentTrajectory = Trajectory.Vertical;
                }
                else if (vSpace < m_minVerticalDist
                && hSpace > m_minHorizontalDist
                && hSpace < m_maxWidth)
                {
                    currentTrajectory = Trajectory.Horizontal;
                }
                else
                {
                    currentTrajectory = Trajectory.Diagonal;
                }
            }

            switch (currentTrajectory)
            {
                case Trajectory.Vertical:

                    if (prev != 0)
                    {
                        int minN = Mathf.FloorToInt((prev + movingPlatform.GetWaitTime()) / (m_minWidth / speed + movingPlatform.GetWaitTime()));
                        int maxN = Mathf.FloorToInt((Mathf.Min(m_maxWidth, vSpace) / speed + movingPlatform.GetWaitTime()) / (prev + movingPlatform.GetWaitTime()));
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

                        platform = PlacePlatform(movingPlatform, first, second, ref prev, speed, n % 2 != 0);
                    }
                    else
                    {
                        second = first + Vector3.up * (height < 0 ? -1 : 1) * Mathf.Min(Random.Range(m_minWidth, m_maxWidth), vSpace);
                        platform = AddPlatform(movingPlatform, first, second);
                        prev = Mathf.Abs(second.y - first.y) / speed;
                    }


                    lastPoint = second;
                    break;

                case Trajectory.Horizontal:

                    if (prev != 0)
                    {
                        int minN = Mathf.FloorToInt((prev + movingPlatform.GetWaitTime()) / (m_minWidth / speed + movingPlatform.GetWaitTime()));
                        int maxN = Mathf.FloorToInt((Mathf.Min(m_maxWidth, hSpace) / speed + movingPlatform.GetWaitTime()) / (prev + movingPlatform.GetWaitTime()));
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
                        platform = PlacePlatform(movingPlatform, first, second, ref prev, speed, n % 2 != 0);
                    }
                    else
                    {
                        second = first + Vector3.right * Mathf.Min(Random.Range(m_minWidth, m_maxWidth), hSpace);
                        platform = AddPlatform(movingPlatform, first, second);
                        prev = (second.x - first.x) / speed;
                    }

                    lastPoint = second;
                    break;

                case Trajectory.Diagonal:

                    if (prev != 0)
                    {
                        int minN = Mathf.FloorToInt((prev + movingPlatform.GetWaitTime()) / (Mathf.Sqrt(Mathf.Pow(m_minWidth, 2) + Mathf.Pow(m_minWidth, 2)) / speed + movingPlatform.GetWaitTime()));
                        int maxN = Mathf.FloorToInt((Mathf.Min(Mathf.Sqrt(Mathf.Pow(m_maxWidth, 2) + Mathf.Pow(m_maxWidth, 2)), Mathf.Min(vSpace, hSpace)) / speed
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
                            platform = AddPlatform(movingPlatform, first, second);
                        }
                        else
                        {
                            platform = AddPlatform(movingPlatform, second, first);
                            prev = 0;
                        }
                    }
                    else
                    {
                        second = first +
                        new Vector3(Mathf.Min(Random.Range(m_minWidth / 2, m_maxWidth / 2), hSpace), (height < 0 ? -1 : 1) * Mathf.Min(Random.Range(m_minWidth / 2, m_maxWidth / 2), vSpace));
                        platform = AddPlatform(movingPlatform, first, second);
                        prev = Mathf.Sqrt(Mathf.Pow(second.y - first.y, 2) + Mathf.Pow(second.x - first.x, 2)) / speed;
                    }

                    lastPoint = second;
                    break;

                case Trajectory.Circular:
                    float d;
                    Vector3 fifth;
                    bool reverse = false;
                    if (prev != 0)
                    {
                        int minN = Mathf.FloorToInt((prev + movingPlatform.GetWaitTime()) / (Mathf.Sqrt(Mathf.Pow(m_minWidth, 2) + Mathf.Pow(m_minWidth, 2)) / speed + movingPlatform.GetWaitTime()));
                        int maxN = Mathf.FloorToInt((Mathf.Min(Mathf.Sqrt(Mathf.Pow(m_maxWidth, 2) + Mathf.Pow(m_maxWidth, 2)), Mathf.Min(vSpace, hSpace)) / speed
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
                        reverse = n % 2 != 0;
                    }
                    else
                    {
                        d = Mathf.Min(Random.Range(m_minWidth, m_maxWidth), Mathf.Min(vSpace, hSpace));
                    }

                    second = first + new Vector3(1, 1) * d / 4;
                    Vector3 third = first + new Vector3(1, 1) * d / 2;
                    Vector3 fourth = first + new Vector3(d * 3, d) / 4;
                    fifth = first + Vector3.right * d;
                    Vector3 sixth = first + new Vector3(d * 3, -d) / 4;
                    Vector3 seventh = first + new Vector3(d, -d) / 2;
                    Vector3 eighth = first + new Vector3(d, -d) / 4;

                    if (reverse)
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
                    else
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

                    lastPoint = fifth;
                    break;
            }

            platform.SetSpeed(speed);
            room.AddEnviromentObject(platform.gameObject);
            room.CreatePlatform(Vector3Int.CeilToInt(lastPoint) -Vector3Int.right,2);
            lastPoint += Vector3.right * (m_levelTheme.m_movingPlatform.GetWidth() + 1);
        }
        // create bounds for player's fall

        end = new Vector3Int(Mathf.CeilToInt(lastPoint.x) - 1, Mathf.CeilToInt(lastPoint.y) - 1);
        room.SetEndPosition(end);
        room.AddTransition(new Room(end, end));

        room.AddEnviromentObject(CreateHorizontalBounds(start, end, width, height));
        CreateSideBound(room, height < 0);

        prevRoom.GetNextTransition().Clear(m_editor);
        prevRoom.AddTransition(transition);

        return room;
    }

    MovingPlatform AddPlatform(MovingPlatform prefab, Vector3 first, Vector3 second)
    {
        MovingPlatform platform = Object.Instantiate(prefab, first, Quaternion.identity);
        platform.AddCheckpoint(second);
        return platform;
    }

    MovingPlatform PlacePlatform(MovingPlatform prefab, Vector3 first, Vector3 second, ref float prev, float speed, bool cond)
    {
        MovingPlatform platform = AddPlatform(prefab, cond ? second : first, cond ? first : second);
        platform.SetSpeed(speed);

        // ƒобавл€ем чекпоинт в нужном пор€дке
        if (cond)
            platform.AddCheckpoint(first);
        else
            platform.AddCheckpoint(second);

        // ¬ычисл€ем новое prev
        prev = cond ? 0f : Vector3.Distance(first, second) / speed;
        return platform;
    }


    public override Room FillTransition(Room room)
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
        transition.AddEnviromentObject(CreateHorizontalBounds(transition.GetStartPosition(), end, width, height));

        return transition;
    }
}
