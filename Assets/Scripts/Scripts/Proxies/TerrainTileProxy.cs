using Model.Property;
using MoonSharp.Interpreter;

namespace Scripts.Proxies {

public class TerrainTileProxy {
    private readonly TerrainTile tile;

    [MoonSharpHidden]
    public TerrainTileProxy(TerrainTile tile) {
        this.tile = tile;
    }

    public Vector2Wrapper position {
        get => new Vector2Wrapper(tile.TilePosition);
        set => tile.TilePosition = value.GetVector2Int();
    }
}

}