using UnityEngine;
using Util;

namespace Model.Property {

/// <summary>
/// Used by property objects to specify the slots of surfaces.
/// </summary>
public class SurfaceSlot : MonoBehaviour, IObjectSlot {
    public Vector3 SlotPosition => transform.position;
    public PropertyObject SlotObject { get; set; }

    public static SurfaceSlot CreateSlot(PropertyObject propertyObject, Vector3 localPosition) {
        GameObject gameObject = new GameObject("Slot");
        Transform transform = gameObject.transform;
        transform.parent = propertyObject.transform;
        transform.localPosition = localPosition;
        return gameObject.AddComponent<SurfaceSlot>();
    }

    public void Rotate(ObjectRotation previousRotation, ObjectRotation newRotation) {
        int turns = previousRotation.GetTurnsTo(newRotation);
        Transform transform = this.transform;
        transform.localPosition = transform.localPosition.RotateInGroup((ObjectRotation) (turns + 1));
    }
}

}