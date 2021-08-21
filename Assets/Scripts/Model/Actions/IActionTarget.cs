using System.Collections.Generic;
using Model.Ponies;

namespace Model.Actions {

public interface IActionTarget {
    /// <summary>
    /// Get all actions that the given pony can do on this target.
    /// If showInvisible is true, invisible actions that are normally not invokable by the player are also included.
    /// </summary>
    ICollection<PonyAction> GetActions(Pony pony, bool showInvisible);
}

}