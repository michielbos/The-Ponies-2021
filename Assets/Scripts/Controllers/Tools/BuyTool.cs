using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Util;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

/// <summary>
/// Tool for buy/build mode that deals with buying, moving and selling furniture.
/// </summary>
/// 
[CreateAssetMenu(fileName = "BuyTool", menuName = "Tools/Buy Tool", order = 10)]
public class BuyTool : ScriptableObject, ITool
{
	private const int LAYER_TERRAIN = 8;

	public GameObject buildMarkerPrefab;
	public GameObject buyMarkingPrefab;
	public Material buyMarkingNormalMaterial;
	public Material buyMarkingDisallowedMaterial;

	private FurniturePreset placingPreset;
	private int placingSkin;

    private PropertyObject movingObject;

	private GameObject buildMarker;
	private ObjectRotation markerRotation = ObjectRotation.SouthEast;
	private List<GameObject> buyMarkings;
	private TerrainTile targetTile;
	private bool pressingTile;
	private bool canPlace;

	public BuyTool()
	{
		buyMarkings = new List<GameObject>();
	}

	public void OnDisable()
	{
		ClearSelection();
	}

	/// <summary>
	/// Set the furniture preset that is being placed.
	/// </summary>
	/// <param name="catalogItem">The FurniturePreset that is being placed.</param>
	/// <param name="skin">The skin that should be applied to the preset.</param>

	private void CreateBuildMarker()
	{
		buildMarker = Instantiate(buildMarkerPrefab);
		placingPreset.ApplyToGameObject(buildMarker, buildMarker.transform.position, buildMarker.transform.eulerAngles, placingSkin, true);
		SetBuildMarkerPosition(0, 0);
		PlaceBuyMarkings(0, 0);
	}

	private void PlaceBuyMarkings(int x, int y)
	{
		foreach (Vector2Int tile in GetMovingPreset().occupiedTiles)
		{
			buyMarkings.Add(Instantiate(buyMarkingPrefab, new Vector3(x + tile.x + 0.5f, 0.01f, y + tile.y + 0.5f),
				buyMarkingPrefab.transform.rotation, buildMarker.transform));
		}
	}

	private FurniturePreset GetMovingPreset()
	{
		if (placingPreset != null)
			return placingPreset;
		return movingObject != null ? movingObject.preset : null;
	}

	private void RemoveBuyMarkings()
	{
		foreach (GameObject buyMarking in buyMarkings)
		{
			Destroy(buyMarking);
		}
		buyMarkings.Clear();
	}

	private void UpdateBuildMarker()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (!HUDController.GetInstance().IsMouseOverGui() && Physics.Raycast(ray, out hit, 1000, 1 << LAYER_TERRAIN))
		{
			TerrainTile newTargetTile = hit.collider.GetComponent<TerrainTileDummy>().terrainTile;
			if (newTargetTile != targetTile)
			{
				BuildMarkerMoved(newTargetTile);
			}
			if (Input.GetMouseButtonDown(0))
			{
				pressingTile = true;
			}
		}
		else if (targetTile != null)
		{
			buildMarker.transform.position = new Vector3(0, -100, 0);
			targetTile = null;
		}
	}

	private void BuildMarkerMoved(TerrainTile newTile)
	{
		targetTile = newTile;
		canPlace = CanPlaceObject();
		foreach (GameObject buyMarking in buyMarkings)
		{
			buyMarking.GetComponent<Renderer>().material =
				canPlace ? buyMarkingNormalMaterial : buyMarkingDisallowedMaterial;
		}
		SetBuildMarkerPosition(targetTile.x, targetTile.y);
	}

	private bool CanPlaceObject()
	{
		FurniturePreset movingPreset = GetMovingPreset();
		Vector2Int[] requiredTiles = movingPreset.GetOccupiedTiles(new Vector2Int(targetTile.x, targetTile.y));
		List<PropertyObject> occupyingObjects = PropertyController.Instance.property.GetObjectsOnTiles(requiredTiles);
		bool canPlace = placingPreset == null || placingPreset.price <= MoneyController.Instance.Funds;
		if (canPlace && !CheatsController.Instance.moveObjectsMode)
		{
			foreach (PropertyObject occupyingObject in occupyingObjects)
			{
				if (occupyingObject != movingObject)
					return false;
			}
		}
		//TODO: Check for floors, walls, tables, etc.
		return canPlace && movingPreset.AllowsPlacement(PlacementRestriction.Terrain);
	}

	private void SetBuildMarkerPosition(int x, int y)
	{
		FurniturePreset preset = GetMovingPreset();
		buildMarker.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
		buildMarker.transform.eulerAngles = ObjectRotationUtil.GetRotationVector(markerRotation);
		preset.ApplyOffsets(buildMarker.transform);
		preset.AdjustToTiles(buildMarker.transform);
	}

	private void HandlePlacementHolding()
	{
		if (Input.GetMouseButtonUp(0))
		{
			PlaceObject();
			pressingTile = false;
		}
	}

	private void PlaceObject()
	{
		if (!canPlace)
		{
			SoundController.Instance.PlaySound(SoundType.Deny);
		}
		else if (movingObject != null)
		{
			SoundController.Instance.PlaySound(SoundType.Place);
			movingObject.x = targetTile.x;
			movingObject.y = targetTile.y;
			ClearSelection();
		}
		else
		{
			SoundController.Instance.PlaySound(SoundType.Buy);
			MoneyController.Instance.ChangeFunds(-placingPreset.price);
			PropertyController.Instance.PlacePropertyObject(targetTile.x, targetTile.y, ObjectRotation.SouthEast, placingPreset, placingSkin);
			if (Input.GetKey(KeyCode.LeftShift))
			{
				BuildMarkerMoved(targetTile);
			}
			else
			{
				ClearSelection();
			}
		}
	}

	private void HandleHovering()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (HUDController.GetInstance().IsMouseOverGui() || !Physics.Raycast(ray, out hit, 1000))
			return;
		PropertyObjectDummy dummy = hit.collider.GetComponent<PropertyObjectDummy>();
		if (dummy == null)
			return;
		//TODO: Highlight object if it can be picked up.
		if (!Input.GetMouseButtonDown(0))
			return;
		if (dummy.propertyObject.preset.pickupable || CheatsController.Instance.moveObjectsMode)
		{
			PickUpObject(dummy.propertyObject);
		}
		else
		{
			SoundController.Instance.PlaySound(SoundType.Deny);
		}
	}

	private void PickUpObject(PropertyObject propertyObject)
	{
		movingObject = propertyObject;
		buildMarker = movingObject.dummyObject;
		PlaceBuyMarkings(movingObject.x, movingObject.y);
	}

	private void SellSelection()
	{
		if (movingObject == null)
		{
			ClearSelection();
		}
		else if (movingObject.preset.sellable || CheatsController.Instance.moveObjectsMode)
		{
			SoundController.Instance.PlaySound(SoundType.Sell);
			PropertyController.Instance.RemovePropertyObject(movingObject);
			MoneyController.Instance.ChangeFunds(movingObject.value);
			ClearSelection();
		}
		else
		{
			SoundController.Instance.PlaySound(SoundType.Deny);
		}
	}

	private void ClearSelection()
	{
		placingPreset = null;
		if (movingObject != null && movingObject.dummyObject.Exists())
		{
			movingObject.RefreshDummy();
			movingObject = null;
			buildMarker = null;
		}
		if (buildMarker != null)
		{
			Destroy(buildMarker);
			buildMarker = null;
		}
		RemoveBuyMarkings();
	}

    public void UpdateTool(Vector3 tilePosition, Vector2Int tileIndex)
    {
        if (GetMovingPreset() != null)
        {
            if (pressingTile)
            {
                HandlePlacementHolding();
            }
            else
            {
                UpdateBuildMarker();
            }
        }
        else
        {
            HandleHovering();
        }

        if (Input.GetKey(KeyCode.Delete))
        {
            SellSelection();
        }
    }

    public void OnCatalogSelect(CatalogItem item, int skin)
    {
        FurniturePreset furniturePreset = item as FurniturePreset;
        if (furniturePreset == null)
        {
            Debug.LogWarning(item + " is not a FurniturePreset, cannot be set to BuyTool.");
            return;
        }
        ClearSelection();
        placingPreset = furniturePreset;
        placingSkin = skin;
        CreateBuildMarker();
    }

    public void Enable()
    {
        pressingTile = false;
        movingObject = null;
        buildMarker = null;
        placingPreset = null;
    }

    public void Disable()
    {
    }

    public void OnClicked(MouseButton button)
    {
    }
}
