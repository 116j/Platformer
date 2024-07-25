using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileEditor : MonoBehaviour
{
    [SerializeField]
    Vector2Int m_tilePositionMin;
    [SerializeField]
    Vector2Int m_tilePositionMax;
    [SerializeField]
    TileChanger[] m_tileChangers;

    Tilemap m_ground;
    Tilemap m_walls;
    Tilemap m_notCollidable;

    Dictionary<TileBase, TileChanger> m_tileToChanger = new Dictionary<TileBase, TileChanger>();
    bool[,] m_positions;

    public int m_tilePaletteIndex;
    public bool m_change;


    private void Start()
    {
        m_positions = new bool[m_tilePositionMax.x - m_tilePositionMin.x + 1, m_tilePositionMax.y - m_tilePositionMin.y + 1];
        m_ground = transform.GetChild(0).GetComponent<Tilemap>();
        m_walls = transform.GetChild(1).GetComponent<Tilemap>();
        m_notCollidable = transform.GetChild(2).GetComponent<Tilemap>();

        foreach (var changer in m_tileChangers)
        {
            foreach (var tile in changer.tiles)
            {
                m_tileToChanger.Add(tile, changer);
            }
        }

    }

    private void Update()
    {
        if (m_change)
        {
            m_change = false;
            ChangeTiles();
            ResetPositions();
        }
    }

    void ResetPositions()
    {
        for (int i = 0; i <= m_tilePositionMax.x - m_tilePositionMin.x; i++)
        {
            for (int j = 0; j <= m_tilePositionMax.y - m_tilePositionMin.y; j++)
            {
                m_positions[i, j] = false;
            }
        }
    }

    public void ChangeTiles()
    {
        for (int j = m_tilePositionMin.y; j <= m_tilePositionMax.y; j++)
        {
            for (int i = m_tilePositionMin.x; i <= m_tilePositionMax.x; i++)
            {
                // Debug.Log(i + ", " + j);

                ChangeTile(new Vector3Int(i, j));
            }
        }
    }

    void ChangeTile(Vector3Int position)
    {
        TileBase tile = GetTile(position);
        if (tile != null)
        {
            TileChanger changer = m_tileToChanger[tile];

            if (!m_positions[position.x - m_tilePositionMin.x, position.y - m_tilePositionMin.y])
            {
                Vector3Int surPosition;
                bool suitable = false;
                List<TileBase> tiles = new List<TileBase>(GetTilesAnalog(changer));

                for (int i = 0; i < changer.analogTiles.surroundings.Length; i++)
                {
                    if (changer.analogTiles.surroundings[i])
                    {
                        surPosition = new Vector3Int(position.x + (int)Mathf.Pow(-1, i) * (3 - i) / 2, position.y + (int)Mathf.Pow(-1, i) * i / 2);
                        if (surPosition.y <= m_tilePositionMax.y && m_positions[surPosition.x - m_tilePositionMin.x, surPosition.y - m_tilePositionMin.y])
                        {
                            if (m_tileToChanger[GetTile(surPosition)].changeTiles[i + (int)Mathf.Pow(-1, i % 2)] != null)
                                tiles = m_tileToChanger[GetTile(surPosition)].changeTiles[i + (int)Mathf.Pow(-1, i % 2)].MatchesTiles(tiles);
                        }
                    }
                }
                if (tiles.Count == 0)
                {
                    tiles = GetTilesAnalog(changer).ToList();
                    //for (int i = 0; i < changer.analogTiles.surroundings.Length; i++)
                    //{
                    //    if (changer.analogTiles.surroundings[i])
                    //    {
                    //        surPosition = new Vector3Int(position.x + (int)Mathf.Pow(-1, i) * (3 - i) / 2, position.y + (int)Mathf.Pow(-1, i) * i / 2);
                    //        if (surPosition.y <= m_tilePositionMax.y && m_positions[surPosition.x - m_tilePositionMin.x, surPosition.y - m_tilePositionMin.y])
                    //        {
                    //            if (m_tileToChanger[GetTile(surPosition)].changeTiles[i + (int)Mathf.Pow(-1, i % 2)] != null)
                    //                tiles = m_tileToChanger[GetTile(surPosition)].changeTiles[i + (int)Mathf.Pow(-1, i % 2)].MatchesTiles(tiles);
                    //        }
                    //    }
                    //}
                    //tiles = GetTilesAnalog(changer).ToList();
                }
                while (!suitable)
                {
                    if (tiles.Count == 0)
                    {
                        SetTile(changer, tile, position);
                        return;
                        //tiles = new List<TileBase>(GetTilesAnalog(changer));

                        //for (int i = 0; i < changer.analogTiles.surroundings.Length; i++)
                        //{
                        //    if (changer.analogTiles.surroundings[i])
                        //    {
                        //        surPosition = new Vector3Int(position.x + (int)Mathf.Pow(-1, i) * (3 - i) / 2, position.y + (int)Mathf.Pow(-1, i) * i / 2);
                        //        if (surPosition.y <= m_tilePositionMax.y && m_positions[surPosition.x - m_tilePositionMin.x, surPosition.y - m_tilePositionMin.y])
                        //        {
                        //            if (m_tileToChanger[GetTile(surPosition)].changeTiles[i + (int)Mathf.Pow(-1, i % 2)] != null)
                        //                tiles = m_tileToChanger[GetTile(surPosition)].changeTiles[i + (int)Mathf.Pow(-1, i % 2)].MatchesTiles(tiles);
                        //        }
                        //    }
                        //}
                        //if (tiles.Count == 0)
                        //{
                        //    tiles = GetTilesAnalog(changer).ToList();
                        //}
                    }

                    tile = tiles[Random.Range(0, tiles.Count)];
                    if (tile == null)
                    {
                        m_ground.SetTile(position, null);
                        m_walls.SetTile(position, null);
                        m_notCollidable.SetTile(position, null);
                        return;
                    }
                    TileChanger newChanger = m_tileToChanger[tile];
                    m_positions[position.x - m_tilePositionMin.x, position.y - m_tilePositionMin.y] = suitable = true;

                    for (int i = 0; i < newChanger.analogTiles.surroundings.Length; i++)
                    {
                        if (newChanger.analogTiles.surroundings[i])
                        {
                            surPosition = new Vector3Int(position.x + (int)Mathf.Pow(-1, i) * (3 - i) / 2, position.y + (int)Mathf.Pow(-1, i) * i / 2);
                            if (surPosition.y > m_tilePositionMax.y && newChanger.changeTiles[i] != null && GetTile(surPosition) == null)
                            {
                                m_notCollidable.SetTile(surPosition, newChanger.changeTiles[i].GetTiles()[Random.Range(0, newChanger.changeTiles[i].GetTiles().Count)]);
                            }
                            else if (surPosition.y > m_tilePositionMax.y && GetTile(surPosition) != null && newChanger.changeTiles[i] == null)
                            {
                                m_notCollidable.SetTile(surPosition, null);
                            }
                            else if (surPosition.y <= m_tilePositionMax.y && newChanger.changeTiles[i] != null &&
                                (newChanger.changeTiles[i].MatchesTiles(GetTilesAnalog(m_tileToChanger[GetTile(surPosition)])).Count == 0 ||
                                m_positions[surPosition.x - m_tilePositionMin.x, surPosition.y - m_tilePositionMin.y] && !newChanger.changeTiles[i].ContainsTile(GetTile(surPosition))))
                            {
                                suitable = false;
                                tiles.Remove(tile);
                                break;
                            }

                            if (newChanger.checkMoreSurroundings)
                            {

                                if (CheckSurroundings(newChanger, surPosition, i))
                                {
                                    suitable = false;
                                    tiles.Remove(tile);
                                    break;
                                }
                            }
                        }
                    }
                }

                SetTile(changer, tile, position);
            }
        }
    }

    bool CheckSurroundings(TileChanger changer, Vector3Int position, int index)
    {
        TileBase surSurPositionR = GetTile(new Vector3Int(position.x + (int)Mathf.Pow(-1, index) * index / 2, position.y + (int)Mathf.Pow(-1, index) * (3 - index) / 2));
        TileBase surSurPositionL = GetTile(new Vector3Int(position.x - (int)Mathf.Pow(-1, index) * index / 2, position.y - (int)Mathf.Pow(-1, index) * (3 - index) / 2));

        return changer.changeTiles[index] != null &&
                                     (surSurPositionR != null && changer.changeTiles[index].MatchesTiles(GetTilesAnalog(m_tileToChanger[GetTile(position)])).All(t =>
                                     {
                                         if (m_tileToChanger[t].analogTiles.surroundings[(3 - index) - (int)Mathf.Pow(-1, index % 2)] && m_tileToChanger[t].changeTiles[(3 - index) - (int)Mathf.Pow(-1, index % 2)] != null)
                                         {
                                             if (m_positions[position.x + (int)Mathf.Pow(-1, index) * index / 2 - m_tilePositionMin.x, position.y + (int)Mathf.Pow(-1, index) * (3 - index) / 2 - m_tilePositionMin.y])
                                                 return !m_tileToChanger[t].changeTiles[(3 - index) - (int)Mathf.Pow(-1, index % 2)].ContainsTile(surSurPositionR);
                                             else
                                                 return m_tileToChanger[t].changeTiles[(3 - index) - (int)Mathf.Pow(-1, index % 2)].MatchesTiles(GetTilesAnalog(m_tileToChanger[surSurPositionR])).Count == 0;
                                         }
                                         else return false;
                                     }) ||
                                     surSurPositionL != null && changer.changeTiles[index].MatchesTiles(GetTilesAnalog(m_tileToChanger[GetTile(position)])).All(t =>
                                     {
                                         if (m_tileToChanger[t].analogTiles.surroundings[3 - index] && m_tileToChanger[t].changeTiles[3 - index] != null)
                                         {
                                             if (m_positions[position.x - (int)Mathf.Pow(-1, index) * index / 2 - m_tilePositionMin.x, position.y - (int)Mathf.Pow(-1, index) * (3 - index) / 2 - m_tilePositionMin.y])
                                                 return !m_tileToChanger[t].changeTiles[3 - index].ContainsTile(surSurPositionL);
                                             else
                                                 return m_tileToChanger[t].changeTiles[3 - index].MatchesTiles(GetTilesAnalog(m_tileToChanger[surSurPositionL])).Count == 0;
                                         }
                                         else return false;
                                     }));
    }

    TileBase[] GetTilesAnalog(TileChanger changer)
    {
        return m_tilePaletteIndex switch
        {
            0 => changer.analogTiles.dark,
            1 => changer.analogTiles.bright,
            2 => changer.analogTiles.calm,
            _ => null,
        };
    }

    void SetTile(TileChanger changer, TileBase tile, Vector3Int position)
    {
        if (m_ground.CompareTag(changer.analogTiles.tilemapTag))
        {
            m_ground.SetTile(position, tile);
            m_walls.SetTile(position, null);
            m_notCollidable.SetTile(position, null);
        }
        else if (m_walls.CompareTag(changer.analogTiles.tilemapTag))
        {
            m_walls.SetTile(position, tile);
            m_ground.SetTile(position, null);
            m_notCollidable.SetTile(position, null);
        }
        else if (m_notCollidable.CompareTag(changer.analogTiles.tilemapTag))
        {
            m_notCollidable.SetTile(position, tile);
            m_walls.SetTile(position, null);
            m_ground.SetTile(position, null);
        }
    }

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
        return null;
    }

}
