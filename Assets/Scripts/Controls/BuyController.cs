﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for buy mode that deals with buying, moving and selling furniture.
/// </summary>
public class BuyController : MonoBehaviour {
	public GameObject buildMarkerPrefab;
	public GameObject buyMarkingPrefab;
	public Material buyMarkingNormalMaterial;
	public Material buyMarkingDisallowedMaterial;
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
	private TerrainTile targetTile;
	private bool pressingTile;
	private bool canPlace;

	public BuyController () {
		buyMarkings = new List<GameObject>();
	}

	public void OnDisable () {
		ClearSelection();
	}

	public void Update () {
		if (GetMovingPreset() != null) {
			if (pressingTile) {
				HandlePlacementHolding();
			} else {
				UpdateBuildMarker();
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
		//TODO: Set skin
		placingPreset.ApplyToGameObject(buildMarker, buildMarker.transform.position, buildMarker.transform.eulerAngles, 0, true);
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
			TerrainTile newTargetTile = hit.collider.GetComponent<TerrainTileDummy>().terrainTile;
			if (newTargetTile != targetTile) {
				BuildMarkerMoved(newTargetTile);
			}
			if (Input.GetMouseButtonDown(0)) {
				pressingTile = true;
			}
		} else if (targetTile != null) {
			buildMarker.transform.position = new Vector3(0, -100, 0);
			targetTile = null;
		}
	}

	private void BuildMarkerMoved (TerrainTile newTile) {
		targetTile = newTile;
		Vector2Int[] requiredTiles = GetMovingPreset().GetOccupiedTiles(new Vector2Int(targetTile.x, targetTile.y));
		List<PropertyObject> occupyingObjects = propertyController.property.GetObjectsOnTiles(requiredTiles);
		canPlace = placingPreset == null || placingPreset.price <= hudController.GetFunds();
		if (canPlace && !cheatsController.moveObjectsMode) {
			foreach (PropertyObject occupyingObject in occupyingObjects) {
				if (occupyingObject == movingObject)
					continue;
				canPlace = false;
				break;
			}
		}
		foreach (GameObject buyMarking in buyMarkings) {
			buyMarking.GetComponent<Renderer>().material =
				canPlace ? buyMarkingNormalMaterial : buyMarkingDisallowedMaterial;
		}
		SetBuildMarkerPosition(targetTile.x, targetTile.y);
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
			pressingTile = false;
		}
	}

	private void PlaceObject () {
		if (!canPlace) {
			audioSource.PlayOneShot(denySound);
		} else if (movingObject != null) {
			audioSource.PlayOneShot(placeSound);
			movingObject.x = targetTile.x;
			movingObject.y = targetTile.y;
			ClearSelection();
		} else {
			audioSource.PlayOneShot(placeSound);
			audioSource.PlayOneShot(buySound);
			hudController.ChangeFunds(-placingPreset.price);
			//TODO: Set skin
			int tempSkin = Random.Range(0, placingPreset.furnitureSkins.Length);
			propertyController.PlacePropertyObject(targetTile.x, targetTile.y, ObjectRotation.SouthEast, placingPreset, tempSkin);
			if (Input.GetKey(KeyCode.LeftShift)) {
				BuildMarkerMoved(targetTile);
			} else {
				ClearSelection();
			}
		}
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
