using System.Collections.Generic;
using Model.Actions;
using UnityEngine;

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
    /// Walk to a target.
    /// </summary>
    /// <param name="action">The action that is running this method.</param>
    /// <param name="target">The target to walk to.</param>
    /// <param name="stopOnCancel">If true, moving will fail if the action is canceled.</param>
    /// <param name="maxUsers">If 0 or more, moving will fail if there are more than this amount of users using
    /// this object.</param>
    /// <returns></returns>
    public static ActionResult WalkToClosest(this ObjectAction action, ICollection<Vector2Int> target,
        bool stopOnCancel = true, int maxUsers = -1) {
        if (maxUsers >= 0 && action.target.users.Count >= maxUsers)
            return ActionResult.Failed;

        return ((PonyAction) action).WalkToClosest(target, stopOnCancel);
    }
}

}