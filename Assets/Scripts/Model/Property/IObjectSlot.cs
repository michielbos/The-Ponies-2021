using JetBrains.Annotations;
using UnityEngine;

namespace Model.Property {

/// <summary>
/// A slot that can contain a property object.
/// Each terrain tile is an object slot. Surface objects can contain zero or more object slots.
/// </summary>
public interface IObjectSlot {
    /// <summary>
    /// The position of this slot in world space.
    /// </summary>
    Vector3 SlotPosition { get; }
    
    PropertyObject SlotObject { get; set; }

    /// <summary>
    /// Place a PropertyObject on this slot.
    /// This should update the object's position (and optionally parent) to match.
    /// </summary>
    void PlaceObject(PropertyObject propertyObject);
}

}