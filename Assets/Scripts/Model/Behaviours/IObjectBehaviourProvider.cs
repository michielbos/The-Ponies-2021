using System;
using System.Collections.Generic;
using Model.Property;

namespace Model.Behaviours {

/// <summary>
/// Provider for object behaviours.
/// </summary>
public interface IObjectBehaviourProvider {
    /// <summary>
    /// Called for every property object to determine if this behaviour provider can provide any behaviours.
    /// Behaviours can for example be provided based on the property object's tag.
    /// If this provider has no behaviours for the given object, null can be returned.
    /// </summary>
    IEnumerable<Type> GetBehaviours(PropertyObject target);
}

}