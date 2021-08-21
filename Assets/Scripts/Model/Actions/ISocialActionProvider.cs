using System.Collections.Generic;
using Model.Ponies;

namespace Model.Actions {

public interface ISocialActionProvider {
    IEnumerable<ObjectAction> GetActions(Pony pony, Pony target);

    ObjectAction LoadAction(string identifier, Pony pony, Pony target);
}

}