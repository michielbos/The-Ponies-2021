using System.Collections.Generic;
using Model.Actions;
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
        if (maxUsers >= 0 && action.target.users.Count >= maxUsers)
            return ActionResult.Failed;

        return ((PonyAction) action).WalkToClosest(targets, stopOnCancel);
    }
    
    /// <summary>
    /// Walks to the closest tile next to a given target.
    /// </summary>
    /// <param name="action">The action that is running this method.</param>
    /// <param name="target">The target to pick a neighbour from.</param>
    /// <param name="stopOnCancel">If true, moving will fail if the action is canceled.</param>
    /// <param name="maxUsers">If 0 or more, moving will fail if there are more than this amount of users using
    /// this object.</param>
    /// <returns></returns>
    public static ActionResult WalkToClosestNeighbour(this ObjectAction action, Vector2Int target,
        bool stopOnCancel = true, int maxUsers = -1) {
        return action.WalkToClosest(target.GetNeighbourTiles(), stopOnCancel, maxUsers);
    }
}

}