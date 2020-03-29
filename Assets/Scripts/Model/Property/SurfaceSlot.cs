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
        propertyObject.ClearParent();
        SlotObject = propertyObject;
        Transform objectTransform = propertyObject.transform;
        objectTransform.parent = transform;
        objectTransform.localPosition = Vector3.zero;
        objectTransform.localRotation = Quaternion.identity;
        propertyObject.OnPlaced();
    }

    public static SurfaceSlot CreateSlot(PropertyObject propertyObject, Vector3 localPosition) {
        GameObject gameObject = new GameObject("Slot");
        Transform transform = gameObject.transform;
        transform.parent = propertyObject.Model;
        transform.localPosition = localPosition;
        SurfaceSlot slot = gameObject.AddComponent<SurfaceSlot>();
        slot.SlotOwner = propertyObject;
        return slot;
    }

    /// <summary>
    /// Match the Y-position of this surface slot with the owner object below it.
    /// This is might be just a temporary solution until we implement something to make the slots configurable in PoneCrafter.
    /// </summary>
    public void MatchHeightWithOwner() {
        Vector3 position = transform.position;
        RaycastHit[] hits = Physics.RaycastAll(new Vector3(position.x, 5, position.z), Vector3.down, 5,
            LayerMask.GetMask("Default"));

        // Check if the slot's owner was hit with the raycasts and copy the hit height.
        foreach (RaycastHit hit in hits) {
            if (hit.transform?.parent?.parent == SlotOwner.transform) {
                transform.position = new Vector3(position.x, hit.point.y, position.z);
                return;
            }
        }
    }
}

}