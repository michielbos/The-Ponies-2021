using System.Collections.Generic;
using System.Linq;
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
            if (target.users.FirstOrDefault() == pony) {
                return HandleSitting();
            }

            // Quit if canceled or if another pony is using this.
            if (canceled || target.users.Count > 0)
                return true;
            // Walk to the seat.
            if (!pony.WalkTo(target.TilePosition.GetNeighbourTile(target.Rotation))) {
                return pony.WalkingFailed;
            }
            target.users.Add(pony);
            pony.TilePosition = target.TilePosition;

            return false;
        }

        private bool HandleSitting() {
            if (canceled) {
                target.users.Remove(pony);
                pony.TilePosition = target.TilePosition.GetNeighbourTile(target.Rotation);
                return true;
            }
            // Hourly comfort gain is 10% * comfort score.
            pony.needs.Comfort += 0.1f * target.preset.needStats.comfort / 60f;
            return false;
        }

        protected override void OnFinish() {
            target.users.Remove(pony);
        }
    }
}

}