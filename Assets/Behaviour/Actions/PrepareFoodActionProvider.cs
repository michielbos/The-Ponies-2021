using System;
using System.Collections.Generic;
using Controllers.Singletons;
using JetBrains.Annotations;
using Model;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using ThePoniesBehaviour.Extensions;
using Util;

namespace ThePoniesBehaviour.Actions {

/// <summary>
/// Provides actions for raw food.
/// </summary>
public class PrepareFoodActionProvider : IObjectActionProvider {
    //board: 0000418e-663a-c38b-4deb-182df032981e
    //pancakes: 000001ee-ffd5-c794-1cda-cf42be44ae72
    private const string FridgePrepareIdentifier = "fridgePrepare";
    private const string PrepareIdentifier = "rawFoodPrepare";
    private const string PutAwayIdentifier = "rawFoodPutAway";
    private const string CleanUpIdentifier = "rawFoodCleanUp";
    private const string FridgeType = "fridge";
    private const string StoveType = "stove";
    
    private const string BoardUuid = "0000418e-663a-c38b-4deb-182df032981e";
    private const string PancakesUuid = "000001ee-ffd5-c794-1cda-cf42be44ae72";
    
    private const string DataDestination = "destination";
    private const string DataAttempt = "attempt";

    private const int MaxAttempts = 3;
    
    public IEnumerable<ObjectAction> GetActions(Pony pony, PropertyObject target) {
        if (target.Type == FridgeType) {
            return new [] {new FridgePrepareAction(pony, target)};
        }
        if (target.preset.guid == new Guid(BoardUuid)) {
            List<ObjectAction> actions = new List<ObjectAction>(2);
            actions.Add(new PrepareAction(pony, target));
            actions.Add(new CleanUpAction(CleanUpIdentifier, pony, target));
            return actions;
        }
        return new ObjectAction[0];
    }

    public ObjectAction LoadAction(string identifier, Pony pony, PropertyObject target) {
        if (identifier == FridgePrepareIdentifier)
            return new PrepareAction(pony, target);
        if (identifier == PrepareIdentifier)
            return new PrepareAction(pony, target);
        if (identifier == PutAwayIdentifier)
            return new PutAwayAction(PutAwayIdentifier, pony, target);
        if (identifier == CleanUpIdentifier)
            return new CleanUpAction(CleanUpIdentifier, pony, target);
        return null;
    }

    private class FridgePrepareAction : ObjectAction {

        public FridgePrepareAction(Pony pony, PropertyObject target) : base(PrepareIdentifier, pony, target, "Prepare pancakes") { }

        public override bool Tick() {
            ActionResult walkResult = this.WalkNextTo(target.TilePosition, target.Rotation, maxUsers: 1);
            if (walkResult == ActionResult.Busy)
                return false;
            if (walkResult == ActionResult.Failed)
                return true;

            SoundController.Instance.PlaySound(SoundType.Buy);
            MoneyController.Instance.ChangeFunds(-10);
            FurniturePreset preset = FurniturePresets.Instance.GetFurniturePreset(new Guid(BoardUuid));
            PropertyObject food = PropertyController.Property.PlacePropertyObject(pony.HoofSlot, target.Rotation, preset);
            pony.QueueActionFirst(new PrepareAction(pony, food));

            return true;
        }
    }

    private class PrepareAction : ObjectAction {
        
        public PrepareAction(Pony pony, PropertyObject target) : base(PrepareIdentifier, pony, target, "Prepare") { }

        public override bool Tick() {
            if (pony.HoofSlot.SlotObject != target) {
                ActionResult walkResult = this.WalkNextTo(target.TilePosition, maxUsers: 1);
                if (walkResult == ActionResult.Busy)
                    return false;
                if (walkResult == ActionResult.Failed)
                    return true;
                pony.PickUp(target);
                return false;
            }

            if (canceled) {
                pony.DropHoofItem();
                return true;
            }

            PropertyObject destination = data.GetPropertyObjectOrNull(DataDestination);
            int attempt = data.GetInt(DataAttempt, 0);

            // Get a target object, if we have none.
            if (destination == null) {
                destination = FindStove();
                if (destination == null || attempt >= MaxAttempts) {
                    pony.DropHoofItem();
                    return true;
                }
                data.Put(DataDestination, destination);
                data.Put(DataAttempt, ++attempt);
            }

            ActionResult result = this.WalkNextTo(destination.TilePosition, destination.Rotation);
            if (result == ActionResult.Busy)
                return false;
            if (result == ActionResult.Failed) {
                // Try again next tick.
                data.Put(DataDestination, null);
                return false;
            }

            PrepareFood();
            return true;
        }

        [CanBeNull]
        private PropertyObject FindStove() {
            Property property = PropertyController.Property;
            return pony.GetClosestObjectWhere(
                obj => obj.Type == StoveType && !property.WallExists(obj.TilePosition, obj.Rotation),
                obj => new[] {obj.TilePosition.GetNeighbourTile(obj.Rotation)});
        }

        /// <summary>
        /// Handle preparing of the food.
        /// </summary>
        private void PrepareFood() {
            FurniturePreset pancakePreset = FurniturePresets.Instance.GetFurniturePreset(new Guid(PancakesUuid));
            PropertyObject pancakes = PropertyController.Property.PlacePropertyObject(pony.HoofSlot, target.Rotation, pancakePreset);
            PropertyController.Property.RemovePropertyObject(target);
            pony.QueueActionFirst(new FoodActionProvider.EatAction(pony, pancakes));
        }

        protected override void OnFinish() {
            if (target != null) {
                target.users.Remove(pony);
            }
        }
    }
}

}