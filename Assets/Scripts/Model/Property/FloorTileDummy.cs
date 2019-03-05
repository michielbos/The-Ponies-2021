using UnityEngine;

namespace Model.Property {

/// <summary>
/// The "dummy" of a floor tile, used for displaying and interaction.
/// It should only be controlled by its owner, which is in the floorTile attribute.
/// </summary>
public class FloorTileDummy : MonoBehaviour {
	public FloorTile floorTile;
}

}
