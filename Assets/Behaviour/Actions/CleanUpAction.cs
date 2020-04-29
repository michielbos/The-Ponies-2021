using JetBrains.Annotations;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using ThePoniesBehaviour.Extensions;
using Util;

namespace ThePoniesBehaviour.Actions {

/// <summary>
/// Reusable action for picking up an item and throwing it in the trash can, sink, or dishwasher.
/// (but currently everything is flushed down the toilet)
/// </summary>
public class CleanUpAction : ObjectAction {
    private const string DataDestination = "destination";
    private const string DataAttempt = "attempt";

    private const int MaxAttempts = 3;

    public CleanUpAction(string identifier, Pony pony, PropertyObject target, string name = "Clean up") :
        base(identifier, pony, target, name) { }

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
            destination = FindToilet();
            if (destination == null || attempt >= MaxAttempts) {
                pony.DropHoofItem();
                return true;
            }
            data.Put(DataDestination,destination);
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

        PropertyController.Property.RemovePropertyObject(target);
        return true;
    }

    [CanBeNull]
    private PropertyObject FindToilet() {
        Property property = PropertyController.Property;
        return pony.GetClosestObjectWhere(
            obj => obj.Type == "toilet" && !property.WallExists(obj.TilePosition, obj.Rotation),
            obj => new[] {obj.TilePosition.GetNeighbourTile(obj.Rotation)});
    }
}

}