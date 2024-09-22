using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class GroupWithTiles : TileGroup
{
    public TileGroup[] tileGroups;

    public override List<TileBase> GetTiles()
    {
        List<TileBase> tiles = new List<TileBase>();
        foreach (var group in tileGroups)
        {
            tiles.AddRange(group.tiles);
        }
        tiles.AddRange(this.tiles);
        return tiles;
    }

    public override List<TileBase> MatchesTiles(IEnumerable<TileBase> tiles)
    {
        List<TileBase> matches = new List<TileBase>();
        foreach (TileGroup group in tileGroups)
        {
            matches.AddRange(group.MatchesTiles(tiles));
        }

        matches.AddRange(base.MatchesTiles(tiles));
        return matches;
    }

    public override bool ContainsTile(TileBase tile)
    {
        foreach (TileGroup group in tileGroups)
        {
            if (group.ContainsTile(tile))
                return true;
        }
        return base.ContainsTile(tile);
    }
}
