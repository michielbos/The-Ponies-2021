using UnityEngine;

namespace Model.Property {

/// <summary>
/// The "dummy" of a property object, used for displaying and interaction.
/// It should only be controlled by its owner, which is in the propertyObject attribute.
/// </summary>
public class PropertyObjectDummy : MonoBehaviour {
	public PropertyObject propertyObject;
}

}
