using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TilePlaceAnalog : ScriptableObject
{
    public TileBase[] dark;
    public TileBase[] calm;

    public string tilemapTag;

    public bool[] surroundings;

    public TilePlaceAnalog placeAnalog;
}
