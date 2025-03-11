using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public static LevelBuilder Instance { get; private set; }

    [SerializeField]
    LevelTheme[] m_themes;
    [SerializeField]
    AudioClip[] m_backgroundMusic;

    [Header("Spawn Objects")]
    [SerializeField]
    TilePlaceAnalog m_destroyableTile;

    [Header("Spawn chances")]
    [SerializeField]
    AnimationCurve m_enemiesCountPerRoom;
    [SerializeField]
    AnimationCurve m_trapsCountPerRoom;
    [SerializeField]
    AnimationCurve m_movingPlatformSpeed;

    [SerializeField]
    Vector3Int m_startPosition;
    [SerializeField]
    int m_maxRoomsCount;

    PlayerController m_player;
    LevelTheme m_currentTheme;

    List<Room> m_rooms;
    List<FillStrategy> m_usedStrategies = new List<FillStrategy>();
    List<FillStrategy> m_strategies;
    float[] m_strategyWeights = { 0.3f, 0.0f, 0.0f, 0.0f };
    Room m_currentRoom;
    bool m_changeTransposer = false;
    bool m_transitionBounds = true;
    bool m_roomBounds = true;
    //Count of spawned rooms
    public int RoomsCount { get; private set; } = 1;

    AudioSource m_audio;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        int m_currentThemeNum = Random.Range(0, m_themes.Length);
        m_currentTheme = m_themes[m_currentThemeNum];
        TileEditor.Instance.SetTheme(m_currentTheme.m_themeNum);
        Instantiate(m_currentTheme.m_backgrounds[Random.Range(0, m_currentTheme.m_backgrounds.Length)], Camera.main.transform);

        m_strategies = new List<FillStrategy>
        {
            new FillStrategy(m_currentTheme,m_enemiesCountPerRoom,m_trapsCountPerRoom),
            new CeilStrategy(m_currentTheme),
            new GridStrategy(m_currentTheme),
            new MovingPlatformStrategy(m_currentTheme, m_movingPlatformSpeed),
            new DestroyableBrickStrategy(m_currentTheme,m_destroyableTile),
            new MazeStrategy(m_currentTheme, m_enemiesCountPerRoom,m_trapsCountPerRoom)
        };

        m_rooms = new List<Room>
        {
            m_strategies[0].FillStratRoom(m_startPosition,m_strategies[Random.Range(0, 4)])
        };
        m_usedStrategies.Add(m_strategies[0]);
        m_currentRoom = m_rooms[0];
        SpawnRoom();
        SpawnRoom();

        m_player.transform.position = m_startPosition + m_player.gameObject.GetComponent<SpawnValues>().GetOffset();
        m_audio = GetComponent<AudioSource>();
        m_audio.clip = m_backgroundMusic[Random.Range(0, m_backgroundMusic.Length)];
        m_audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_currentRoom != null && m_player.transform.position.x > m_currentRoom.GetEndPosition().x)
        {
            if (m_usedStrategies[m_rooms.IndexOf(m_currentRoom)] is GridStrategy || m_usedStrategies[m_rooms.IndexOf(m_currentRoom)] is MovingPlatformStrategy)
            {
                m_roomBounds = true;
                m_transitionBounds = false;
            }

            m_player.SetLevelCheckpoint(m_currentRoom.GetEndPosition());
            m_currentRoom = m_rooms[m_rooms.IndexOf(m_currentRoom) + 1];
            if (RoomsCount <= m_maxRoomsCount)
            {
                SpawnRoom();
            }
            ClearRoom();

            if (m_usedStrategies[m_rooms.IndexOf(m_currentRoom)] is GridStrategy || m_usedStrategies[m_rooms.IndexOf(m_currentRoom)] is MovingPlatformStrategy)
            {
                m_roomBounds = true;
                m_transitionBounds = false;
            }
        }
        //resize camera bounds and camera offset
        else if (m_currentRoom != null && m_player.transform.position.x < m_currentRoom.GetStartPosition().x && m_transitionBounds)
        {
            if (m_currentRoom.GetTransitionHeight() < 0 && !m_changeTransposer)
            {
                m_changeTransposer = !m_changeTransposer;
                m_player.ChangeTransposerHeight(m_changeTransposer);
            }
            m_transitionBounds = false;
            m_roomBounds = true;
            m_currentRoom.SetTransitionCameraBouns();
        }
        //resize camera bounds and camera offset
        else if (m_currentRoom != null && m_player.transform.position.x >= m_currentRoom.GetStartPosition().x && m_roomBounds)
        {
            if ((m_usedStrategies[m_rooms.IndexOf(m_currentRoom)] is GridStrategy || m_usedStrategies[m_rooms.IndexOf(m_currentRoom)] is MovingPlatformStrategy)
                && m_currentRoom.GetEndPosition().y < m_currentRoom.GetStartPosition().y)
            {
                if (!m_changeTransposer)
                {
                    m_changeTransposer = !m_changeTransposer;
                    m_player.ChangeTransposerHeight(m_changeTransposer);
                }
                m_currentRoom.SetCameraBounds();
            }
            else
            {
                if (m_changeTransposer)
                {
                    m_changeTransposer = !m_changeTransposer;
                    m_player.ChangeTransposerHeight(m_changeTransposer);
                }
                CameraBounds.Instance.SetHeight(m_player.transform.position, 1, false);
            }
            m_transitionBounds = true;
            m_roomBounds = false;
            //  m_currentRoom.SetCameraBounds();
        }
    }

    public void CatPetted(int cats)
    {
        m_strategies[0].CatPetted(cats);
    }

    public void ShopDestroyed()
    {
        m_strategies[0].ShopDestroyed();
    }
    /// <summary>
    /// Creates room from random strategy
    /// </summary>
    void SpawnRoom()
    {
        RoomsCount++;
        if (RoomsCount > m_maxRoomsCount)
        {
            m_usedStrategies.Add(m_strategies[0]);
            m_rooms.Add(m_strategies[0].FillFinalRoom(m_rooms.Last()));
        }
        else while (true)
            {
                FillStrategy s = m_strategies[GetStrategy()];
                if ((m_usedStrategies.Last() is GridStrategy || m_usedStrategies.Last() is MovingPlatformStrategy) &&
                    (s is GridStrategy || s is MovingPlatformStrategy))
                    continue;
                m_usedStrategies.Add(s);
                Room r = s.FillRoom(m_rooms.Last(), m_strategies[Random.Range(0, 4)]);
                if (r == null)
                    continue;
                ; m_rooms.Add(r);
                break;
            }
    }

    int GetStrategy()
    {
        float value = Random.Range(0, m_strategyWeights.Sum());
        float sum = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += m_strategyWeights[i];
            if (value < sum)
            {
                return i;
            }
        }
        return 3;
    }
    /// <summary>
    /// Removes first room if rooms count is larger than 4
    /// </summary>
    void ClearRoom()
    {
        if (m_rooms.IndexOf(m_currentRoom) >= 3)
        {
            m_rooms[0].Clear();
            m_rooms.RemoveAt(0);
            m_usedStrategies.RemoveAt(0);
            if (m_usedStrategies[0] is GridStrategy || m_usedStrategies[0] is MovingPlatformStrategy)
                m_player.SetRebornCheckpoint(m_rooms[1].GetStartPosition());
            else
                m_player.SetRebornCheckpoint(m_rooms[0].GetStartPosition());
        }
    }
}
