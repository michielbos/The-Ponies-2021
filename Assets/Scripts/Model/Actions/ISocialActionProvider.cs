using System.Collections.Generic;
using Model.Ponies;

namespace Model.Actions {

public interface ISocialActionProvider {
    IEnumerable<SocialAction> GetActions(Pony pony, Pony target);

    SocialAction LoadAction(string identifier, Pony pony, Pony target);
}

}