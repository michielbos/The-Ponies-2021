using System;
using System.Collections.Generic;
using Model.Ponies;
using Model.Property;

namespace Model.Actions {

public static class ActionManager {
    private static List<IObjectActionProvider> objectActionProviders = new List<IObjectActionProvider>();

    public static void AddObjectActionProviders(IEnumerable<IObjectActionProvider> actionProviders) {
        objectActionProviders.AddRange(actionProviders);
    }

    public static ICollection<PonyAction> GetActionsForObject(Pony pony, PropertyObject propertyObject) {
        List<PonyAction> objectActions = new List<PonyAction>();
        foreach (IObjectActionProvider actionProvider in objectActionProviders) {
            objectActions.AddRange(actionProvider.GetActions(pony, propertyObject));
        }
        return objectActions;
    }
}

}