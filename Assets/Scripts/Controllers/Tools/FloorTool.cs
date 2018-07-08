using System;
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
	private Vector2Int dragPosition;
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
		placingPreset = null;
	}

	public void Disable() {
		
	}

	public void OnClicked(MouseButton button) {
		
	}

	private void CreateBuildMarker () {
		if (buildMarker != null) {
			Destroy(buildMarker);
		}
		buildMarker = Instantiate(buildMarkerPrefab);
		placingPreset.ApplyToGameObject(buildMarker);
		SetBuildMarkerPosition(0, 0);
	}

	private void UpdateBuildMarker () {
		TerrainTile newTargetTile = GetTileUnderMouse();
		if (newTargetTile != null) {
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

	private TerrainTile GetTileUnderMouse() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (!HUDController.Instance.IsMouseOverGui() && Physics.Raycast(ray, out hit, 1000, 1 << LAYER_TERRAIN)) {
			return hit.collider.GetComponent<TerrainTileDummy>().terrainTile;
		}
		return null;
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
				CreateBuildMarker();
			}
			pressingTile = false;
		} else {
			HandleFloorDragging();
		}
	}

	private void HandleFloorDragging() {
		TerrainTile terrainTile = GetTileUnderMouse();
		if (terrainTile == null)
			return;
		dragPosition = terrainTile.GetPosition();
		int x1 = Math.Min(targetTile.x, dragPosition.x);
		int x2 = Math.Max(targetTile.x, dragPosition.x) + 1;
		int y1 = Math.Min(targetTile.y, dragPosition.y);
		int y2 = Math.Max(targetTile.y, dragPosition.y) + 1;
		RectInt dragRect = new RectInt();
		dragRect.SetMinMax(new Vector2Int(x1, y1), new Vector2Int(x2, y2));
		Transform markerTransform = buildMarker.transform;
		markerTransform.localScale = new Vector3(dragRect.width, markerTransform.localScale.y, dragRect.height);
		markerTransform.position = new Vector3(
			dragRect.x + dragRect.width / 2f,
			markerTransform.position.y,
			dragRect.y + dragRect.height / 2f
		);
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
        if (floorPreset == null) {
            Debug.LogWarning(item + " is not a FloorPreset, cannot be set to FloorTool.");
            return;
        }
        placingPreset = floorPreset;
        CreateBuildMarker();
    }
}
