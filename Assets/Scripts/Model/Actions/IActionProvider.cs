using System.Collections.Generic;
using Model.Ponies;

namespace Model.Actions {

public interface IActionProvider {
    List<PonyAction> GetActions(Pony pony);
}

}