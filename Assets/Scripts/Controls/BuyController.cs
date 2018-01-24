using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for buy mode that deals with buying, moving and selling furniture.
/// </summary>
public class BuyController : MonoBehaviour {
	public GameObject buildMarkerPrefab;
	public AudioClip buySound;
	public AudioClip denySound;
	public AudioClip sellSound;
	public AudioClip placeSound;
	public AudioClip rotateSound;
	public PropertyController propertyController;
	public AudioSource audioSource;

	private FurniturePreset placingPreset = null;
	private PropertyObject movingObject = null;
	private GameObject buildMarker;
	private TerrainTile pressedTile = null;

	public void OnDisable () {
		ClearSelection();
	}

	public void Update () {
		if (placingPreset != null || movingObject != null) {
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
		buildMarker = Instantiate(buildMarkerPrefab);
	}

	private void UpdateBuildMarker () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000, 1 << 8)) {
			buildMarker.transform.position = new Vector3(hit.transform.position.x + 0.5f, 0, hit.transform.position.z + 0.5f);
			if (Input.GetMouseButtonDown(0)) {
				pressedTile = hit.collider.GetComponent<TerrainTileDummy>().terrainTile;
			}
		} else {
			buildMarker.transform.position = new Vector3(0, -100, 0);
		}
	}

	private void HandlePlacementHolding () {
		if (Input.GetMouseButtonUp(0)) {
			PlaceObject();
		}
	}

	private void PlaceObject () {
		if (movingObject != null) {
			audioSource.PlayOneShot(placeSound);
			movingObject.x = Mathf.FloorToInt(buildMarker.transform.position.x);
			movingObject.y = Mathf.FloorToInt(buildMarker.transform.position.z);
			ClearSelection();
		} else {
			audioSource.PlayOneShot(buySound);
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
		if (Physics.Raycast(ray, out hit, 1000)) {
			//TODO: Highlight object if it can be picked up.
			if (Input.GetMouseButtonDown(0)) {
				PropertyObjectDummy dummy = hit.collider.GetComponent<PropertyObjectDummy>();
				if (dummy != null) {
					movingObject = dummy.propertyObject;
					buildMarker = movingObject.dummyObject;
				}
			}
		}
	}

	private void SellSelection () {
		if (movingObject != null) {
			audioSource.PlayOneShot(sellSound);
			propertyController.RemovePropertyObject(movingObject);
			//TODO: Refund
		}
		ClearSelection();
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
	}
}
