using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.ParticleSystem;

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
    Dictionary<Vector3Int, bool> m_tilePositionsUsage = new Dictionary<Vector3Int, bool>();

    int m_tilePaletteIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
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

    public void SetTheme(int num)
    {
        m_tilePaletteIndex = num;
    }

    public bool AddGrass(Vector3Int tilePos)
    {
        return m_tileToChanger[m_ground.GetTile(tilePos)].addGrass;
    }
    /// <summary>
    /// Reset if tiles are changed
    /// </summary>
    public void ClearTiles(List<Vector3Int> tilePositions)
    {
        TileBase tile = null;
        foreach (var pos in tilePositions)
        {
            if (tile = GetTile(pos))
            {
                SetTile(m_tileToChanger[tile], null, pos);
                if (m_tileToChanger[tile].addGrass)
                {
                    m_notCollidable.SetTile(new Vector3Int(pos.x, pos.y + 1), null);
                }
            }
        }
    }

    public void SetTiles(List<Vector3Int> tilePositions)
    {
        foreach (var pos in tilePositions)
        {
            m_tilePositionsUsage[pos] = false;
        }
        ChangeTiles();
    }

    public void SetTiles(List<Vector3Int> tilePositions, TilePlaceAnalog analog)
    {
        foreach (var pos in tilePositions)
        {
            m_destroyable.SetTile(pos, GetTilesAnalog(analog)[0]);
        }
    }

    public void ChangeTiles()
    {
        for (int i = 0; i < m_tilePositionsUsage.Count; i++)
        {
            StartCoroutine(nameof(ChangeTile), m_tilePositionsUsage.ElementAt(i).Key);
        }
        m_tilePositionsUsage.Clear();
    }
    /// <summary>
    /// Search for suitble tile based on its surroundings
    /// </summary>
    /// <param name="position">grid position</param>
    System.Collections.IEnumerator ChangeTile(Vector3Int position)
    {
        TilePlaceAnalog analog = GetTileAnalog(position);
        if (!m_tilePositionsUsage[position] && analog != null)
        {
            TileBase tile = null;
            Vector3Int surPosition;
            List<TileBase> tiles = new List<TileBase>(GetTilesAnalog(analog));

            for (int i = 0; i < analog.surroundings.Length; i++)
            {
                if (analog.surroundings[i])
                {
                    surPosition = new Vector3Int(position.x + (int)Mathf.Pow(-1, i) * (3 - i) / 2, position.y + (int)Mathf.Pow(-1, i) * i / 2);
                    // if surrounding is changed - get its changed tile group for current tile and match with analog
                    if (m_tilePositionsUsage.ContainsKey(surPosition) && m_tilePositionsUsage[surPosition])
                    {
                        if (m_tileToChanger[GetTile(surPosition)].changeTiles[i + (int)Mathf.Pow(-1, i % 2)] != null)
                            tiles = m_tileToChanger[GetTile(surPosition)].changeTiles[i + (int)Mathf.Pow(-1, i % 2)].MatchesTiles(tiles);
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
                    yield return null;
                }
                TileChanger newChanger = m_tileToChanger[tile];

                for (int i = 0; i < newChanger.analogTiles.surroundings.Length; i++)
                {
                    surPosition = new Vector3Int(position.x + (int)Mathf.Pow(-1, i) * (3 - i) / 2, position.y + (int)Mathf.Pow(-1, i) * i / 2);
                    if (newChanger.analogTiles.surroundings[i])
                    {
                        //if surrounding's tiles doesnt match with change tile group or surrounding's tile is changed and change tile group doesnt contain it - change tile
                        if (m_tilePositionsUsage.ContainsKey(surPosition) && CheckSurrounding(tile, surPosition, i))
                        {
                            tiles.Remove(tile);
                            break;
                        }

                    }
                    else if (m_tilePositionsUsage.ContainsKey(surPosition) && m_notCollidable.GetTile(surPosition) == null)
                    {
                        tiles.Remove(tile);
                        break;
                    }
                    // if need to delete mot collidable tile
                    else if (i == 2 && m_notCollidable.GetTile(surPosition) != null && !newChanger.addGrass)
                    {
                        m_notCollidable.SetTile(surPosition, null);
                    }
                    //if need to add not collidable tile
                    else if (i == 2 && newChanger.addGrass)
                    {
                        m_notCollidable.SetTile(surPosition, newChanger.changeTiles[i].GetTiles()[Random.Range(0, newChanger.changeTiles[i].GetTiles().Count)]);
                    }
                }

                if (tiles.Contains(tile))
                {
                    break;
                }
            }
            if (tiles.Count == 0)
            {
                tiles = new List<TileBase>(GetTilesAnalog(analog));

                for (int i = 0; i < analog.surroundings.Length; i++)
                {
                    if (analog.surroundings[i])
                    {
                        surPosition = new Vector3Int(position.x + (int)Mathf.Pow(-1, i) * (3 - i) / 2, position.y + (int)Mathf.Pow(-1, i) * i / 2);
                        if (m_tilePositionsUsage.ContainsKey(surPosition) && m_tilePositionsUsage[surPosition])
                        {
                            if (m_tileToChanger[GetTile(surPosition)].changeTiles[i + (int)Mathf.Pow(-1, i % 2)] != null)
                                tiles = m_tileToChanger[GetTile(surPosition)].changeTiles[i + (int)Mathf.Pow(-1, i % 2)].MatchesTiles(tiles);
                        }
                    }
                }
            }
            m_tilePositionsUsage[position] = true;
            SetTile(m_tileToChanger[tile], tile, position);
        }
        yield return null;
    }


    bool CheckSurrounding(TileBase tile, Vector3Int surPos, int surIndex)
    {
        TilePlaceAnalog surAnalog = GetTileAnalog(surPos);
        TileBase surTile = GetTile(surPos);
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
    TileBase GetTile(Vector3Int position)
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

    public TilePlaceAnalog GetTileAnalog(Vector3Int position)
    {
        TilePlaceAnalog result = m_tileAnalogs.Find(a =>
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_tilePositionsUsage.ContainsKey(new Vector3Int(position.x + (int)Mathf.Pow(-1, i) * (3 - i) / 2, position.y + (int)Mathf.Pow(-1, i) * i / 2)) != a.surroundings[i])
                    return false;
            }
            return true;
        });
        //foreach (var analog in m_tileAnalogs)
        //{
        //    bool suitable = true;
        //    for (int i = 0; i < 4; i++)
        //    {
        //        Vector3Int surPosition = new Vector3Int(position.x + (int)Mathf.Pow(-1, i) * (3 - i) / 2, position.y + (int)Mathf.Pow(-1, i) * i / 2);
        //        if (analog.surroundings[i] != m_tilePositionsUsage.ContainsKey(surPosition))
        //        {
        //            suitable = false;
        //            break;
        //        }
        //    }

        //    if(suitable)
        //    {
        //        result = analog;
        //    }
        //}

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
