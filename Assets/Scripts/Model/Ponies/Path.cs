using System.Linq;
using UnityEngine;

namespace Model.Ponies {

public class Path {
    private readonly Vector2Int[] tiles;
    private int progress = 0;
    
    public Vector2Int Destination => tiles[tiles.Length - 1];

    public Path(Vector2Int[] tiles) {
        this.tiles = tiles;
    }

    public bool HasNext() {
        return progress < tiles.Length;
    }

    public Vector2Int NextTile() {
        Vector2Int tile = tiles[progress];
        progress++;
        return tile;
    } 

    public override string ToString() {
        return tiles.Aggregate("", (current, t) => current + t.ToString());
    }
}

}