using Model.Data;
using Model.Ponies;
using Model.Property;
using UnityEngine;

namespace Model.Actions {

public abstract class TileAction : PonyAction {
    protected readonly TerrainTile target;

    protected TileAction(string identifier, Pony pony, TerrainTile target, string name) :
        base(identifier, pony, name) {
        this.target = target;
    }

    public override PonyActionData GetData() {
        Vector2Int tilePosition = target.TilePosition;
        return new PonyActionData(identifier, tickCount, canceled, tilePosition.x, tilePosition.y, data.GetDataPairs());
    }

    protected internal override bool TargetExists() {
        return true;
    }
}

}