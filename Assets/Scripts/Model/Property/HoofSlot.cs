using UnityEngine;

namespace Model.Property {

public class HoofSlot : MonoBehaviour, IObjectSlot {
    public Vector3 SlotPosition => transform.position;
    
    /// <summary>
    /// The object that is occupying this slot.
    /// Null if the slot is empty.
    /// </summary>
    public PropertyObject SlotObject { get; set; }

    public void PlaceObject(PropertyObject propertyObject) {
        propertyObject.ClearParent();
        SlotObject = propertyObject;
        Transform objectTransform = propertyObject.transform;
        objectTransform.parent = transform;
        objectTransform.localPosition = Vector3.zero;
        objectTransform.localRotation = Quaternion.identity;
        propertyObject.OnPlaced();
    }
}

}