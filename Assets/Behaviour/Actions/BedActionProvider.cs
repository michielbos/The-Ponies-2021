using System.Collections.Generic;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using ThePoniesBehaviour.Extensions;
using UnityEngine;
using Util;

namespace ThePoniesBehaviour.Actions {

public class BedActionProvider : IObjectActionProvider {
    private const string SingleBedIdentifier = "singleBed";
    private const string DoubleBedIdentifier = "doubleBed";
    private const string SingleBedSleepIdentifier = "singleBedSleep";

    public IEnumerable<ObjectAction> GetActions(Pony pony, PropertyObject target) {
        List<ObjectAction> actions = new List<ObjectAction>(2);
        if (target.Type == SingleBedIdentifier) {
            actions.Add(new SingleBedSleepAction(SingleBedSleepIdentifier, pony, target, "Sleep"));
        }
        return actions;
    }

    public ObjectAction LoadAction(string identifier, Pony pony, PropertyObject target) {
        if (identifier == SingleBedSleepIdentifier)
            return new SingleBedSleepAction(identifier, pony, target, "Sleep");
        return null;
    }

    private class SingleBedSleepAction : ObjectAction {
        public override int AutonomyScore => 100 - Mathf.RoundToInt(pony.needs.Energy * 150);
        
        public SingleBedSleepAction(string identifier, Pony pony, PropertyObject target, string name) : base(identifier,
            pony, target, name) { }

        public override bool Tick() {
            Vector2Int headEndTile = target.TilePosition.GetNeighbourTile(target.Rotation.Inverse());
            if (pony.TilePosition == headEndTile)
                return HandleSleeping();
            
            ActionResult walkResult = WalkToBed(headEndTile);
            if (walkResult == ActionResult.Busy)
                return false;
            if (walkResult == ActionResult.Failed)
                return true;
            
            return false;
        }

        protected override void OnFinish() {
            target.users.Remove(pony);
        }

        /// <summary>
        /// Let the pony walk to the bed.
        /// Start using the bed after reaching it.
        /// </summary>
        /// <returns>The action result of the walk action.</returns>
        private ActionResult WalkToBed(Vector2Int headEndTile) {
            ObjectRotation left = target.Rotation.RotateCounterClockwise();
            ObjectRotation right = target.Rotation.RotateClockwise();
            // Move to the closest of the two tiles next to the head end of the bed.
            ActionResult result = this.WalkNextTo(headEndTile, new[] {left, right}, maxUsers: 1);
            if (result == ActionResult.Failed || result == ActionResult.Busy)
                return result;

            // Mark the bed as used.
            target.users.Add(pony);
            pony.TilePosition = headEndTile;

            return ActionResult.Success;
        }

        /// <summary>
        /// Handle the pony sleeping.
        /// </summary>
        /// <returns>True if the pony got out of bed.</returns>
        private bool HandleSleeping() {
            if (canceled)
                return GetOutOfBed();

            // Hourly energy gain is 5% per bed level.
            pony.needs.Energy += target.preset.needStats.energy / 20f / 60f;

            if (pony.needs.Energy >= 1f)
                return GetOutOfBed();

            return false;
        }

        /// <summary>
        /// Try to get out of bed.
        /// </summary>
        /// <returns>True if the pony got out of bed.</returns>
        private bool GetOutOfBed() {
            Vector2Int leftTile = pony.TilePosition.GetNeighbourTile(target.Rotation.RotateCounterClockwise());
            Vector2Int rightTile = pony.TilePosition.GetNeighbourTile(target.Rotation.RotateClockwise());
            // Try to get out on the left side first.
            if (PropertyController.Property.CanPassBorder(pony.TilePosition, leftTile)) {
                pony.TilePosition = leftTile;
                return true;
            }
            // If the left side is blocked, go out the right side.
            if (PropertyController.Property.CanPassBorder(pony.TilePosition, rightTile)) {
                pony.TilePosition = rightTile;
                return true;
            }
            // Failed to get out. You're stuck in bed. Forever.
            return false;
        }
    }
}

}