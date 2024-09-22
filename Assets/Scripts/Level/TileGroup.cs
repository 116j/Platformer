using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileGroup : Group
{
    public List<TileBase> tiles;

    public override List<TileBase> GetTiles()
    {
        return this.tiles;
    }

    public virtual bool ContainsTile(TileBase tile)
    {
        return tiles.Contains(tile);
    }

    public virtual List<TileBase> MatchesTiles(IEnumerable<TileBase> tiles)
    {
        List<TileBase> matches = new List<TileBase>();
        foreach (TileBase tile in this.tiles)
        {
            if (tiles.Contains(tile))
            {
                matches.Add(tile);
            }
        }
        return matches;
    }

}
