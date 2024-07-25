using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TilePlaceAnalog : Group
{
    public TileBase[] dark;
    public TileBase[] bright;
    public TileBase[] calm;

    public string tilemapTag;

    public bool[] surroundings;

    public TileGroup GetTileGroup(int index)
    {
        return index switch
        {
            0 => ScriptableObject.CreateInstance<TileGroup>(),
            1 => ScriptableObject.CreateInstance<TileGroup>(),
            2 => ScriptableObject.CreateInstance<TileGroup>(),
            _ => null,
        };
    }

    public override List<TileBase> GetTiles()
    {
        List<TileBase > tiles = new List<TileBase>(dark);
        tiles.AddRange(bright);
        tiles.AddRange(calm);
        return tiles;
    }

}
