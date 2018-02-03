using UnityEngine;

/// <summary>
/// Controller for managing the currently loaded lot.
/// </summary>
public class PropertyController : MonoBehaviour {
	public GameObject terrainTilePrefab;
	public GameObject wallPrefab;
	public GameObject propertyObjectPrefab;
	public Property property;
	private int nextObjectId;

	/// <summary>
	/// Called when the scene is just started and a property should be loaded.
	/// </summary>
	/// <param name="propertyId">The id of the property to load.</param>
	public void Initialize (int propertyId) {
		property = new PropertyLoader().LoadOrCreateProperty(propertyId);
		foreach (PropertyObject propertyObject in property.propertyObjects) {
			if (propertyObject.id >= nextObjectId) {
				nextObjectId = propertyObject.id + 1;
			}
		}
		PlaceTerrainTiles();
		PlaceWalls();
		PlacePropertyObjects();
	}

	/// <summary>
	/// Save the currently open property.
	/// </summary>
	public void SaveProperty () {
		new PropertyLoader().SaveProperty(property);
	}
	
	/// <summary>
	/// Place "dummy" instances of all loaded terrain tiles, so they are visible and interactable.
	/// </summary>
	public void PlaceTerrainTiles () {
		foreach (TerrainTile tt in property.terrainTiles) {
			tt.PlaceTile(terrainTilePrefab);
		}
	}
	
	/// <summary>
	/// Place "dummy" instances of all loaded walls, so they are visible and interactable.
	/// </summary>
	public void PlaceWalls () {
		foreach (Wall w in property.walls) {
			w.PlaceWall(wallPrefab);
		}
	}

	/// <summary>
	/// Place "dummy" instances of all loaded property objects, so they are visible and interactable.
	/// </summary>
	public void PlacePropertyObjects () {
		foreach (PropertyObject po in property.propertyObjects) {
			po.PlaceObject(propertyObjectPrefab);
		}
	}

	/// <summary>
	/// Add a new property object to the property. This will also instantiate a dummy to make the object visible and interactable.
	/// </summary>
	/// <param name="x">The X position of the object.</param>
	/// <param name="y">The Y position of the object.</param>
	/// <param name="objectRotation">The rotation of the object.</param>
	/// <param name="preset">The FurniturePreset that this object is based on.</param>
	/// <param name="skin">The number of the skin to apply to this object.</param>
	public void PlacePropertyObject (int x, int y, ObjectRotation objectRotation, FurniturePreset preset, int skin) {
		PropertyObject propertyObject = new PropertyObject(nextObjectId++, x, y, objectRotation, preset, skin);
		property.propertyObjects.Add(propertyObject);
		propertyObject.PlaceObject(propertyObjectPrefab);
	}

	/// <summary>
	/// Remove a property object from the property. This will also clean up the dummy object in the scene.
	/// </summary>
	/// <param name="propertyObject">The PropertyObject to remove.</param>
	public void RemovePropertyObject (PropertyObject propertyObject) {
		propertyObject.RemoveObject();
		property.propertyObjects.Remove(propertyObject);
	}
}
