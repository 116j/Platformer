using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Collider2D))]
public class DestroyableTile : MonoBehaviour
{
    public static DestroyableTile Instance { get; private set; }

    [SerializeField]
    GameObject m_tileObject;
    [SerializeField]
    List<TileBase> m_tiles;

    Tilemap m_tilemap;
    TileBase m_tile;

    Vector3Int m_tilePos = Vector3Int.zero;

    List<Vector3Int> m_destroyedTiles;

    float m_timer = 0f;
    float m_destroyTime = 10f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        m_tilemap = GetComponent<Tilemap>();

        m_destroyedTiles = new List<Vector3Int>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_tilePos = m_tilemap.WorldToCell(collision.contacts[0].point) - Vector3Int.up;
        m_timer = 0f;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        m_timer += Time.deltaTime;
        Vector3Int tilePos = m_tilemap.WorldToCell(collision.contacts[0].point) - Vector3Int.up;
        //if player moves from tile - destroy it 
        if (tilePos != m_tilePos && m_tilemap.GetTile(m_tilePos) != null)
        {
            m_tile = m_tilemap.GetTile(m_tilePos);
            m_destroyedTiles.Add(m_tilePos);
            m_tilemap.SetTile(m_tilePos, null);
            Trap destroyable = Instantiate(m_tileObject, m_tilemap.GetCellCenterWorld(m_tilePos), Quaternion.identity).GetComponent<Trap>();
            destroyable.SetTrap(m_tiles.IndexOf(m_tile));
            m_tilePos = tilePos;
            m_timer = 0f;
        }
        else if(tilePos == m_tilePos && m_tilemap.GetTile(m_tilePos) != null && m_timer >= m_destroyTime)
        {
            m_tile = m_tilemap.GetTile(m_tilePos);
            m_tilemap.SetTile(m_tilePos, null);
            m_destroyedTiles.Add(m_tilePos);
            Trap destroyable = Instantiate(m_tileObject, m_tilemap.GetCellCenterWorld(m_tilePos), Quaternion.identity).GetComponent<Trap>();
            Debug.Log(m_tiles.IndexOf(m_tile));
            destroyable.SetTrap(m_tiles.IndexOf(m_tile));
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        m_timer = 0f;
        // destroy tile where the player was standing
        if (m_tilePos != null && m_tilemap.GetTile(m_tilePos) != null)
        {
            m_tile = m_tilemap.GetTile(m_tilePos);
            m_tilemap.SetTile(m_tilePos, null);
            m_destroyedTiles.Add(m_tilePos);
            Trap destroyable = Instantiate(m_tileObject, m_tilemap.GetCellCenterWorld(m_tilePos), Quaternion.identity).GetComponent<Trap>();
            destroyable.SetTrap(m_tiles.IndexOf(m_tile));
        }
    }
    /// <summary>
    /// Reset tiles if player fell or died
    /// </summary>
    /// <param name="position"></param>
    public void Restart(Vector3 position)
    {
        if (m_tilePos.x >= position.x || m_tilePos.y >= position.y)
        {
            foreach (var tilePos in m_destroyedTiles)
            {
                m_tilemap.SetTile(tilePos, m_tile);
            }
            m_destroyedTiles.Clear();
        }
    }
}
