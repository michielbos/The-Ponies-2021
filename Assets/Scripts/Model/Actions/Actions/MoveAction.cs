using Model.Ponies;
using Model.Property;
using UnityEngine;

namespace Model.Actions.Actions {

public class MoveAction : PonyAction {
    private readonly TerrainTile target;
    
    public MoveAction(Pony pony, string name, TerrainTile target) : base(pony, name) {
        this.target = target;
    }
    public override void Tick() {
        if (tickCount == 1) {
            if (!pony.SetWalkTarget(target.TilePosition)) {
                Finish();
            }
        }
        if (!pony.IsWalking) {
            Finish();
        }
    }

    protected override void OnCancel() {
        pony.CancelWalking();
    }
}

}