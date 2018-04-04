using Assets.Scripts.Controllers;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

/// <summary>
/// Tool for build mode that deals with placing and removing floors.
/// </summary>
[CreateAssetMenu(fileName = "FloorTool", menuName = "Tools/Floor Tool", order = 10)]
public class FloorTool : ScriptableObject, ITool
{
	private const int LAYER_TERRAIN = 8;
	
	public GameObject buildMarkerPrefab;

	private FloorPreset placingPreset;
	private GameObject buildMarker;
	private TerrainTile targetTile;
	private bool pressingTile;
	private bool canPlace;

	public void UpdateTool(Vector3 tilePosition, Vector2Int tileIndex) {
		if (placingPreset != null) {
			if (pressingTile) {
				HandlePlacementHolding();
			} else {
				UpdateBuildMarker();
			}
		}
	}
	
	public void Enable() {
		CreateBuildMarker();
	}

	public void Disable() {
		placingPreset = null;
	}

	public void OnClicked(MouseButton button) {
	}

	private void CreateBuildMarker () {
		buildMarker = Instantiate(buildMarkerPrefab);
		placingPreset.ApplyToGameObject(buildMarker);
		SetBuildMarkerPosition(0, 0);
	}

	private void UpdateBuildMarker () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (!HUDController.Instance.IsMouseOverGui() && Physics.Raycast(ray, out hit, 1000, 1 << LAYER_TERRAIN)) {
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
		return placingPreset.price <= MoneyController.Instance.Funds;
	}

	private void SetBuildMarkerPosition (int x, int y) {
		buildMarker.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
	}

	private void HandlePlacementHolding () {
		if (Input.GetMouseButtonUp(0)) {
			if (Input.GetKey(KeyCode.LeftControl)) {
				FloorTile selectedTile = PropertyController.Instance.property.GetFloorTile(targetTile.x, targetTile.y);
				if (selectedTile != null) {
					SoundController.Instance.PlaySound(SoundType.Place);
					SellFloor(selectedTile);
				}
			} else {
				PlaceObject();
			}
			pressingTile = false;
		}
	}

	private void PlaceObject () {
		if (!canPlace) {
			SoundController.Instance.PlaySound(SoundType.Deny);
		} else {
			FloorTile currentTile = PropertyController.Instance.property.GetFloorTile(targetTile.x, targetTile.y);
			if (currentTile != null) {
				if (currentTile.preset != placingPreset) {
					SellFloor(currentTile);
				} else {
					return;
				}
			}
			SoundController.Instance.PlaySound(SoundType.Place);
			MoneyController.Instance.ChangeFunds(-placingPreset.price);
			PropertyController.Instance.PlaceFloor(targetTile.x, targetTile.y, placingPreset);
			BuildMarkerMoved(targetTile);
		}
	}

	private void SellFloor (FloorTile floorTile) {
		MoneyController.Instance.ChangeFunds(floorTile.preset.GetSellValue());
		PropertyController.Instance.RemoveFloor(floorTile);
	}

    public void OnCatalogSelect(CatalogItem item, int skin)
    {
        FloorPreset floorPreset = item as FloorPreset;
        if (floorPreset == null)
        {
            Debug.LogWarning(item + " is not a FloorPreset, cannot be set to FloorTool.");
            return;
        }
        placingPreset = floorPreset;
        CreateBuildMarker();
    }
}
