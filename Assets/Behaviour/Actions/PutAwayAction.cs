using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using ThePoniesBehaviour.Extensions;
using UnityEngine;
using Util;

namespace ThePoniesBehaviour.Actions {

/// <summary>
/// Reusable action for putting a carried item on a surface.
/// </summary>
public class PutAwayAction : ObjectAction {
    private const string DataTargetTile = "targetTile";
    private const string DataAttempt = "attempt";
    
    private const int MaxAttempts = 3;

    public PutAwayAction(string identifier, Pony pony, PropertyObject target, string name = "Put away") :
        base(identifier, pony, target, name) { }

    public override bool Tick() {
        if (pony.HoofSlot.SlotObject == null)
            return true;

        if (canceled) {
            pony.DropHoofItem();
            return true;
        }

        Vector2Int? targetTile = data.GetVector2IntOrNull(DataTargetTile);
        int attempt = data.GetInt(DataAttempt, 0);
        
        // Get a target slot, if we have none.
        if (!targetTile.HasValue) {
            targetTile = FindEmptySurface();
            if (!targetTile.HasValue || attempt >= MaxAttempts) {
                pony.DropHoofItem();
                return true;
            }
            data.Put(DataTargetTile, targetTile.Value);
            data.Put(DataAttempt, ++attempt);
        }

        ActionResult result = this.WalkTo(targetTile.Value);
        if (result == ActionResult.Busy)
            return false;
        if (result == ActionResult.Failed) {
            // Try again next tick.
            data.Put(DataTargetTile, null);
            return false;
        }

        // Find any free surface from the destination tile and put the item there.
        // This is definitely not optimal for performance, but will save us a LOT of state inconsistency bugs.
        ICollection<Vector2Int> possibleSlotTiles = pony.TilePosition.GetNeighbourTiles();
        Property property = PropertyController.Property;
        SurfaceSlot surfaceSlot = property.propertyObjects.Values
            .SelectMany(obj => obj.SurfaceSlots)
            .FirstOrDefault(slot => slot.SlotObject == null && 
                           possibleSlotTiles.Contains(slot.TilePosition) &&
                           !property.WallExists(pony.TilePosition.GetBorderBetweenTiles(slot.TilePosition)));
        if (surfaceSlot != null) {
            surfaceSlot.PlaceObject(pony.HoofSlot.SlotObject);
            return true;
        } else {
            // Try again next tick.
            data.Put(DataTargetTile, null);
            return false;
        }
    }

    [CanBeNull]
    private Vector2Int? FindEmptySurface() {
        Property property = PropertyController.Property;

        // Selector to get all tiles that are next to an empty surface slot, without being intersected with a wall.
        Func<PropertyObject, IEnumerable<Vector2Int>> tileSelector = obj => obj.SurfaceSlots
            .Where(slot => slot.SlotObject == null)
            .SelectMany(slot => slot.TilePosition.GetNeighbourTiles()
                .Where(tile => !property.WallExists(slot.TilePosition.GetBorderBetweenTiles(tile))));
        
        PropertyObject targetObject = pony.GetClosestObjectWhere(obj => obj.preset.IsSurface, 
            obj => tileSelector(obj).ToArray());

        if (targetObject == null)
            return null;

        // Double pathfinding. Sorry.
        return Pathfinding.PathToNearest(pony.TilePosition, tileSelector(targetObject))?.Destination;
    }
}

}