using System;

namespace Model.Data {
[Serializable]
public class PonyActionData {
    public string identifier;
    public int tickCount;
    public bool canceled;
    public int trigger;
    public int targetObjectid;
    public string targetPonyUuid;
    public int targetTileX;
    public int targetTileY;
    public DataPair[] data;

    public enum TargetType {
        Object,
        Pony,
        Tile
    }

    public PonyActionData(string identifier, int tickCount, bool canceled, int trigger, int targetObjectid,
        DataPair[] data) {
        this.identifier = identifier;
        this.tickCount = tickCount;
        this.canceled = canceled;
        this.trigger = trigger;
        this.targetObjectid = targetObjectid;
        this.targetPonyUuid = "";
        this.targetTileX = -1;
        this.targetTileY = -1;
        this.data = data;
    }

    public PonyActionData(string identifier, int tickCount, bool canceled, int trigger, string targetPonyUuid,
        DataPair[] data) {
        this.identifier = identifier;
        this.tickCount = tickCount;
        this.canceled = canceled;
        this.trigger = trigger;
        this.targetObjectid = -1;
        this.targetPonyUuid = targetPonyUuid;
        this.targetTileX = -1;
        this.targetTileY = -1;
        this.data = data;
    }

    public PonyActionData(string identifier, int tickCount, bool canceled, int trigger, int targetTileX,
        int targetTileY, DataPair[] data) {
        this.identifier = identifier;
        this.tickCount = tickCount;
        this.canceled = canceled;
        this.trigger = trigger;
        this.targetObjectid = -1;
        this.targetPonyUuid = "";
        this.targetTileX = targetTileX;
        this.targetTileY = targetTileY;
        this.data = data;
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