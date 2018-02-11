using UnityEngine;

/// <summary>
/// Tool for build mode that deals with placing and removing floors.
/// </summary>
public class FloorTool : Tool {
	private const int LAYER_TERRAIN = 8;
	
	public GameObject buildMarkerPrefab;
	public AudioClip denySound;
	public AudioClip placeSound;
	public PropertyController propertyController;
	public HUDController hudController;
	public AudioSource audioSource;

	private FloorPreset placingPreset;
	private GameObject buildMarker;
	private TerrainTile targetTile;
	private bool pressingTile;
	private bool canPlace;

	public void OnDisable () {
		placingPreset = null;
	}

	public void Update () {
		if (placingPreset != null) {
			if (pressingTile) {
				HandlePlacementHolding();
			} else {
				UpdateBuildMarker();
			}
		}
	}
	
	public override void SetSelectedPreset (CatalogItem catalogItem, int skin) {
		FloorPreset floorPreset = catalogItem as FloorPreset;
		if (floorPreset == null) {
			Debug.LogWarning(catalogItem + " is not a FloorPreset, cannot be set to FloorTool.");
			return;
		}
		placingPreset = floorPreset;
		CreateBuildMarker();
	}

	private void CreateBuildMarker () {
		buildMarker = Instantiate(buildMarkerPrefab);
		placingPreset.ApplyToGameObject(buildMarker);
		SetBuildMarkerPosition(0, 0);
	}

	private void UpdateBuildMarker () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (!hudController.IsMouseOverGui() && Physics.Raycast(ray, out hit, 1000, 1 << LAYER_TERRAIN)) {
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
		canPlace = CanPlaceFloor();
		SetBuildMarkerPosition(targetTile.x, targetTile.y);
	}

	private bool CanPlaceFloor () {
		return placingPreset.price <= hudController.GetFunds();
	}

	private void SetBuildMarkerPosition (int x, int y) {
		buildMarker.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
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
		} else {
			FloorTile currentTile = propertyController.property.GetFloorTile(targetTile.x, targetTile.y);
			if (currentTile != null) {
				if (currentTile.preset != placingPreset) {
					SellFloor(currentTile);
				} else {
					return;
				}
			}
			audioSource.PlayOneShot(placeSound);
			hudController.ChangeFunds(-placingPreset.price);
			propertyController.PlaceFloor(targetTile.x, targetTile.y, placingPreset);
			BuildMarkerMoved(targetTile);
		}
	}

	private void SellFloor (FloorTile floorTile) {
		hudController.ChangeFunds(floorTile.preset.GetSellValue());
		propertyController.RemoveFloor(floorTile);
	}
}
