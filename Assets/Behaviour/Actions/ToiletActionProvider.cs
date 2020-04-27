using System.Collections.Generic;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using ThePoniesBehaviour.Extensions;
using UnityEngine;
using Util;

namespace ThePoniesBehaviour.Actions {

/// <summary>
/// Provides actions for toilets.
/// </summary>
public class ToiletActionProvider : IObjectActionProvider {
    private const string UseIdentifier = "toiletUse";
    private const string FlushIdentifier = "toiletFlush";
    private const string ToiletType = "toilet";

    public IEnumerable<ObjectAction> GetActions(Pony pony, PropertyObject target) {
        if (target.Type == ToiletType) {
            List<ObjectAction> actions = new List<ObjectAction>(2);
            actions.Add(new UseAction(pony, target));
            
            if (target.data.GetBool("needsFlushing", false))
                actions.Add(new FlushAction(pony, target));
            
            return actions;
        }
        return new ObjectAction[0];
    }

    public ObjectAction LoadAction(string identifier, Pony pony, PropertyObject target) {
        if (identifier == UseIdentifier)
            return new UseAction(pony, target);
        if (identifier == FlushIdentifier)
            return new FlushAction(pony, target);
        return null;
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
            ActionResult walkResult = this.WalkNextTo(target.TilePosition, target.Rotation, maxUsers: 1);
            if (walkResult == ActionResult.Busy)
                return false;
            if (walkResult == ActionResult.Failed)
                return true;

            // Add this pony as user when it reached the toilet.
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
                return TryLeaveToilet(false);

            target.data.Put("needsFlushing", true);

            // Fully recharging bladder takes 30 minutes.
            pony.needs.Bladder += 1f / 30f;
            // Hourly comfort gain is 10% * comfort score.
            pony.needs.Comfort += 0.1f * target.preset.needStats.comfort / 60f;

            if (pony.needs.Bladder >= 1f)
                return TryLeaveToilet(true);

            return false;
        }

        /// <summary>
        /// Try to stand up from the toilet.
        /// </summary>
        /// <returns>Returns true if the pony stood up, false if there was no room to stand up.</returns>
        private bool TryLeaveToilet(bool flush) {
            Vector2Int cancelPosition = pony.TilePosition.GetNeighbourTile(target.Rotation);
            if (PropertyController.Property.CanPassBorder(pony.TilePosition, cancelPosition)) {
                target.users.Remove(pony);
                pony.TilePosition = cancelPosition;
                if (flush) {
                    pony.QueueActionFirst(new FlushAction(pony, target));
                }
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

    private class FlushAction : ObjectAction {
        public FlushAction(Pony pony, PropertyObject target) : base(FlushIdentifier, pony, target, "Flush") { }

        public override bool Tick() {
            ActionResult result = this.WalkNextTo(target.TilePosition, target.Rotation, maxUsers: 1);
            if (result == ActionResult.Busy)
                return false;
            if (result == ActionResult.Failed)
                return true;

            // TODO: Insert animation and sound.

            target.data.Put("needsFlushing", false);

            return true;
        }
    }
}

}