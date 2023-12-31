using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Model.Data;
using Model.Ponies;
using Model.Property;
using UnityEngine;

namespace Model.Actions {
public static class ActionManager {
    private static readonly List<IObjectActionProvider> objectActionProviders = new List<IObjectActionProvider>();
    private static readonly List<ITileActionProvider> tileActionProviders = new List<ITileActionProvider>();
    private static readonly List<ISocialActionProvider> socialActionProviders = new List<ISocialActionProvider>();

    public static void AddObjectActionProviders(IEnumerable<IObjectActionProvider> actionProviders) {
        objectActionProviders.AddRange(actionProviders);
    }

    public static void AddTileActionProviders(IEnumerable<ITileActionProvider> actionProviders) {
        tileActionProviders.AddRange(actionProviders);
    }

    public static void AddSocialActionProviders(IEnumerable<ISocialActionProvider> actionProviders) {
        socialActionProviders.AddRange(actionProviders);
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

    public static ICollection<PonyAction> GetActionsForPony(Pony pony, Pony targetPony) {
        List<PonyAction> tileActions = new List<PonyAction>();
        foreach (ISocialActionProvider actionProvider in socialActionProviders) {
            tileActions.AddRange(actionProvider.GetActions(pony, targetPony));
        }
        return tileActions;
    }

    [CanBeNull]
    public static PonyAction LoadFromData(Pony pony, PonyActionData data, Property.Property property) {
        PonyAction ponyAction;
        switch (data.GetTargetType()) {
            case PonyActionData.TargetType.Object:
                ponyAction = LoadObjectAction(pony, data, property);
                break;
            case PonyActionData.TargetType.Pony:
                ponyAction = LoadSocialAction(pony, data, property);
                break;
            case PonyActionData.TargetType.Tile:
                ponyAction = LoadTileAction(pony, data, property);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        if (ponyAction == null) {
            Debug.LogWarning($"Action {data.identifier} could not be found in behaviours.");
            return null;
        }
        ponyAction.canceled = data.canceled;
        ponyAction.trigger = (PonyActionTrigger)data.trigger;
        ponyAction.Load(data.tickCount);
        ponyAction.AddDataPairs(data.data, property);
        return ponyAction;
    }

    private static PonyAction LoadObjectAction(Pony pony, PonyActionData data, Property.Property property) {
        PropertyObject target = property.GetPropertyObject(data.targetObjectid);
        return objectActionProviders
            .Select(provider => provider.LoadAction(data.identifier, pony, target))
            .FirstOrDefault(action => action != null);
    }

    private static PonyAction LoadTileAction(Pony pony, PonyActionData data, Property.Property property) {
        TerrainTile target = property.GetTerrainTile(data.targetTileX, data.targetTileY);
        return tileActionProviders
            .Select(provider => provider.LoadAction(data.identifier, pony, target))
            .FirstOrDefault(action => action != null);
    }

    private static PonyAction LoadSocialAction(Pony pony, PonyActionData data, Property.Property property) {
        Pony target = property.GetPony(new Guid(data.targetPonyUuid));
        return socialActionProviders
            .Select(provider => provider.LoadAction(data.identifier, pony, target))
            .FirstOrDefault(action => action != null);
    }
}
}