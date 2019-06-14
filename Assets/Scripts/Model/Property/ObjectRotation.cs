using UnityEngine;

namespace Model.Property {

/// <summary>
/// Enum for the rotation of furniture objects.
/// SouthEast is the default rotation, which is when the object faces towards the lower-right on the default camera orientation.
/// </summary>
public enum ObjectRotation {
    SouthEast = 1,
    SouthWest = 2,
    NorthWest = 3,
    NorthEast = 4
}

/// <summary>
/// Util class for ObjectRotation because C# doesn't seem to support any enum methods.
/// </summary>
public static class ObjectRotationUtil {
    public static float GetRotationAngle(ObjectRotation objectRotation) {
        return ((int) objectRotation - 1) * 90;
    }

    public static Vector3 GetRotationVector(ObjectRotation objectRotation) {
        return new Vector3(0, GetRotationAngle(objectRotation), 0);
    }

    public static ObjectRotation RotateClockwise(ObjectRotation current) {
        if (current < ObjectRotation.NorthEast) {
            return current + 1;
        }
        return ObjectRotation.SouthEast;
    }

    public static ObjectRotation RotateCounterClockwise(ObjectRotation current) {
        if (current > ObjectRotation.SouthEast) {
            return current - 1;
        }
        return ObjectRotation.NorthEast;
    }
}

}