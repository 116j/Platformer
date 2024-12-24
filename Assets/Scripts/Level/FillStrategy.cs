using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Recorder.OutputPath;

public class FillStrategy
{
    protected int m_maxRoomWidth = 60;
    protected int m_minRoomWidth = 12;
    protected int m_roomHeight = 6;

    protected int m_minTransitionWidth = 2;
    protected int m_maxTransitionWidth = 30;
    protected int m_maxTransitionHeight = 15;

    protected int m_minElevationHeight = 2;
    protected int m_maxElevationHeight = 20;
    // min width of staright ground
    protected readonly int m_minStraightSection = 6;
    // min width for a section where wnwmy can walk
    protected readonly int m_minEnemyWidth = 8;
    protected readonly int m_maxSlopeHeight = 7;

    protected MovingPlatform m_movingPlatform;
    protected Jumper m_jumper;
    GameObject m_cat;
    GameObject m_shop;

    int m_enemiesPerLevel;
    int m_trapsPerLevel;
    int m_catsLeft;
    int m_catsSpawned;
    int m_trapsNum;
    // offset for traps spawn when there's a jumper or a platform
    float m_rightOffset;

    protected readonly LevelTheme m_levelTheme;

    protected AnimationCurve m_enemiesCount;
    protected AnimationCurve m_trapsCount;

    protected float m_slopeChance = 0.7f;

    protected readonly int m_playerJumpWidth = 9;
    protected readonly int m_playerJumpHeight = 6;
    protected readonly float m_playerWidth = 1f;

    protected bool m_wasLastEnemy;
    WalkEnemy m_lastEnemy;


    public FillStrategy(LevelTheme levelTheme)
    {
        m_levelTheme = levelTheme;
    }

    public FillStrategy(LevelTheme levelTheme, GameObject cat, AnimationCurve enemiesCount, AnimationCurve trapsCount)
    {
        m_levelTheme = levelTheme;
        m_enemiesCount = enemiesCount;
        m_trapsCount = trapsCount;
        m_cat = cat;
    }

    public FillStrategy(LevelTheme levelTheme, MovingPlatform movingPlatform, Jumper jumper)
    {
        m_levelTheme = levelTheme;
        m_movingPlatform = movingPlatform;
        m_jumper = jumper;
    }

    public FillStrategy(LevelTheme levelTheme, GameObject cat, AnimationCurve enemiesCount, AnimationCurve trapsCount, MovingPlatform movingPlatform, Jumper jumper, GameObject shop) : this(levelTheme, cat, enemiesCount, trapsCount)
    {
        m_shop = shop;
        m_movingPlatform = movingPlatform;
        m_jumper = jumper;
    }

    public void CatPetted(int cats)
    {
        m_catsSpawned-=cats;
    }
    /// <summary>
    /// Creates room with elevations and lowlands, adds landscape and draws tiles
    /// </summary>
    /// <param name="prevRoom">previous room</param>
    /// <param name="transitionStrategy">strategy to create the transition between this and next rooms</param>
    /// <returns>filled room</returns>
    public virtual Room FillRoom(Room prevRoom, FillStrategy transitionStrategy)
    {
        Vector3Int start = prevRoom.GetTransition().GetEndPosition();
        Vector3Int end = new Vector3Int(start.x + Random.Range(m_minRoomWidth, m_maxRoomWidth), start.y);
        //width of start straight section
        int startWidth = Random.Range(m_minStraightSection, end.x - start.x);
        Room room = new Room(end, startWidth, prevRoom.GetTransition());

        m_enemiesPerLevel = (int)m_enemiesCount.Evaluate(LevelBuilder.Instance.RoomsCount);
        m_trapsPerLevel = (int)m_trapsCount.Evaluate(LevelBuilder.Instance.RoomsCount);
        m_catsLeft =UIController.Instance.AllHerats - UIController.Instance.CurrentHearts - m_catsSpawned;

        SpawnEnemyOrTrap(room, startWidth, int.MaxValue, start);
        CreateElevations(room, start + startWidth * Vector3Int.right, true);
        room.AddTransition(transitionStrategy.FillTransition(room));
        room.DrawTiles();
        AddLandscape(room, int.MaxValue, true);

        return room;
    }
    /// <summary>
    /// Creates transition from room
    /// </summary>
    /// <param name="room"></param>
    public virtual Room FillTransition(Room room)
    {
        int width = Random.Range(m_minTransitionWidth, m_maxTransitionWidth);
        int height = Random.Range(-m_maxTransitionHeight, Mathf.Clamp(width / 3 * m_playerJumpHeight, 0, m_maxTransitionHeight));
        Vector3Int end = new Vector3Int(room.GetEndPosition().x + width, room.GetEndPosition().y + height);
        Room transition = new Room(room.GetEndPosition(), end);

        // if transition width and height are too large for player's jump - create ledges
        if (width > m_playerJumpWidth || height > m_playerJumpHeight || Mathf.Abs(height) > GetJumpHeight(width))
        {
            int localHeight, localWidth;
            Vector3Int lastPoint = transition.GetStartPosition();
            while (lastPoint.x < end.x - 3)
            {
                // max width for ledge where its max height for jump and its located on the straight line between the transition's start and the end
                int maxWidth = (int)(m_playerJumpHeight * 1.0f / (Mathf.Abs(end.y - lastPoint.y) * 1.0f / (end.x - lastPoint.x) + m_playerJumpHeight * 1.0f / m_playerJumpWidth));
                localWidth = Random.Range(m_minTransitionWidth, Mathf.Clamp(maxWidth, m_minTransitionWidth, Mathf.Clamp(m_playerJumpWidth, 0, end.x - lastPoint.x - 2)));
                // height between the point of the local width and the max width on the straifht line between the end of the transition and the current point 
                localHeight = Random.Range((int)Mathf.Clamp(Mathf.Abs(end.y - lastPoint.y) * 1.0f / (end.x - lastPoint.x) * localWidth, 0, Mathf.Abs(end.y - lastPoint.y)),
                    (int)Mathf.Clamp(Mathf.Abs(end.y - lastPoint.y) * 1.0f / (end.x - lastPoint.x) * maxWidth, 0, Mathf.Abs(end.y - lastPoint.y)));
                if (height < 0)
                {
                    localHeight = -localHeight;
                }
                lastPoint = new Vector3Int(lastPoint.x + localWidth, lastPoint.y + localHeight);
                transition.CreateLedge(lastPoint);
            }
        }
        // create bounds for player's fall
        BoxCollider2D bounds = new GameObject().AddComponent<BoxCollider2D>();
        bounds.gameObject.transform.position = new Vector3(room.GetEndPosition().x, (height > 0 ? room.GetEndPosition().y : end.y) - m_roomHeight);
        bounds.isTrigger = true;
        bounds.gameObject.tag = "bounds";
        bounds.size = new Vector2(width + 1, 0.5f);
        bounds.offset = new Vector2((width + 1) / 2, -0.5f);
        transition.AddEnviromentObject(bounds.gameObject);

        transition.DrawTiles();
        AddLandscape(transition, int.MaxValue, false);
        return transition;
    }

    protected int GetJumpWidth(int height)
    {
        return Mathf.CeilToInt(-(Mathf.Abs(height) - m_playerJumpHeight) * 1.0f / m_playerJumpHeight * m_playerJumpWidth);
    }

    protected int GetJumpHeight(int width)
    {
        return Mathf.CeilToInt(-m_playerJumpHeight * 1.0f / m_playerJumpWidth * width + m_playerJumpHeight);
    }

    /// <summary>
    /// Creates start room from start position
    /// </summary>
    /// <param name="start">start position of the room</param>
    /// <param name="transitionStrategy">strategy to create the transition between this and next rooms</param>
    /// <returns>filled room</returns>
    public Room FillStratRoom(Vector3Int start, FillStrategy transitionStrategy)
    {
        Vector3Int end = new Vector3Int(start.x + Random.Range(m_minRoomWidth, m_maxRoomWidth), start.y);
        Room room = new Room(start, end, new Room(start, start));
        //width of start straight section
        int startWidth = Random.Range(m_minStraightSection, end.x - start.x);
        // creates polygon with start width
        room.MakePolygon(startWidth, start);
        CreateElevations(room, start + startWidth * Vector3Int.right, false);
        room.AddTransition(transitionStrategy.FillTransition(room));
        room.DrawTiles();
        AddLandscape(room, int.MaxValue, true);
        return room;
    }
    /// <summary>
    /// Creates elevations and lowlands for the room, adds enemies and traps
    /// </summary>
    /// <param name="room"></param>
    /// <param name="lastPoint">last point of the room's straight section</param>
    /// <param name="spawnEnemyOrTrap"></param>
    protected void CreateElevations(Room room, Vector3Int lastPoint, bool spawnEnemyOrTrap)
    {
        int width = 0;
        while (room.GetEndPosition().x - lastPoint.x > m_minStraightSection)
        {
            m_rightOffset = 0f;
            //if slopeChance and the remaining distance is enought for a slope
            if (Random.value > m_slopeChance && m_minElevationHeight * 2 + m_minStraightSection + lastPoint.x <= room.GetEndPosition().x - m_minStraightSection)
            {
                int slopeHeight = Random.Range(m_minElevationHeight, Mathf.Clamp((room.GetEndPosition().x - m_minStraightSection * 2 - lastPoint.x) / 2, m_minElevationHeight, m_maxSlopeHeight));
                width = Random.Range(m_minStraightSection, room.GetEndPosition().x - m_minStraightSection - slopeHeight * 2 - lastPoint.x);
                room.CreateSlope(slopeHeight, width, lastPoint);
                // spawn enemies or traps on the slope
                if (spawnEnemyOrTrap)
                    SpawnEnemyOrTrap(room, width - 2, int.MaxValue, new Vector3(lastPoint.x + slopeHeight + 1, lastPoint.y + slopeHeight));
                // spawn enemies or traps behind the slope
                if (spawnEnemyOrTrap && room.GetEndPosition().x - lastPoint.x - width > m_minStraightSection)
                    SpawnEnemyOrTrap(room, m_minStraightSection, int.MaxValue, new Vector3(lastPoint.x + slopeHeight * 2 + width, lastPoint.y));

                lastPoint = new Vector3Int(lastPoint.x + slopeHeight * 2 + width + m_minStraightSection, lastPoint.y);
                width = m_minStraightSection;
            }
            else
            {
                int height = Random.Range(m_minElevationHeight, m_maxElevationHeight);
                if (Random.value > 0.5f)
                {
                    height = -height;
                }
                width = Random.Range(m_minStraightSection, room.GetEndPosition().x - lastPoint.x);

                room.CreateElevationOrLowland(height, width, lastPoint);
                // create a moving platform or a jumper if the height is higher than player's jump height
                if (height > m_playerJumpHeight)
                {
                    if (Random.value > 0.35f)
                    {
                        SpawnValues spawn = m_movingPlatform.gameObject.GetComponent<SpawnValues>();
                        MovingPlatform platform = Object.Instantiate(m_movingPlatform, lastPoint + spawn.GetOffset()+Vector3.up * (height - 2 + spawn.GetHeight()), Quaternion.identity);
                        m_rightOffset = spawn.GetOffset().x;
                        platform.AddCheckpoint(lastPoint + spawn.GetOffset());
                        room.AddEnviromentObject(platform.gameObject);
                        if (spawnEnemyOrTrap&&m_wasLastEnemy)
                        {
                            platform.DisableAutoMovement();
                            m_lastEnemy.ConnectPlatform(platform);
                        }                       
                    }
                    else
                    {
                        Jumper jumper = Object.Instantiate(m_jumper, lastPoint, Quaternion.identity);
                        m_rightOffset = jumper.GetOffset().x;
                        jumper.SetWallHeight(height);
                        room.AddEnviromentObject(jumper.gameObject);
                    }
                }
                if (spawnEnemyOrTrap && room.GetEndPosition().x - lastPoint.x - width > m_minStraightSection)
                    SpawnEnemyOrTrap(room, width, int.MaxValue, lastPoint + Vector3Int.up * height);
                lastPoint = new Vector3Int(lastPoint.x + width, lastPoint.y + height);
            }
        }
        // add remining tiles for the room
        room.AddTiles(m_roomHeight, room.GetEndPosition().x - lastPoint.x, lastPoint);
        if (spawnEnemyOrTrap && width > 0)
            SpawnEnemyOrTrap(room, room.GetEndPosition().x - lastPoint.x + width, int.MaxValue, lastPoint - Vector3Int.right * width);
    }
    /// <summary>
    /// Adds landscape to the room's ground
    /// </summary>
    /// <param name="room"></param>
    /// <param name="height">max vegetation height</param>
    /// <param name="addTrees"></param>
    protected void AddLandscape(Room room, int height, bool addTrees)
    {
        int width = 0;
        int grassWidth = 0;
        Vector3Int start = room.GetStartPosition();
        Vector3Int grassStart = room.GetStartPosition();
        foreach (var ground in room.GetGround())
        {
            // if tile has grass - don't add one
            if (!TileEditor.Instance.AddGrass(ground) && ground.y == start.y && ground.x == start.x + width)
            {
                if (grassWidth == 0)
                {
                    grassStart = ground;
                }
                width++;
                grassWidth++;
            }
            // if a ground straight section is over - add vegetation
            else if (grassWidth > 0 || ground.y != start.y || ground.x != start.x + width)
            {
                foreach (var obj in AddVegetation(grassWidth, height, grassStart, m_levelTheme.grass))
                {
                    room.AddEnviromentObject(obj.gameObject);
                }

                if (ground.y != start.y || ground.x != start.x + width)
                {
                    foreach (var obj in AddVegetation(width, height, start, m_levelTheme.bushes))
                    {
                        room.AddEnviromentObject(obj.gameObject);
                    }
                    if (addTrees)
                        foreach (var obj in AddVegetation(width, height, start, m_levelTheme.trees))
                        {
                            room.AddEnviromentObject(obj.gameObject);
                        }

                    start = grassStart = ground;
                    width = grassWidth = 1;
                }
                else
                {
                    width++;
                    grassWidth = 0;
                }
            }
        }

        foreach (var obj in AddVegetation(grassWidth, height, grassStart, m_levelTheme.grass))
        {
            room.AddEnviromentObject(obj.gameObject);
        }
        foreach (var obj in AddVegetation(width, height, start, m_levelTheme.bushes))
        {
            room.AddEnviromentObject(obj.gameObject);
        }
        if (addTrees)
            foreach (var obj in AddVegetation(width, height, start, m_levelTheme.trees))
            {
                room.AddEnviromentObject(obj.gameObject);
            }
    }
    /// <summary>
    /// Adds vegetation on the straight section of the ground
    /// </summary>
    /// <param name="width">width of the straight section</param>
    /// <param name="height">max veg's height</param>
    /// <param name="start">start of the straight section</param>
    /// <param name="vegs">array of the vegetation</param>
    /// <returns></returns>
    protected List<EnviromentObject> AddVegetation(int width, int height, Vector3Int start, EnviromentObject[] vegs)
    {
        List<EnviromentObject> objs = new List<EnviromentObject>();
        if (width == 0)
            return objs;
        // tries for respawn
        int tries = width + 2;
        float length = 0;
        while (tries >= 0)
        {
            EnviromentObject obj = Object.Instantiate(vegs[Random.Range(0, vegs.Length)], start, Quaternion.identity).GetComponent<EnviromentObject>();
            Vector3 pos = new Vector3(Random.Range(start.x + obj.GetRightBorder(), start.x + width + obj.GetLeftBorder()), start.y);
            obj.transform.position = pos + obj.GetOffset();
            // if obj collides other objs for more than 1/3 of its width
            bool collides = objs.Any(o => o.transform.position.x > obj.transform.position.x &&
                obj.transform.position.x + obj.GetRightBorder() - o.transform.position.x - o.GetLeftBorder() > obj.GetWidth() / 3 ||
                o.transform.position.x < obj.transform.position.x &&
                o.transform.position.x + o.GetRightBorder() - obj.transform.position.x - obj.GetLeftBorder() > obj.GetWidth() / 3);

            if (obj.GetHeight() > height || collides || (Random.value > 0.65f && length > width * 1.0f / 2) || pos.x + obj.GetRightBorder() > start.x + width || pos.x + obj.GetLeftBorder() < start.x)
            {
                tries--;
                Object.Destroy(obj.gameObject);
                continue;
            }
            length += obj.GetWidth();
            objs.Add(obj);
        }
        return objs;
    }
    /// <summary>
    /// Get random enemy based on their spawn chances
    /// </summary>
    /// <returns></returns>
    int GetEnemyNum()
    {
        List<float> chances = new List<float>();
        foreach (var enemy in m_levelTheme.enemies)
        {
            chances.Add(enemy.gameObject.GetComponent<WalkEnemy>().GetSpawnChance());
        }

        float value = Random.Range(0, chances.Sum());
        float sum = 0;
        for (int i = 0; i < chances.Count; i++)
        {
            sum += chances[i];
            if (value < sum)
            {
                return i;
            }
        }
        return chances.Count - 1;
    }
    /// <summary>
    /// Spawns enemy or traps
    /// </summary>
    /// <param name="room"></param>
    /// <param name="sectionWidth"></param>
    /// <param name="height">max trap height</param>
    /// <param name="startPos"></param>
    protected void SpawnEnemyOrTrap(Room room, int sectionWidth, int height, Vector3 startPos)
    {
        if (Random.value>0.6f&&m_catsLeft > 0 && m_rightOffset != m_jumper.GetOffset().x)
        {
            room.AddEnviromentObject(Object.Instantiate(m_cat, new Vector3(Random.Range(startPos.x, startPos.x + sectionWidth + m_rightOffset), startPos.y), Quaternion.identity));
            m_catsLeft--;
            m_catsSpawned++;
            m_wasLastEnemy = false;
        }
        else if (m_rightOffset != m_jumper.GetOffset().x && sectionWidth > m_minEnemyWidth && m_enemiesPerLevel > 0 && Random.value < (m_enemiesPerLevel / m_enemiesCount.Evaluate(LevelBuilder.Instance.RoomsCount)))
        {
            SpawnValues enemy = m_levelTheme.enemies[GetEnemyNum()];
            Vector3 pos = new Vector3(Random.Range(startPos.x - enemy.GetLeftBorder(), startPos.x + sectionWidth - enemy.GetRightBorder() + m_rightOffset), startPos.y);
            m_lastEnemy = Object.Instantiate(m_levelTheme.enemies[GetEnemyNum()], pos, Quaternion.identity).GetComponent<WalkEnemy>();
            m_wasLastEnemy = true;
            room.AddEnviromentObject(m_lastEnemy.gameObject);
            m_enemiesPerLevel--;
        }
        else if (m_trapsPerLevel > 0 && Random.value < (m_trapsPerLevel / m_trapsCount.Evaluate(LevelBuilder.Instance.RoomsCount)) && sectionWidth > 0)
        {
            List<Trap> traps = new List<Trap>();
            while (traps.Count == 0)
            {
                Trap trap = m_levelTheme.floorTraps[Random.Range(0, m_levelTheme.floorTraps.Length)];
                trap.SetTrapNum();
                if (trap.GetWidth() > sectionWidth || trap.GetHeight() > height)
                {
                    continue;
                }
                m_trapsNum = Random.Range(1, (int)(sectionWidth / (trap.GetWidth() + m_playerWidth)) + 1);
                float rightBorder = startPos.x + sectionWidth + m_rightOffset;
                float leftBorder = startPos.x + m_playerWidth;

                if (trap.GetAttackDirection() == Vector3.right || trap.GetAttackDirection() == Vector3.forward)
                {
                    // if trap shoots - crop borders
                    if (trap.GetWidth() > sectionWidth / 3)
                    {
                        m_trapsNum = 1;
                    }

                    if (trap.GetAttackDirection() == Vector3.forward)
                    {
                        rightBorder -= sectionWidth / 2;

                    }
                    else
                    {
                        leftBorder += sectionWidth / 2;
                    }
                }
                // if trap is serial - set a series of traps
                if (trap.IsSeries())
                {
                    m_trapsNum = Random.Range(1, (int)Mathf.Clamp(-(trap.GetHeight() - m_playerJumpHeight) * m_playerJumpWidth * 1.0f / m_playerJumpHeight, 1, (rightBorder - trap.GetRightBorder() - leftBorder + trap.GetLeftBorder()) / trap.GetWidth()));
                    Vector3 pos = new Vector3(Random.Range(leftBorder - trap.GetRightBorder(), rightBorder - trap.GetRightBorder() - m_trapsNum * trap.GetWidth()), startPos.y);
                    for (int i = 0; i < m_trapsNum; i++)
                    {
                        traps.Add(Object.Instantiate(trap, pos + i * trap.GetWidth() * Vector3.right, Quaternion.identity));
                    }
                }
                else
                {
                    SpawnTrap(leftBorder, rightBorder, startPos.y, trap, traps);
                }

            }

            foreach (var trap in traps)
            {
                room.AddEnviromentObject(trap.gameObject);
            }
            m_wasLastEnemy = false;
            m_trapsPerLevel--;
        }
    }

    void SpawnTrap(float leftBorder, float rightBorder, float posY, Trap trap, List<Trap> traps)
    {
        if (m_trapsNum == 0 || trap.GetWidth() > rightBorder - leftBorder)
            return;
        float posX = Random.Range(leftBorder - trap.GetLeftBorder(), rightBorder - trap.GetRightBorder());
        m_trapsNum--;
        traps.Add(Object.Instantiate(trap, new Vector3(posX, posY), Quaternion.identity));

        SpawnTrap(leftBorder, posX + trap.GetLeftBorder() - m_playerWidth, posY, trap, traps);
        SpawnTrap(posX + trap.GetRightBorder() + m_playerWidth, rightBorder, posY, trap, traps);
    }
}
