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

    /// <summary>
    /// The object that is occupying this slot.
    /// Null if the slot is empty.
    /// </summary>
    [CanBeNull]
    PropertyObject SlotObject { get; set; }
}

}