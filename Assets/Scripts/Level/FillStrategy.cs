using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FillStrategy
{
    protected int m_maxRoomWidth = 60;
    protected int m_minRoomWidth = 12;
    protected int m_roomHeight = 5;

    protected int m_minTransitionWidth = 2;
    protected int m_maxTransitionWidth = 30;
    protected int m_maxTransitionHeight = 15;

    protected int m_minElevationHeight = 2;
    protected int m_maxElevationHeight = 20;
    int m_maxSlopeHeight = 8;

    protected WalkEnemy[] m_enemies;
    protected List<Trap> m_traps;
    protected MovingPlatform m_movingPlatform;
    protected Jumper m_jumper;
    protected GameObject m_checkpoint;


    protected readonly int m_minStraightSection = 5;

    LevelTheme m_levelTheme;

    protected AnimationCurve m_enemiesCount;
    protected AnimationCurve m_trapsCount;

    protected float m_enemySpawnChance = 0.4f;
    protected float m_trapSpawnChance = 0.5f;
    protected float m_slopeChance = 0.7f;


    readonly int m_playerJumpWidth = 10;
    readonly int m_playerJumpHeight = 5;

    public FillStrategy(LevelTheme levelTheme)
    {
        m_levelTheme = levelTheme;
    }

    public FillStrategy(LevelTheme levelTheme, AnimationCurve enemiesCount, AnimationCurve trapsCount, GameObject checkpoint)
    {
        m_levelTheme = levelTheme;
        m_enemiesCount = enemiesCount;
        m_trapsCount = trapsCount;
        m_checkpoint = checkpoint;
    }

    public FillStrategy(LevelTheme levelTheme, GameObject checkpoint, AnimationCurve trapsCount)
    {
        m_levelTheme = levelTheme;
        m_trapsCount = trapsCount;
        m_checkpoint = checkpoint;
    }

    public FillStrategy(LevelTheme levelTheme, GameObject checkpoint, AnimationCurve enemiesCount, AnimationCurve trapsCount, WalkEnemy[] enemies, List<Trap> traps) : this(levelTheme, enemiesCount, trapsCount, checkpoint)
    {
        m_enemies = enemies;
        m_traps = traps;
    }

    public FillStrategy(LevelTheme levelTheme, GameObject checkpoint, AnimationCurve enemiesCount, AnimationCurve trapsCount, WalkEnemy[] enemies, List<Trap> traps, MovingPlatform movingPlatform, Jumper jumper) : this(levelTheme, checkpoint, enemiesCount, trapsCount, enemies, traps)
    {
        m_movingPlatform = movingPlatform;
        m_jumper = jumper;
    }

    public FillStrategy(LevelTheme levelTheme, GameObject checkpoint, AnimationCurve trapsCount, List<Trap> traps) : this(levelTheme, checkpoint, trapsCount)
    {
        m_traps = traps;
    }

    public virtual Room FillRoom(Room transition)
    {
        Vector3Int start = transition.GetEndPosition();
        Vector3Int end = new Vector3Int(start.x + Random.Range(m_minRoomWidth, m_maxRoomWidth), start.y);
        Room room = new Room(end, transition);

        // int enemies = (int)m_enemiesCount.Evaluate(Time.time);

        //for (int i = 0; i < enemies; i++)
        //{
        //    if (Random.value > m_enemySpawnChance)
        //    {
        //        SpawnEnemy();
        //    }
        //}  

        CreateElevations(room);
        room.AddEnviromentObject(Object.Instantiate(m_checkpoint, room.GetEndPosition(), Quaternion.identity));
        room.DrawTiles();
        AddLandscape(room);

        return room;
    }

    public virtual Room FillTransition(Room previousRoom)
    {
        int width = Random.Range(m_minTransitionWidth, m_maxTransitionWidth);
        int height = Random.Range(-m_maxTransitionHeight, System.Math.Clamp(width / 3 * m_playerJumpHeight, 0, m_maxTransitionHeight));
        Vector3Int end = new Vector3Int(previousRoom.GetEndPosition().x + width, previousRoom.GetEndPosition().y + height);
        Room room = new Room(previousRoom.GetEndPosition(), end);
        if (width > m_playerJumpWidth || height > m_playerJumpHeight || height > -m_playerJumpHeight * 1.0f / m_playerJumpWidth * width + m_playerJumpHeight)
        {
            int localHeight, localWidth;
            Vector3Int lastPoint = room.GetStartPosition();
            while (lastPoint.x < room.GetEndPosition().x - 3)
            {
                int maxWidth = (int)(m_playerJumpHeight * 1.0f / (Mathf.Abs(room.GetEndPosition().y - lastPoint.y) * 1.0f / (room.GetEndPosition().x - lastPoint.x) + m_playerJumpHeight * 1.0f / m_playerJumpWidth));
                localWidth = Random.Range(m_minTransitionWidth, System.Math.Clamp(maxWidth, m_minTransitionWidth, System.Math.Clamp(m_playerJumpWidth, 0, room.GetEndPosition().x - lastPoint.x - 2)));
                localHeight = Random.Range((int)Mathf.Clamp(Mathf.Abs(room.GetEndPosition().y - lastPoint.y) * 1.0f / (room.GetEndPosition().x - lastPoint.x) * localWidth, 0, Mathf.Abs(room.GetEndPosition().y - lastPoint.y)),
                    (int)Mathf.Clamp(Mathf.Abs(room.GetEndPosition().y - lastPoint.y) * 1.0f / (room.GetEndPosition().x - lastPoint.x) * maxWidth, 0, Mathf.Abs(room.GetEndPosition().y - lastPoint.y)));
                if (height < 0)
                {
                    localHeight = -localHeight;
                }
                lastPoint = new Vector3Int(lastPoint.x + localWidth, lastPoint.y + localHeight);
                room.CreateLedge(lastPoint);
            }
        }

        BoxCollider2D bounds = new GameObject().AddComponent<BoxCollider2D>();
        bounds.gameObject.transform.position = new Vector3(previousRoom.GetEndPosition().x, (height > 0 ? previousRoom.GetEndPosition().y : end.y) - m_minStraightSection);
        bounds.isTrigger = true;
        bounds.gameObject.tag = "bounds";
        bounds.size = new Vector2(width, 0.5f);
        bounds.offset = new Vector2(width / 2, -0.5f);
        room.AddEnviromentObject(bounds.gameObject);

        room.DrawTiles();
        return room;
    }

    public Room FillStratRoom(Vector3Int start)
    {
        Vector3Int end = new Vector3Int(start.x + Random.Range(m_minRoomWidth, m_maxRoomWidth), start.y);
        Room room = new Room(start, end);
        room.MakePolygon(m_minStraightSection, start - m_minStraightSection * Vector3Int.up);
        CreateElevations(room);
        room.AddEnviromentObject(Object.Instantiate(m_checkpoint, room.GetEndPosition(), Quaternion.identity));
        room.DrawTiles();
        AddLandscape(room);
        return room;
    }

    void CreateElevations(Room room)
    {
        Vector3Int lastPoint = new Vector3Int(room.GetStartPosition().x + m_minStraightSection, room.GetStartPosition().y);
        while (room.GetEndPosition().x - lastPoint.x > m_minStraightSection)
        {
            if (Random.value > m_slopeChance && m_minElevationHeight * 2 + m_minStraightSection + lastPoint.x <= room.GetEndPosition().x - m_minStraightSection)
            {
                int slopeHeight = Random.Range(m_minElevationHeight, System.Math.Clamp((room.GetEndPosition().x - m_minStraightSection * 2 - lastPoint.x) / 2, m_minElevationHeight, m_maxSlopeHeight));
                int width = Random.Range(m_minStraightSection, room.GetEndPosition().x - m_minStraightSection - slopeHeight * 2 - lastPoint.x);
                room.CreateSlope(slopeHeight, width, lastPoint);
                lastPoint = new Vector3Int(lastPoint.x + slopeHeight * 2 + width + m_minStraightSection, lastPoint.y);
            }
            else
            {
                int height = Random.Range(m_minElevationHeight, m_maxElevationHeight);
                if (Random.value > 0.5f)
                {
                    height = -height;
                }

                int width = Random.Range(m_minStraightSection, room.GetEndPosition().x - lastPoint.x);

                room.CreateElevationOrLowland(height, width, lastPoint);

                if (height > m_playerJumpHeight)
                {
                    if (Random.value > 0.5f)
                    {
                        MovingPlatform platform = Object.Instantiate(m_movingPlatform, lastPoint, Quaternion.identity);
                        platform.AddCheckpoint(platform.transform.position + Vector3.up * height);
                        room.AddEnviromentObject(platform.gameObject);
                        platform.StartMovement();
                    }
                    else
                    {
                        Jumper jumper = Object.Instantiate(m_jumper, lastPoint, Quaternion.identity);
                        jumper.SetWallHeight(height);
                        room.AddEnviromentObject(jumper.gameObject);
                    }
                }

                lastPoint = new Vector3Int(lastPoint.x + width, lastPoint.y + height);
            }
        }

        room.AddTiles(m_minStraightSection, room.GetEndPosition().x - lastPoint.x, lastPoint);
    }

    void AddLandscape(Room room)
    {
        int width = 0;
        int grassWidth = 0;
        Vector3Int start = room.GetStartPosition();
        Vector3Int grassStart = room.GetStartPosition();
        foreach (var ground in room.GetGround())
        {
            if (!TileEditor.Instance.AddGrass(ground) && ground.y == start.y)
            {
                if (grassWidth == 0)
                {
                    grassStart = ground;
                }
                width++;
                grassWidth++;
            }
            else if (grassWidth > 0 || ground.y != start.y)
            {
                foreach (var obj in AddVegetation(grassWidth, grassStart,m_levelTheme.m_grass))
                {
                    room.AddEnviromentObject(obj.gameObject);
                }

                if (ground.y != start.y)
                {
                    foreach (var obj in AddVegetation(width, start, m_levelTheme.m_bushes))
                    {
                        room.AddEnviromentObject(obj.gameObject);
                    }
                    foreach (var obj in AddVegetation(width, start, m_levelTheme.m_trees))
                    {
                        room.AddEnviromentObject(obj.gameObject);
                    }
                    start =grassStart= ground;
                    width = grassWidth = 1;
                }
                else
                {
                    width++;
                    grassWidth = 0;
                }
            }
        }

        foreach (var obj in AddVegetation(grassWidth, grassStart, m_levelTheme.m_grass))
        {
            room.AddEnviromentObject(obj.gameObject);
        }
        foreach (var obj in AddVegetation(width, start, m_levelTheme.m_bushes))
        {
            room.AddEnviromentObject(obj.gameObject);
        }
        foreach (var obj in AddVegetation(width, start, m_levelTheme.m_trees))
        {
            room.AddEnviromentObject(obj.gameObject);
        }
    }

    List<EnviromentObject> AddVegetation(int width, Vector3Int start, EnviromentObject[] vegs)
    {
        if(width==0)
            return null;
        List<EnviromentObject> objs = new List<EnviromentObject>();
        int tries = 3;
        float length = 0;
        while (tries >= 0)
        {
            EnviromentObject obj = Object.Instantiate(vegs[Random.Range(0, vegs.Length)], start, Quaternion.identity).GetComponent<EnviromentObject>();
            Vector3 pos = new Vector3(Random.Range(start.x + obj.GetRightBorder(), start.x + width + obj.GetLeftBorder()), start.y);
            obj.transform.position = pos;
            obj.SetOffset();
            bool collides = objs.Any(o => o.transform.position.x > obj.transform.position.x &&
                obj.transform.position.x + obj.GetRightBorder() - o.transform.position.x - o.GetLeftBorder() > obj.GetWidth() / 2 ||
                o.transform.position.x < obj.transform.position.x &&
                o.transform.position.x + o.GetRightBorder() - obj.transform.position.x - obj.GetLeftBorder() > obj.GetWidth() / 2);
            if (collides || (Random.value > 0.65f&&length>width*1.0f/2)|| pos.x+ obj.GetRightBorder()> start.x + width|| pos.x + obj.GetLeftBorder()<start.x)
            {
                if (collides)
                    tries--;
                Object.Destroy(obj.gameObject);
                continue;
            }
            length += obj.GetWidth();
            objs.Add(obj);
        }
        return objs;
    }

    int GetEnemyNum()
    {
        List<float> chances = new List<float>();
        foreach (var enemy in m_enemies)
        {
            chances.Add(enemy.GetSpawnChance());
        }

        float value = Random.Range(0, chances.Sum());
        float sum = 0;
        for (int i = 0; i < chances.Count; i++)
        {
            sum += chances[i];
            if (sum < value)
            {
                return i;
            }
        }
        return chances.Count - 1;
    }

    void SpawnEnemy()
    {
        WalkEnemy enemy = m_enemies[GetEnemyNum()];
    }

}
