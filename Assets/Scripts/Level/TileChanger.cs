using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileChanger : ScriptableObject
{
    public TileBase[] tiles;
    public TilePlaceAnalog analogTiles;
    public bool addGrass;
    public bool makeGround;
    public TileGroup[] changeTiles;
}
