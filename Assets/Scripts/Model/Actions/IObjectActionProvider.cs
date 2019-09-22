using System.Collections.Generic;
using Model.Ponies;
using Model.Property;

namespace Model.Actions {

public interface IObjectActionProvider {
    IEnumerable<ObjectAction> GetActions(Pony pony, PropertyObject target);

    ObjectAction LoadAction(string identifier, Pony pony, PropertyObject target);
}

}