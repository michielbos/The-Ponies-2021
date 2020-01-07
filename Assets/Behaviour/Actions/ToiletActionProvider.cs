using System.Collections.Generic;
using System.Linq;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using UnityEngine;
using Util;

namespace ThePoniesBehaviour.Actions {

/// <summary>
/// Provides actions for toilets.
/// </summary>
public class ToiletActionProvider : IObjectActionProvider {
    private const string UseIdentifier = "toiletUse";
    private const string ToiletType = "toilet";

    public IEnumerable<ObjectAction> GetActions(Pony pony, PropertyObject target) {
        if (target.Type == ToiletType) {
            return new[] {new UseAction(pony, target)};
        }
        return new ObjectAction[0];
    }

    public ObjectAction LoadAction(string identifier, Pony pony, PropertyObject target) {
        return identifier == UseIdentifier ? new UseAction(pony, target) : null;
    }

    private class UseAction : ObjectAction {
        public UseAction(Pony pony, PropertyObject target) : base(UseIdentifier, pony, target, "Use") { }

        public override bool Tick() {
            if (target.users.Contains(pony))
                return HandleUsing();
            return HandleMoveToToilet();
        }

        /// <summary>
        /// Move to the toilet.
        /// </summary>
        /// <returns>True if the action failed.</returns>
        private bool HandleMoveToToilet() {
            if (canceled)
                return true;

            // Nope out if someone else is using this.
            if (target.users.Count > 0)
                return true;

            // Walk to the toilet.
            if (!pony.WalkTo(target.TilePosition.GetNeighbourTile(target.Rotation))) {
                return pony.WalkingFailed;
            }
            
            target.users.Add(pony);
            pony.TilePosition = target.TilePosition;

            return false;
        }

        /// <summary>
        /// Handle the actual usage of the toilet.
        /// </summary>
        /// <returns>True when the action was finished.</returns>
        private bool HandleUsing() {
            if (canceled)
                return TryLeaveToilet();
            
            // Fully recharging bladder takes 30 minutes.
            pony.needs.Bladder += 1f / 30f;
            // Hourly comfort gain is 10% * comfort score.
            pony.needs.Comfort += 0.1f * target.preset.needStats.comfort / 60f;

            if (pony.needs.Bladder >= 1f)
                return TryLeaveToilet();
            
            return false;
        }

        /// <summary>
        /// Try to stand up from the toilet.
        /// </summary>
        /// <returns>Returns true if the pony stood up, false if there was no room to stand up.</returns>
        private bool TryLeaveToilet() {
            Vector2Int cancelPosition = pony.TilePosition.GetNeighbourTile(target.Rotation);
            if (PropertyController.Property.CanPassBorder(pony.TilePosition, cancelPosition)) {
                target.users.Remove(pony);
                pony.TilePosition = cancelPosition;
                return true;
            }
            return false;
        }

        protected override void OnFinish() {
            if (target != null) {
                target.users.Remove(pony);
            }
        }
    }
}

}