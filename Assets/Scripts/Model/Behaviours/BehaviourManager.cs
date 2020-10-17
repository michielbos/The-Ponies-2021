using System;
using System.Collections.Generic;
using Model.Actions;
using Model.Property;

namespace Model.Behaviours {

/// <summary>
/// Manager for content/mod-provided MonoBehaviours.
/// </summary>
public static class BehaviourManager {
    private static readonly List<IObjectBehaviourProvider> objectBehaviourProviders =
        new List<IObjectBehaviourProvider>();

    public static void AddObjectBehaviourProvider(IObjectBehaviourProvider behaviourProvider) {
        objectBehaviourProviders.Add(behaviourProvider);
    }

    /// <summary>
    /// Get all object behaviours that should be added to a given object.
    /// </summary>
    public static ICollection<Type> GetObjectBehaviours(PropertyObject propertyObject) {
        List<Type> behaviours = new List<Type>();
        foreach (IObjectBehaviourProvider provider in objectBehaviourProviders) {
            IEnumerable<Type> objectBehaviours = provider.GetBehaviours(propertyObject);
            if (objectBehaviours != null) {
                behaviours.AddRange(objectBehaviours);
            }
        }
        return behaviours;
    }
}

}