using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for buy mode that deals with buying, moving and selling furniture.
/// </summary>
public class BuyController : MonoBehaviour {
	public GameObject buildMarkerPrefab;
	public GameObject buyMarkingPrefab;
	public AudioClip buySound;
	public AudioClip denySound;
	public AudioClip sellSound;
	public AudioClip placeSound;
	public AudioClip rotateSound;
	public PropertyController propertyController;
	public HUDController hudController;
	public CheatsController cheatsController;
	public AudioSource audioSource;

	private FurniturePreset placingPreset;
	private PropertyObject movingObject;
	private GameObject buildMarker;
	private ObjectRotation markerRotation = ObjectRotation.SouthEast;
	private List<GameObject> buyMarkings;
	private TerrainTile pressedTile;

	public BuyController () {
		buyMarkings = new List<GameObject>();
	}

	public void OnDisable () {
		ClearSelection();
	}

	public void Update () {
		if (GetMovingPreset() != null) {
			if (pressedTile == null) {
				UpdateBuildMarker();
			} else {
				HandlePlacementHolding();
			}
		} else {
			HandleHovering();
		}

		if (Input.GetKey(KeyCode.Delete)) {
			SellSelection();
		}
	}
	
	public void SetPlacingPreset (FurniturePreset furniturePreset) {
		ClearSelection();
		placingPreset = furniturePreset;
		CreateBuildMarker();
	}

	private void CreateBuildMarker () {
		buildMarker = Instantiate(buildMarkerPrefab);
		placingPreset.ApplyToGameObject(buildMarker, buildMarker.transform.position, buildMarker.transform.eulerAngles);
		SetBuildMarkerPosition(0, 0);
		PlaceBuyMarkings(0, 0);
	}

	private void PlaceBuyMarkings (int x, int y) {
		foreach (Vector2Int tile in GetMovingPreset().occupiedTiles) {
			buyMarkings.Add(Instantiate(buyMarkingPrefab, new Vector3(x + tile.x + 0.5f, 0.01f, y + tile.y + 0.5f),
				buyMarkingPrefab.transform.rotation, buildMarker.transform));
		}
	}

	private FurniturePreset GetMovingPreset () {
		if (placingPreset != null)
			return placingPreset;
		return movingObject != null ? movingObject.preset : null;
	}

	private void RemoveBuyMarkings () {
		foreach (GameObject buyMarking in buyMarkings) {
			Destroy(buyMarking);
		}
		buyMarkings.Clear();
	}

	private void UpdateBuildMarker () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000, 1 << 8)) {
			SetBuildMarkerPosition((int) hit.transform.position.x, (int) hit.transform.position.z);
			if (Input.GetMouseButtonDown(0)) {
				pressedTile = hit.collider.GetComponent<TerrainTileDummy>().terrainTile;
			}
		} else {
			buildMarker.transform.position = new Vector3(0, -100, 0);
		}
	}

	private void SetBuildMarkerPosition (int x, int y) {
		FurniturePreset preset = GetMovingPreset();
		buildMarker.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
		buildMarker.transform.eulerAngles = ObjectRotationUtil.GetRotationVector(markerRotation);
		preset.ApplyOffsets(buildMarker.transform);
		preset.AdjustToTiles(buildMarker.transform);
	}

	private void HandlePlacementHolding () {
		if (Input.GetMouseButtonUp(0)) {
			PlaceObject();
		}
	}

	private void PlaceObject () {
		if (movingObject != null) {
			audioSource.PlayOneShot(placeSound);
			movingObject.x = pressedTile.x;
			movingObject.y = pressedTile.y;
			ClearSelection();
		} else if (placingPreset.price > hudController.GetFunds()) {
			audioSource.PlayOneShot(denySound);
		} else {
			audioSource.PlayOneShot(placeSound);
			audioSource.PlayOneShot(buySound);
			hudController.ChangeFunds(-placingPreset.price);
			propertyController.PlacePropertyObject(pressedTile.x, pressedTile.y, ObjectRotation.SouthEast, placingPreset);
			if (!Input.GetKey(KeyCode.LeftShift)) {
				ClearSelection();
			}
		}
		pressedTile = null;
	}

	private void HandleHovering () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (!Physics.Raycast(ray, out hit, 1000))
			return;
		PropertyObjectDummy dummy = hit.collider.GetComponent<PropertyObjectDummy>();
		if (dummy == null)
			return;
		//TODO: Highlight object if it can be picked up.
		if (!Input.GetMouseButtonDown(0))
			return;
		if (dummy.propertyObject.preset.pickupable || cheatsController.moveObjectsMode) {
			PickUpObject(dummy.propertyObject);
		} else {
			audioSource.PlayOneShot(denySound);
		}
	}

	private void PickUpObject (PropertyObject propertyObject) {
		movingObject = propertyObject;
		buildMarker = movingObject.dummyObject;
		PlaceBuyMarkings(movingObject.x, movingObject.y);
	}

	private void SellSelection () {
		if (movingObject == null) {
			ClearSelection();
		} else if (movingObject.preset.sellable || cheatsController.moveObjectsMode) {
			audioSource.PlayOneShot(sellSound);
			propertyController.RemovePropertyObject(movingObject);
			hudController.ChangeFunds(movingObject.value);
			ClearSelection();
		} else {
			audioSource.PlayOneShot(denySound);
		}
	}

	private void ClearSelection () {
		placingPreset = null;
		if (movingObject != null) {
			movingObject.RefreshDummy();
			movingObject = null;
			buildMarker = null;
		}
		if (buildMarker != null) {
			Destroy(buildMarker);
			buildMarker = null;
		}
		RemoveBuyMarkings();
	}
}
