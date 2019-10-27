using UnityEngine;

namespace Model.Property {

/// <summary>
/// Used by property objects to specify the slots of surfaces.
/// </summary>
public class SurfaceSlot : IObjectSlot {
    private readonly PropertyObject parent;
    private readonly Vector3 localPosition;

    public Vector3 SlotPosition => parent.transform.position + localPosition;
    public PropertyObject SlotObject { get; set; }

    public SurfaceSlot(PropertyObject parent, Vector3 localPosition) {
        this.parent = parent;
        this.localPosition = localPosition;
    }
}

}