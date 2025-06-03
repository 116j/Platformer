using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileEditor : MonoBehaviour
{
    [SerializeField]
    TileChanger[] m_tileChangers;
    [SerializeField]
    List<TilePlaceAnalog> m_tileAnalogs;

    Tilemap m_ground;
    Tilemap m_slope;
    Tilemap m_walls;
    Tilemap m_notCollidable;

    Dictionary<TileBase, TileChanger> m_tileToChanger = new Dictionary<TileBase, TileChanger>();
    const int TILES_PER_FRAME = 20;

    int m_tilePaletteIndex;


    private void Awake()
    {
        m_ground = transform.GetChild(0).GetComponent<Tilemap>();
        m_slope = transform.GetChild(1).GetComponent<Tilemap>();
        m_walls = transform.GetChild(2).GetComponent<Tilemap>();
        m_notCollidable = transform.GetChild(3).GetComponent<Tilemap>();

        foreach (var changer in m_tileChangers)
        {
            foreach (var tile in changer.tiles)
            {
                m_tileToChanger.Add(tile, changer);
            }
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

        Debug.Log(tilePos);
        return false;

    }
    /// <summary>
    /// Reset if tiles are changed
    /// </summary>
    public void ClearTiles(HashSet<Vector3Int> tilePositions, bool async)
    {
        if (async)
        {
            StartCoroutine(ClearTilesAsync(tilePositions));
        }
        else
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
    }

    public IEnumerator ClearTilesAsync(HashSet<Vector3Int> tilePositions)
    {
        TileBase tile = null;
        int i = 0;
        foreach (var pos in tilePositions)
        {
            if (tile = GetTileFromTilemap(pos))
            {
                SetTile(m_tileToChanger[tile], null, pos);
                if (m_tileToChanger[tile].addGrass)
                {
                    m_notCollidable.SetTile(new Vector3Int(pos.x, pos.y + 1), null);
                }
                i++;
                if (i % TILES_PER_FRAME == 0)          // раз в 50 итераций Ч отдаЄм кадр
                    yield return null;
            }
        }
    }
    /// <summary>
    /// Adds tiles' positions to dictionary and sets tiles
    /// </summary>
    /// <param name="tilePositions"></param>
    public void SetTiles(HashSet<Vector3Int> tilePositions, System.Action callback, bool isInitial)
    {
        if (isInitial)
        {
            ChangeTiles(tilePositions, callback);
        }
        else
        {
            StartCoroutine(ChangeTilesAsync(tilePositions, callback));
        }
    }
    /// <summary>
    /// Sets tiles from dictionary
    /// </summary>
    public IEnumerator ChangeTilesAsync(HashSet<Vector3Int> tilePositions, System.Action callback)
    {
        Dictionary<Vector3Int, bool> tilePositionsUsage = new Dictionary<Vector3Int, bool>(tilePositions.Count);
        foreach (var pos in tilePositions)
            tilePositionsUsage[pos] = false;

        Dictionary<Vector3Int, TileBase> tilemap = new Dictionary<Vector3Int, TileBase>();
        int i = 0;

        foreach (var pos in tilePositions)
        {
            ChangeTile(pos, tilePositionsUsage, tilemap);
            i++;
            if (i % TILES_PER_FRAME == 0)          // раз в 50 итераций Ч отдаЄм кадр
                yield return null;
        }
        i = 0;
        foreach (var (pos, tile) in tilemap)
        {
            SetTile(m_tileToChanger[tile], tile, pos);
            i++;
            if (i % TILES_PER_FRAME == 0)          // раз в 50 итераций Ч отдаЄм кадр
                yield return null;
        }

        callback?.Invoke();
    }

    public void ChangeTiles(HashSet<Vector3Int> tilePositions, System.Action callback)
    {
        Dictionary<Vector3Int, bool> tilePositionsUsage = new Dictionary<Vector3Int, bool>(tilePositions.Count);
        foreach (var pos in tilePositions)
            tilePositionsUsage[pos] = false;

        Dictionary<Vector3Int, TileBase> tilemap = new Dictionary<Vector3Int, TileBase>();

        foreach (var pos in tilePositions)
        {
            ChangeTile(pos, tilePositionsUsage, tilemap);
        }

        foreach (var (pos, tile) in tilemap)
        {
            SetTile(m_tileToChanger[tile], tile, pos);
        }

        callback?.Invoke();
    }
    /// <summary>
    /// Search for suitble tile based on its surroundings
    /// </summary>
    /// <param name="position">grid position</param>
    void ChangeTile(Vector3Int position, Dictionary<Vector3Int, bool> tilePositionsUsage, Dictionary<Vector3Int, TileBase> tilemap)
    {
        TilePlaceAnalog analog = GetTileAnalog(position, tilePositionsUsage);
        if (!tilePositionsUsage[position] && analog != null)
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
                        if (tilePositionsUsage.ContainsKey(surPosition) && tilePositionsUsage[surPosition])
                        {
                            if (m_tileToChanger[tilemap[surPosition]].changeTiles[i + (int)Mathf.Pow(-1, i % 2)] != null)
                                tiles = m_tileToChanger[tilemap[surPosition]].changeTiles[i + (int)Mathf.Pow(-1, i % 2)].MatchesTiles(tiles);
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
                        tilemap.TryGetValue(surPosition, out surTile);

                        if (newChanger.analogTiles.surroundings[i])
                        {
                            //if surrounding's tiles doesnt match with change tile group or surrounding's tile is changed and change tile group doesnt contain it - change tile
                            if (tilePositionsUsage.ContainsKey(surPosition) && CheckSurrounding(tile, surPosition, i, tilePositionsUsage, tilemap))
                            {
                                tiles.Remove(tile);
                                break;
                            }

                        }
                        else if (tilePositionsUsage.ContainsKey(surPosition) && surTile == null)
                        {
                            tiles.Remove(tile);
                            break;
                        }
                        // if need to delete mot collidable tile
                        else if (i == 2 && tilemap.ContainsKey(surPosition) && surTile != null && !newChanger.addGrass)
                        {

                            tilemap[surPosition] = null;
                        }
                        //if need to add not collidable tile
                        else if (i == 2 && newChanger.addGrass)
                        {

                            tilemap[surPosition] = newChanger.changeTiles[i].GetTiles()[Random.Range(0, newChanger.changeTiles[i].GetTiles().Count)];
                        }
                    }

                    if (tiles.Contains(tile))
                    {
                        break;
                    }

                }
                tilePositionsUsage[position] = true;

                tilemap[position] = tile; // Ѕлокируем запись
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
    bool CheckSurrounding(TileBase tile, Vector3Int surPos, int surIndex, Dictionary<Vector3Int, bool> tilePositionsUsage, Dictionary<Vector3Int, TileBase> tilemap)
    {
        TilePlaceAnalog surAnalog = GetTileAnalog(surPos, tilePositionsUsage);
        TileBase surTile;

        tilemap.TryGetValue(surPos, out surTile);
        if (m_tileToChanger[tile].changeTiles[surIndex] != null)
        {
            //if surrounding's surrounding is already changed, check if it is in surrounding's change tile group
            if (tilePositionsUsage[surPos])
                return !m_tileToChanger[tile].changeTiles[surIndex].ContainsTile(surTile) ||
                (m_tileToChanger[surTile].changeTiles[surIndex + (int)Mathf.Pow(-1, surIndex % 2)] != null && !m_tileToChanger[surTile].changeTiles[surIndex + (int)Mathf.Pow(-1, surIndex % 2)].ContainsTile(tile));
            else
                // if surrounding's surrounding is not changed, search for any variants
                return m_tileToChanger[tile].changeTiles[surIndex].MatchesTiles(GetTilesAnalog(surAnalog)).Count == 0 ||
                GetTilesAnalog(surAnalog).All(a => a != null && m_tileToChanger[a].changeTiles[surIndex + (int)Mathf.Pow(-1, surIndex % 2)] != null && !m_tileToChanger[a].changeTiles[surIndex + (int)Mathf.Pow(-1, surIndex % 2)].ContainsTile(tile));
        }
        else
        {
            if (tilePositionsUsage[surPos])
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
    public TilePlaceAnalog GetTileAnalog(Vector3Int position, Dictionary<Vector3Int, bool> tilePositionsUsage)
    {
        // finds an analog witch surroundings matchs tile's surroundings
        TilePlaceAnalog result = m_tileAnalogs.Find(a =>
        {
            for (int i = 0; i < 4; i++)
            {
                if (tilePositionsUsage.ContainsKey(new Vector3Int(position.x + (int)Mathf.Pow(-1, i) * (3 - i) / 2, position.y + (int)Mathf.Pow(-1, i) * i / 2)) != a.surroundings[i])
                    return false;
            }
            return true;
        });
        // if analog has a double - specify surroundings
        if (result != null && result.placeAnalog != null)
        {
            bool isSlopeConditionMet = result.tilemapTag == "slope"
            && (tilePositionsUsage.ContainsKey(new Vector3Int(position.x + 1, position.y))
            && GetTileAnalog(new Vector3Int(position.x + 1, position.y), tilePositionsUsage) == GetTileAnalog(new Vector3Int(position.x, position.y - 1), tilePositionsUsage)
             || tilePositionsUsage.ContainsKey(new Vector3Int(position.x - 1, position.y))
             && GetTileAnalog(new Vector3Int(position.x - 1, position.y), tilePositionsUsage) == GetTileAnalog(new Vector3Int(position.x, position.y - 1), tilePositionsUsage)
             || tilePositionsUsage.ContainsKey(new Vector3Int(position.x - 1, position.y - 1))
             && result == GetTileAnalog(new Vector3Int(position.x - 1, position.y - 1), tilePositionsUsage)
             || tilePositionsUsage.ContainsKey(new Vector3Int(position.x + 1, position.y - 1))
             && result == GetTileAnalog(new Vector3Int(position.x + 1, position.y - 1), tilePositionsUsage))
             || result.tilemapTag == "ground";

            return isSlopeConditionMet ? result : result.placeAnalog;
        }

        return result;
    }
}