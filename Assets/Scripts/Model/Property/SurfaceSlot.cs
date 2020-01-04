using UnityEngine;
using Util;

namespace Model.Property {

/// <summary>
/// Used by property objects to specify the slots of surfaces.
/// </summary>
public class SurfaceSlot : MonoBehaviour, IObjectSlot {
    public Vector3 SlotPosition => transform.position;
    
    /// <summary>
    /// The object that is occupying this slot.
    /// Null if the slot is empty.
    /// </summary>
    public PropertyObject SlotObject { get; set; }

    public PropertyObject SlotOwner { get; private set; }
    
    public void PlaceObject(PropertyObject propertyObject) {
        SlotObject = propertyObject;
        Transform objectTransform = propertyObject.transform;
        objectTransform.parent = transform;
        objectTransform.localPosition = Vector3.zero;
    }

    public static SurfaceSlot CreateSlot(PropertyObject propertyObject, Vector3 localPosition) {
        GameObject gameObject = new GameObject("Slot");
        Transform transform = gameObject.transform;
        transform.parent = propertyObject.transform;
        transform.localPosition = localPosition;
        SurfaceSlot slot = gameObject.AddComponent<SurfaceSlot>();
        slot.SlotOwner = propertyObject;
        return slot;
    }

    public void Rotate(ObjectRotation previousRotation, ObjectRotation newRotation) {
        int turns = previousRotation.GetTurnsTo(newRotation);
        Transform transform = this.transform;
        transform.localPosition = transform.localPosition.RotateInGroup((ObjectRotation) (turns + 1));
    }
}

}