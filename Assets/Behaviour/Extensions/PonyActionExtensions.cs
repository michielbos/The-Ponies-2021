using System.Collections.Generic;
using Model.Actions;
using Model.Ponies;
using UnityEngine;

namespace ThePoniesBehaviour.Extensions {

/// <summary>
/// Extensions methods for action behaviour.
/// Contains behaviour that is often repeated in actions.
/// </summary>
public static class PonyActionExtensions {

    /// <summary>
    /// Walk to a target.
    /// </summary>
    /// <param name="action">The action that is running this method.</param>
    /// <param name="target">The target to walk to.</param>
    /// <param name="stopOnCancel">If true, moving will fail if the action is canceled.</param>
    /// <returns></returns>
    public static ActionResult WalkTo(this PonyAction action, Vector2Int target, bool stopOnCancel = true) {
        return WalkToClosest(action, new[] {target}, stopOnCancel);
    }

    /// <summary>
    /// Walk to the closest target.
    /// </summary>
    /// <param name="action">The action that is running this method.</param>
    /// <param name="targets">The list of targets to choose from.</param>
    /// <param name="stopOnCancel">If true, moving will fail if the action is canceled.</param>
    /// <returns></returns>
    public static ActionResult WalkToClosest(this PonyAction action, ICollection<Vector2Int> targets, bool stopOnCancel = true) {
        Pony pony = action.pony;

        if (stopOnCancel && action.canceled) {
            pony.ClearWalkTarget();
            return ActionResult.Failed;
        }

        if (pony.WalkToClosest(targets)) {
            return ActionResult.Success;
        }
        
        return pony.WalkingFailed ? ActionResult.Failed : ActionResult.Busy;
    }
}

}