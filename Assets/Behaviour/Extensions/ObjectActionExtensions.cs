using System.Collections.Generic;
using System.Linq;
using Model.Actions;
using Model.Property;
using UnityEngine;
using Util;

namespace ThePoniesBehaviour.Extensions {

/// <summary>
/// Extension methods for object action behaviour.
/// Contains behaviour that is often repeated in actions.
/// </summary>
public static class ObjectActionExtensions {
    /// <summary>
    /// Walk to a target.
    /// </summary>
    /// <param name="action">The action that is running this method.</param>
    /// <param name="target">The target to walk to.</param>
    /// <param name="stopOnCancel">If true, moving will fail if the action is canceled.</param>
    /// <param name="maxUsers">If 0 or more, moving will fail if there are more than this amount of users using
    /// this object.</param>
    /// <returns></returns>
    public static ActionResult WalkTo(this ObjectAction action, Vector2Int target, bool stopOnCancel = true,
        int maxUsers = -1) {
        return action.WalkToClosest(new[] {target}, stopOnCancel, maxUsers);
    }

    /// <summary>
    /// Walk to the tile next of an object, in the given direction of the object.
    /// If there is a wall between the two tiles, the walk action will fail.
    /// </summary>
    public static ActionResult WalkNextTo(this ObjectAction action, Vector2Int target, ObjectRotation direction,
        bool stopOnCancel = true, int maxUsers = -1) {
        return action.WalkNextTo(target, new[] {direction}, stopOnCancel, maxUsers);
    }
    
    /// <summary>
    /// Walk to the closest tile next of an object, that is not blocked by a wall.
    /// </summary>
    public static ActionResult WalkNextTo(this ObjectAction action, Vector2Int target,
        bool stopOnCancel = true, int maxUsers = -1) {
        return action.WalkNextTo(target, new[] {ObjectRotation.SouthEast, ObjectRotation.SouthWest, 
            ObjectRotation.NorthWest, ObjectRotation.NorthEast}, stopOnCancel, maxUsers);
    }
    
    /// <summary>
    /// Walk to the tile next of an object, in the closest of the given direction of the object.
    /// Directions with walls between them are ignored.
    /// </summary>
    public static ActionResult WalkNextTo(this ObjectAction action, Vector2Int target,
        ICollection<ObjectRotation> directions, bool stopOnCancel = true, int maxUsers = -1) {
        Vector2Int[] targetTiles = directions.Select(direction => target.GetNeighbourTile(direction))
            .Where(tile => !PropertyController.Property.WallExists(target.GetBorderBetweenTiles(tile)))
            .ToArray();

        if (targetTiles.Length == 0) {
            action.pony.ClearWalkTarget();
            return ActionResult.Failed;
        }

        return action.WalkToClosest(targetTiles, stopOnCancel, maxUsers);
    }
    
    /*/// <summary>
    /// Walk to a tile within range of an object.
    /// A range of 1 means the pony has to stand directly next to the object.
    /// Standing diagonally next to the object requires a range of 2.
    /// </summary>
    public static ActionResult WalkToRange(this ObjectAction action, Vector2Int target,
        int range, bool stopOnCancel = true, int maxUsers = -1) {
        Vector2Int[] targetTiles = directions.Select(direction => target.GetNeighbourTile(direction))
            .Where(tile => !PropertyController.Property.WallExists(target.GetBorderBetweenTiles(tile)))
            .ToArray();

        if (targetTiles.Length == 0) {
            action.pony.ClearWalkTarget();
            return ActionResult.Failed;
        }

        return action.WalkToClosest(targetTiles, stopOnCancel, maxUsers);
    }*/

    /// <summary>
    /// Walks to the closest of a list of targets.
    /// </summary>
    /// <param name="action">The action that is running this method.</param>
    /// <param name="targets">The targets to pick from.</param>
    /// <param name="stopOnCancel">If true, moving will fail if the action is canceled.</param>
    /// <param name="maxUsers">If 0 or more, moving will fail if there are more than this amount of users using
    /// this object.</param>
    /// <returns></returns>
    public static ActionResult WalkToClosest(this ObjectAction action, ICollection<Vector2Int> targets,
        bool stopOnCancel = true, int maxUsers = -1) {
        if (maxUsers >= 0 && action.target.users.Count >= maxUsers) {
            action.pony.ClearWalkTarget();
            return ActionResult.Failed;
        }

        return ((PonyAction) action).WalkToClosest(targets, stopOnCancel);
    }
}

}