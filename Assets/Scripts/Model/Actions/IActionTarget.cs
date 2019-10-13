using System.Collections.Generic;
using Model.Ponies;

namespace Model.Actions {

public interface IActionTarget {
    ICollection<PonyAction> GetActions(Pony pony);
}

}