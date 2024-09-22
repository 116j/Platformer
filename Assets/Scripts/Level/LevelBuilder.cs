using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField]
    LevelTheme[] m_themes;

    [Header("Spawn Objects")]    
    [SerializeField]
    Background[] m_backgrounds;
    [SerializeField]
    List<Trap> m_floorTraps;
    [SerializeField]
    List<Trap> m_ceilTraps;
    [SerializeField]
    WalkEnemy[] m_enemies;
    [SerializeField]
    Cat m_cat;
    [SerializeField]
    MovingPlatform m_movingPlatform;
    [SerializeField]
    Jumper m_jumper;
    [SerializeField]
    TilePlaceAnalog m_destroyableTile;
    [SerializeField]
    GameObject m_checkpoint;

    [Header("Spawn chances")]
    [SerializeField]
    AnimationCurve m_enemiesCountPerRoom;
    [SerializeField]
    AnimationCurve m_trapsCountPerRoom;

    [SerializeField]
    Vector3Int m_startPosition;

    Transform m_player;
    LevelTheme m_currentTheme;

    List<Room> m_rooms;
    List<FillStrategy> m_strategies;
    Room m_currentRoom;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindWithTag("Player").transform;

        int m_currentThemeNum = Random.Range(0, m_themes.Length);
        m_currentTheme = m_themes[m_currentThemeNum];
        TileEditor.Instance.SetTheme(m_currentTheme.m_themeNum);
        Background background = Instantiate(m_backgrounds[Random.Range(0, m_backgrounds.Length)], Camera.main.transform);
        background.SetTheme(m_currentThemeNum);

        m_strategies = new List<FillStrategy>
        {
            new FillStrategy(m_currentTheme,m_checkpoint,m_enemiesCountPerRoom,m_trapsCountPerRoom, m_enemies, m_floorTraps,m_movingPlatform,m_jumper),
            new GridStrategy(m_currentTheme),
            new MazeStrategy(m_currentTheme,m_checkpoint, m_enemiesCountPerRoom,m_trapsCountPerRoom, m_enemies, m_ceilTraps.Concat(m_floorTraps).ToList()),
            new CeilStrategy(m_currentTheme,m_checkpoint,m_trapsCountPerRoom, m_ceilTraps),
            new MovingPlatformStrategy(m_currentTheme,m_movingPlatform),
            new DestroyableBrickStrategy(m_currentTheme,m_destroyableTile)
        };

        m_rooms = new List<Room>
        {
            m_strategies[0].FillStratRoom(m_startPosition)
        };
        m_currentRoom = m_rooms[0];
       SpawnRoom();
       SpawnRoom();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_currentRoom!=null&&m_player.position.x > m_currentRoom.GetEndPosition().x)
        {
            m_currentRoom = m_rooms[m_rooms.IndexOf(m_currentRoom) + 1];
            SpawnRoom();
            ClearRoom();
        }
    }

    void SpawnRoom()
    {
        Room transition =  m_strategies[Random.Range(0,m_strategies.Count)].FillTransition(m_rooms.Last());
        m_rooms.Add(m_strategies[/*Random.Range(0, m_strategies.Count)*/0].FillRoom(transition));
    }

    void ClearRoom()
    {
        if (m_rooms.Count > 4)
        {
            m_rooms[0].Clear();
            m_rooms.RemoveAt(0);
        }
    }

}
