using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Tilemaps;

public class TileEditor : MonoBehaviour
{
    public static TileEditor Instance { get; private set; }
    [SerializeField]
    TileChanger[] m_tileChangers;
    [SerializeField]
    List<TilePlaceAnalog> m_tileAnalogs;

    Tilemap m_ground;
    Tilemap m_slope;
    Tilemap m_walls;
    Tilemap m_notCollidable;
    Tilemap m_destroyable;

    Dictionary<TileBase, TileChanger> m_tileToChanger = new Dictionary<TileBase, TileChanger>();
    // if tiles are alr
    Dictionary<Vector3Int, bool> m_tilePositionsUsage = new Dictionary<Vector3Int, bool>();

    Dictionary<Vector3Int, TileBase> m_tilemap = new Dictionary<Vector3Int, TileBase>();

    int m_tilePaletteIndex;

    private Queue<Vector3Int> m_processingQueue = new Queue<Vector3Int>();
    private bool m_isProcessing = false;
    private const int INITIAL_TILES_PER_FRAME = 200; // Больше для начальной загрузки
    private const int NORMAL_TILES_PER_FRAME = 50;   // Меньше для плавности во время игры

    System.Action m_callback;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        m_ground = transform.GetChild(0).GetComponent<Tilemap>();
        m_slope = transform.GetChild(1).GetComponent<Tilemap>();
        m_walls = transform.GetChild(2).GetComponent<Tilemap>();
        m_notCollidable = transform.GetChild(3).GetComponent<Tilemap>();
        m_destroyable = transform.GetChild(4).GetComponent<Tilemap>();

        foreach (var changer in m_tileChangers)
        {
            foreach (var tile in changer.tiles)
            {
                m_tileToChanger.Add(tile, changer);
            }
        }
    }

    // Добавляем метод для ожидания
    public IEnumerator WaitForCompletion()
    {
        while (m_isProcessing)
        {
            yield return null;
        }
    }


    public void SetTheme(int num)
    {
        m_tilePaletteIndex = num;
    }
    /// <summary>
    /// If tile has a grass
    /// </summary>
    /// <param name="tilePos"></param>
    /// <returns></returns>
    public bool AddGrass(Vector3Int tilePos)
    {
        TileBase tile = m_ground.GetTile(tilePos);
        if (tile != null && m_tileToChanger.ContainsKey(tile))
        {
            return m_tileToChanger[tile].addGrass;
        }

        // 2. Если в tilemap еще не обновилось, проверяем кэш
        if (m_tilemap.TryGetValue(tilePos, out tile) && tile != null && m_tileToChanger.ContainsKey(tile))
        {
            return m_tileToChanger[tile].addGrass;
        }
        
            Debug.Log(tilePos);
            return false;
        
    }
    /// <summary>
    /// Reset if tiles are changed
    /// </summary>
    public void ClearTiles(List<Vector3Int> tilePositions)
    {
        TileBase tile = null;
        foreach (var pos in tilePositions)
        {
            if (tile = GetTileFromTilemap(pos))
            {
                SetTile(m_tileToChanger[tile], null, pos);
                if (m_tileToChanger[tile].addGrass)
                {
                    m_notCollidable.SetTile(new Vector3Int(pos.x, pos.y + 1), null);
                }
            }
        }
    }
    /// <summary>
    /// Adds tiles' positions to dictionary and sets tiles
    /// </summary>
    /// <param name="tilePositions"></param>
    public void SetTiles(List<Vector3Int> tilePositions, List<Vector3Int> ground, System.Action<List<Vector3Int>> callback, bool isInitialLoad = false)
    {
        Debug.Log("DrawTilesStart");
        foreach (var pos in tilePositions)
        {
            m_tilePositionsUsage[pos] = false;
            m_processingQueue.Enqueue(pos);
        }

        m_callback = ()=> callback?.Invoke(ground);

        if (!m_isProcessing)
        {
            StartCoroutine(ProcessTiles(isInitialLoad));

        }
    }


    private IEnumerator ProcessTiles(bool isInitialLoad)
    {
        m_isProcessing = true;
        m_tilemap.Clear();

        int tilesPerFrame = isInitialLoad ? INITIAL_TILES_PER_FRAME : NORMAL_TILES_PER_FRAME;

        while (m_processingQueue.Count > 0)
        {
            int processedThisFrame = 0;

            while (processedThisFrame < tilesPerFrame && m_processingQueue.Count > 0)
            {
                var pos = m_processingQueue.Dequeue();
                ChangeTile(pos);
                    processedThisFrame++;             
            }

            foreach (var (pos, tile) in m_tilemap)
            {
                SetTile(m_tileToChanger[tile], tile, pos);
            }

            if (!isInitialLoad)
                yield return null;
        }

        m_tilePositionsUsage.Clear();
        m_isProcessing = false;

        Debug.Log("Callback");
        m_callback.Invoke();
        m_callback = null;
    }

    /// <summary>
    /// Sets tile analog on tiles' positions
    /// </summary>
    /// <param name="tilePositions"></param>
    /// <param name="analog"></param>
    public void SetTiles(List<Vector3Int> tilePositions, TilePlaceAnalog analog)
    {
        foreach (var pos in tilePositions)
        {
            m_destroyable.SetTile(pos, GetTilesAnalog(analog)[0]);
        }
    }
    /// <summary>
    /// Sets tiles from dictionary
    /// </summary>
    //public void ChangeTiles(List<Vector3Int> tilePositions)
    //{
    //    m_tilemap.Clear();

    //    foreach (var pos in tilePositions)
    //    {
    //        StartCoroutine(nameof(ChangeTile), pos);
    //    }

    //   // lock (m_tilemapLock) // Блокируем доступ к m_tilemap
    //  //  {
    //        foreach (var (pos, tile) in m_tilemap)
    //        {
    //            SetTile(m_tileToChanger[tile], tile, pos);
    //        }
    //  //  }
    //    m_tilePositionsUsage.Clear();
    //}
    /// <summary>
    /// Search for suitble tile based on its surroundings
    /// </summary>
    /// <param name="position">grid position</param>
    void ChangeTile(Vector3Int position)
    {
        TilePlaceAnalog analog = GetTileAnalog(position);
        if (!m_tilePositionsUsage[position] && analog != null)
        {
            TileBase tile = null;
            Vector3Int surPosition;
            List<TileBase> tiles = new List<TileBase>(GetTilesAnalog(analog));

            try
            {
                for (int i = 0; i < analog.surroundings.Length; i++)
                {
                    if (analog.surroundings[i])
                    {
                        surPosition = new Vector3Int(position.x + (int)Mathf.Pow(-1, i) * (3 - i) / 2, position.y + (int)Mathf.Pow(-1, i) * i / 2);
                        // if surrounding is changed - get its changed tile group for current tile and match with analog
                        if (m_tilePositionsUsage.ContainsKey(surPosition) && m_tilePositionsUsage[surPosition])
                        {
                            if (m_tileToChanger[m_tilemap[surPosition]].changeTiles[i + (int)Mathf.Pow(-1, i % 2)] != null)
                                tiles = m_tileToChanger[m_tilemap[surPosition]].changeTiles[i + (int)Mathf.Pow(-1, i % 2)].MatchesTiles(tiles);
                        }
                    }
                }
                while (tiles.Count > 0)
                {
                    tile = tiles[Random.Range(0, tiles.Count)];
                    if (tile == null)
                    {
                        m_ground.SetTile(position, null);
                        m_walls.SetTile(position, null);
                        m_notCollidable.SetTile(position, null);
                        //return;
                        throw new System.ArgumentNullException();
                    }
                    TileChanger newChanger = m_tileToChanger[tile];

                    for (int i = 0; i < newChanger.analogTiles.surroundings.Length; i++)
                    {
                        surPosition = new Vector3Int(position.x + (int)Mathf.Pow(-1, i) * (3 - i) / 2, position.y + (int)Mathf.Pow(-1, i) * i / 2);
                        TileBase surTile;

                            m_tilemap.TryGetValue(surPosition, out surTile);
                        if (newChanger.analogTiles.surroundings[i])
                        {
                            //if surrounding's tiles doesnt match with change tile group or surrounding's tile is changed and change tile group doesnt contain it - change tile
                            if (m_tilePositionsUsage.ContainsKey(surPosition) && CheckSurrounding(tile, surPosition, i))
                            {
                                tiles.Remove(tile);
                                break;
                            }

                        }
                        else if (m_tilePositionsUsage.ContainsKey(surPosition) && surTile == null)
                        {
                            tiles.Remove(tile);
                            break;
                        }
                        // if need to delete mot collidable tile
                        else if (i == 2 && m_tilemap.ContainsKey(surPosition) && surTile != null && !newChanger.addGrass)
                        {
  
                                m_tilemap[surPosition] = null;
                        }
                        //if need to add not collidable tile
                        else if (i == 2 && newChanger.addGrass)
                        {
   
                                m_tilemap[surPosition] = newChanger.changeTiles[i].GetTiles()[Random.Range(0, newChanger.changeTiles[i].GetTiles().Count)];
                        }
                    }

                    if (tiles.Contains(tile))
                    {
                        break;
                    }
                }
                m_tilePositionsUsage[position] = true;

                    m_tilemap[position] = tile; // Блокируем запись
            }
            catch (System.ArgumentNullException)
            {
                Debug.Log("Null " + position);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="surPos"></param>
    /// <param name="surIndex"></param>
    /// <returns></returns>
    bool CheckSurrounding(TileBase tile, Vector3Int surPos, int surIndex)
    {
        TilePlaceAnalog surAnalog = GetTileAnalog(surPos);
        TileBase surTile;

            m_tilemap.TryGetValue(surPos, out surTile);
        if (m_tileToChanger[tile].changeTiles[surIndex] != null)
        {
            //if surrounding's surrounding is already changed, check if it is in surrounding's change tile group
            if (m_tilePositionsUsage[surPos])
                return !m_tileToChanger[tile].changeTiles[surIndex].ContainsTile(surTile) ||
                (m_tileToChanger[surTile].changeTiles[surIndex + (int)Mathf.Pow(-1, surIndex % 2)] != null && !m_tileToChanger[surTile].changeTiles[surIndex + (int)Mathf.Pow(-1, surIndex % 2)].ContainsTile(tile));
            else
                // if surrounding's surrounding is not changed, search for any variants
                return m_tileToChanger[tile].changeTiles[surIndex].MatchesTiles(GetTilesAnalog(surAnalog)).Count == 0 ||
                GetTilesAnalog(surAnalog).All(a => a != null && m_tileToChanger[a].changeTiles[surIndex + (int)Mathf.Pow(-1, surIndex % 2)] != null && !m_tileToChanger[a].changeTiles[surIndex + (int)Mathf.Pow(-1, surIndex % 2)].ContainsTile(tile));
        }
        else
        {
            if (m_tilePositionsUsage[surPos])
                return m_tileToChanger[surTile].changeTiles[surIndex + (int)Mathf.Pow(-1, surIndex % 2)] != null && !m_tileToChanger[surTile].changeTiles[surIndex + (int)Mathf.Pow(-1, surIndex % 2)].ContainsTile(tile);
            else
                return GetTilesAnalog(surAnalog).All(a => a != null && m_tileToChanger[a].changeTiles[surIndex + (int)Mathf.Pow(-1, surIndex % 2)] != null && !m_tileToChanger[a].changeTiles[surIndex + (int)Mathf.Pow(-1, surIndex % 2)].ContainsTile(tile));
        }
    }

    /// <summary>
    /// Gets analog tiles based on tilePalette
    /// </summary>
    /// <param name="analog"></param>
    /// <returns></returns>
    TileBase[] GetTilesAnalog(TilePlaceAnalog analog)
    {
        return m_tilePaletteIndex switch
        {
            0 => analog.dark,
            1 => analog.calm,
            _ => null,
        };
    }
    /// <summary>
    /// Gets tilemap tag from changer and sets tile
    /// </summary>
    /// <param name="changer">tile changer</param>
    /// <param name="tile">tile</param>
    /// <param name="position">grid position</param>
    void SetTile(TileChanger changer, TileBase tile, Vector3Int position)
    {
        if (m_ground.CompareTag(changer.analogTiles.tilemapTag))
        {
            m_ground.SetTile(position, tile);
        }
        else if (m_walls.CompareTag(changer.analogTiles.tilemapTag))
        {
            m_walls.SetTile(position, tile);
        }
        else if (m_notCollidable.CompareTag(changer.analogTiles.tilemapTag))
        {
            m_notCollidable.SetTile(position, tile);
        }
        else if (m_slope.CompareTag(changer.analogTiles.tilemapTag))
        {
            m_slope.SetTile(position, tile);
        }
    }
    /// <summary>
    /// Try get tile from each tilemap
    /// </summary>
    /// <param name="position">grid position</param>
    /// <returns>tile</returns>
    TileBase GetTileFromTilemap(Vector3Int position)
    {
        TileBase tile;
        if (tile = m_ground.GetTile(position))
        {
            return tile;
        }
        if (tile = m_walls.GetTile(position))
        {
            return tile;
        }
        if (tile = m_notCollidable.GetTile(position))
        {
            return tile;
        }
        if (tile = m_slope.GetTile(position))
        {
            return tile;
        }

        return null;
    }

    /// <summary>
    /// Finds tile analog based on tile's surroundings
    /// </summary>
    /// <param name="position">tile position</param>
    /// <returns></returns>
    public TilePlaceAnalog GetTileAnalog(Vector3Int position)
    {
        // finds an analog witch surroundings matchs tile's surroundings
        TilePlaceAnalog result = m_tileAnalogs.Find(a =>
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_tilePositionsUsage.ContainsKey(new Vector3Int(position.x + (int)Mathf.Pow(-1, i) * (3 - i) / 2, position.y + (int)Mathf.Pow(-1, i) * i / 2)) != a.surroundings[i])
                    return false;
            }
            return true;
        });
        // if analog has a double - specify surroundings
        if (result != null && result.placeAnalog != null)
        {
            bool isSlopeConditionMet = result.tilemapTag == "slope"
            && (m_tilePositionsUsage.ContainsKey(new Vector3Int(position.x + 1, position.y)) && GetTileAnalog(new Vector3Int(position.x + 1, position.y)) == GetTileAnalog(new Vector3Int(position.x, position.y - 1))
             || m_tilePositionsUsage.ContainsKey(new Vector3Int(position.x - 1, position.y)) && GetTileAnalog(new Vector3Int(position.x - 1, position.y)) == GetTileAnalog(new Vector3Int(position.x, position.y - 1))
             || m_tilePositionsUsage.ContainsKey(new Vector3Int(position.x - 1, position.y - 1)) && result == GetTileAnalog(new Vector3Int(position.x - 1, position.y - 1))
             || m_tilePositionsUsage.ContainsKey(new Vector3Int(position.x + 1, position.y - 1)) && result == GetTileAnalog(new Vector3Int(position.x + 1, position.y - 1)))
             || result.tilemapTag == "ground";

            return isSlopeConditionMet ? result : result.placeAnalog;
        }

        return result;
    }
}