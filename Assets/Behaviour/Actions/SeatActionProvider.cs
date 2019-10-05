using System.Collections.Generic;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using UnityEngine;
using Util;

namespace ThePoniesBehaviour.Actions {

public class SeatActionProvider : IObjectActionProvider {
    private const string SitIdentifier = "seatSit";
    
    public IEnumerable<ObjectAction> GetActions(Pony pony, PropertyObject target) {
        if (target.Type == "chair" || target.Type == "couch") {
            return new[] {new SitAction(pony, target)};
        }
        return new ObjectAction[0];
    }

    public ObjectAction LoadAction(string identifier, Pony pony, PropertyObject target) {
        return identifier == SitIdentifier ? new SitAction(pony, target) : null;
    }
    
    private class SitAction : ObjectAction {
        public SitAction(Pony pony, PropertyObject target) : base(SitIdentifier, pony, target, "Sit") { }

        public override bool Tick() {
            // Walk to couch
            // Sit down
            // Sit until canceled.
            if (!pony.WalkTo(target.TilePosition.GetNeighbourTile(target.Rotation))) {
                return pony.WalkingFailed;
            }
            Debug.Log("Sit action...");
            return tickCount >= 3;
        }
    }
}

}