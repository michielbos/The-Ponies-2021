using System;
using System.Collections.Generic;
using Model.Ponies;
using Model.Property;

namespace Model.Actions {

public static class ActionManager {
    private static readonly List<IObjectActionProvider> objectActionProviders = new List<IObjectActionProvider>();
    private static readonly List<ITileActionProvider> tileActionProviders = new List<ITileActionProvider>();

    public static void AddObjectActionProviders(IEnumerable<IObjectActionProvider> actionProviders) {
        objectActionProviders.AddRange(actionProviders);
    }
    
    public static void AddTileActionProviders(IEnumerable<ITileActionProvider> actionProviders) {
        tileActionProviders.AddRange(actionProviders);
    }

    public static ICollection<PonyAction> GetActionsForObject(Pony pony, PropertyObject propertyObject) {
        List<PonyAction> objectActions = new List<PonyAction>();
        foreach (IObjectActionProvider actionProvider in objectActionProviders) {
            objectActions.AddRange(actionProvider.GetActions(pony, propertyObject));
        }
        return objectActions;
    }
    
    public static ICollection<PonyAction> GetActionsForTile(Pony pony, TerrainTile tile) {
        List<PonyAction> tileActions = new List<PonyAction>();
        foreach (ITileActionProvider actionProvider in tileActionProviders) {
            tileActions.AddRange(actionProvider.GetActions(pony, tile));
        }
        return tileActions;
    }
}

}