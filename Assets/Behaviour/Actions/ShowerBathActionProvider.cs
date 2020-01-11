using System.Collections.Generic;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using ThePoniesBehaviour.Extensions;
using UnityEngine;
using Util;

namespace ThePoniesBehaviour.Actions {

/// <summary>
/// Provider for shower and bath actions.
/// </summary>
public class ShowerBathActionProvider : IObjectActionProvider {
    private const string BathtubIdentifier = "bathtub";
    private const string ShowerTubIdentifier = "showertub";
    private const string ShowerIdentifier = "shower";

    private const string BathUseAction = "bathUse";
    private const string ShowerAction = "shower";

    public IEnumerable<ObjectAction> GetActions(Pony pony, PropertyObject target) {
        List<ObjectAction> actions = new List<ObjectAction>(2);
        if (target.Type == BathtubIdentifier || target.Type == ShowerTubIdentifier) {
            actions.Add(new TakeBathAction(BathUseAction, pony, target, "Take bath"));
        }
        if (target.Type == ShowerIdentifier || target.Type == ShowerTubIdentifier) {
            actions.Add(new TakeShowerAction(ShowerAction, pony, target, "Shower"));
        }
        return actions;
    }

    public ObjectAction LoadAction(string identifier, Pony pony, PropertyObject target) {
        if (identifier == BathUseAction) 
            return new TakeBathAction(identifier, pony, target, "Take bath");
        if (identifier == ShowerAction)
            return new TakeShowerAction(identifier, pony, target, "Shower");
        return null;
    }

    /// <summary>
    /// Take bath action.
    /// </summary>
    private class TakeBathAction : BaseShowerBathAction {
        public TakeBathAction(string identifier, Pony pony, PropertyObject target, string name) : base(
            identifier, pony, target, name, true) { }
    }

    /// <summary>
    /// Shower action.
    /// </summary>
    private class TakeShowerAction : BaseShowerBathAction {
        public TakeShowerAction(string identifier, Pony pony, PropertyObject target, string name) : base(
            identifier, pony, target, name, false) { }
    }

    /// <summary>
    /// Shared behaviour for showers and bathtubs.
    /// </summary>
    private abstract class BaseShowerBathAction : ObjectAction {
        private readonly bool isBath;
        
        protected BaseShowerBathAction(string identifier, Pony pony, PropertyObject target, string name, bool isBath) :
            base(identifier, pony,
                target, name) {
            this.isBath = isBath;
        }

        public override bool Tick() {
            if (!WalkToBath())
                return false;
            if (pony.TilePosition == target.TilePosition) {
                bool temp = HandleBathing();
                return temp;
            }
            return true;
        }

        protected override void OnFinish() {
            // Un-use the bath, so another pony can use it.
            target.users.Remove(pony);
        }

        private bool WalkToBath() {
            // Walk to the tile in front of the bathtub.
            ActionResult result = this.WalkTo(target.TilePosition.GetNeighbourTile(target.Rotation), maxUsers: 1);
            if (result == ActionResult.Busy)
                return false;
            if (result == ActionResult.Failed)
                return true;

            target.users.Add(pony);
            // Finally put that pony in its bath. :D
            pony.TilePosition = target.TilePosition;

            return true;
        }

        /// <summary>
        /// Behaviour while pony is in bath.
        /// </summary>
        /// <returns>True if the pony got out of the bath.</returns>
        private bool HandleBathing() {
            if (canceled) {
                return GetOutOfBath();
            }

            // Showers are more effective for hygiene than bathtubs.
            float hygieneMultiplier = isBath ? 0.8f : 1f;

            // Pony should be clean in 1 hour on a level 10 bathtub.
            pony.needs.Hygiene += hygieneMultiplier * target.preset.needStats.hygiene / 10f / 60f;
            if (isBath) {
                // Hourly comfort gain is 10% * comfort score (same as chairs).
                pony.needs.Comfort += target.preset.needStats.comfort / 10f / 60f;
            }

            // Get out of the bath when clean.
            if (pony.needs.Hygiene >= 1f) {
                return GetOutOfBath();
            }

            return false;
        }

        /// <summary>
        /// Leave the bath if there is room.
        /// If there is no room, wait....
        /// </summary>
        /// <returns>True if the pony got out of bath.</returns>
        private bool GetOutOfBath() {
            Vector2Int targetTile = target.TilePosition.GetNeighbourTile(target.Rotation);
            if (PropertyController.Property.CanPassBorder(pony.TilePosition, targetTile)) {
                pony.TilePosition = targetTile;
                return true;
            }
            return false;
        }
    }
}

}