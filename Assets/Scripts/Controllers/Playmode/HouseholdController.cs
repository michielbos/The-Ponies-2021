using Assets.Scripts.Controllers;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using Model.Ponies;
using Model.Property;
using UnityEngine;

namespace Controllers.Playmode {

public class HouseholdController : SingletonMonoBehaviour<HouseholdController> {
    [CanBeNull] public Household Household => PropertyController.Instance.property.household;
    [CanBeNull] public Pony selectedPony; 

    public void SetSelectedPony(int index) {
        if (selectedPony != null) {
            selectedPony.SetSelected(false);
        }
        selectedPony = Household.ponies[index];
        selectedPony.SetSelected(true);
    }

    private void Update() {
        if (ModeController.Instance.CurrentMode == HudPanel.Live) {
            HandlePlayerInteraction();
        }
    }

    private void HandlePlayerInteraction() {
        CursorController cursorController = CursorController.Instance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (HUDController.GetInstance().IsMouseOverGui() || !Physics.Raycast(ray, out hit, 1000)) {
            cursorController.SetCursor(CursorType.Normal);
            return;
        }
        TerrainTile terrainTile = hit.transform.GetComponent<TerrainTile>();
        if (terrainTile != null) {
            cursorController.SetCursor(CursorType.GoHere);
            return;
        }
        PropertyObject propertyObject = hit.transform.parent.GetComponent<PropertyObject>();
        if (propertyObject != null) {
            cursorController.SetCursor(CursorType.Interact);
            return;
        }
        Pony pony = hit.transform.parent.GetComponent<Pony>();
        if (pony != null) {
            cursorController.SetCursor(CursorType.InteractPony);
            return;
        }
        cursorController.SetCursor(CursorType.Normal);
    }
}

}