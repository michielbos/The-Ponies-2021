using System;

namespace Model.Data {

[Serializable]
public class PonyActionData {
    public string identifier;
    public int tickCount;
    public bool canceled;
    public int targetObjectid;
    public string targetPonyUuid;
    public int targetTileX;
    public int targetTileY;
    
    public enum TargetType {
        Object, Pony, Tile
    }

    public PonyActionData(string identifier, int tickCount, bool canceled, int targetObjectid) {
        this.identifier = identifier;
        this.tickCount = tickCount;
        this.canceled = canceled;
        this.targetObjectid = targetObjectid;
        this.targetPonyUuid = "";
        this.targetTileX = -1;
        this.targetTileY = -1;
    }
    
    public PonyActionData(string identifier, int tickCount, bool canceled, string targetPonyUuid) {
        this.identifier = identifier;
        this.tickCount = tickCount;
        this.canceled = canceled;
        this.targetObjectid = -1;
        this.targetPonyUuid = targetPonyUuid;
        this.targetTileX = -1;
        this.targetTileY = -1;
    }
    
    public PonyActionData(string identifier, int tickCount, bool canceled, int targetTileX, int targetTileY) {
        this.identifier = identifier;
        this.tickCount = tickCount;
        this.canceled = canceled;
        this.targetObjectid = -1;
        this.targetPonyUuid = "";
        this.targetTileX = targetTileX;
        this.targetTileY = targetTileY;
    }

    public TargetType GetTargetType() {
        if (targetObjectid >= 0)
            return TargetType.Object;
        if (targetPonyUuid.Length > 0)
            return TargetType.Pony;
        return TargetType.Tile;
    }
}

}