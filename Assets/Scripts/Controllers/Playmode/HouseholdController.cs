using Assets.Scripts.Controllers;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using UnityEngine;

namespace Controllers.Playmode {

public class HouseholdController : SingletonMonoBehaviour<HouseholdController> {
    public PieMenu pieMenuPrefab;
    public ActionQueue actionQueue;
    [CanBeNull] public Household Household => PropertyController.Instance.property.household;
    [CanBeNull] public Pony selectedPony;

    private PieMenu pieMenu;

    public void SetSelectedPony(int index) {
        if (selectedPony != null) {
            selectedPony.SetSelected(false);
        }
        selectedPony = Household.ponies[index];
        selectedPony.SetSelected(true);
        actionQueue.UpdateActions(selectedPony.queuedActions);
    }

    private void Update() {
        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) && pieMenu != null) {
            Destroy(pieMenu.gameObject);
        }
        if (ModeController.Instance.CurrentMode == HudPanel.Live && selectedPony != null) {
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
            HandleHover(terrainTile);
            return;
        }
        Transform hitParent = hit.transform.parent;
        if (hitParent != null) {
            PropertyObject propertyObject = hit.transform.parent.GetComponent<PropertyObject>();
            if (propertyObject != null) {
                cursorController.SetCursor(CursorType.Interact);
                HandleHover(propertyObject);
                return;
            }
            Pony pony = hit.transform.parent.GetComponent<Pony>();
            if (pony != null) {
                cursorController.SetCursor(CursorType.InteractPony);
                HandleHover(pony);
                return;
            }
        }
        cursorController.SetCursor(CursorType.Normal);
    }

    private void HandleHover(IActionProvider actionProvider) {
        if (Input.GetMouseButtonUp(0)) {
            if (pieMenu != null) {
                Destroy(pieMenu.gameObject);
            }
            pieMenu = Instantiate(pieMenuPrefab, transform.parent);
            RectTransform pieTransform = pieMenu.GetComponent<RectTransform>();
            Vector2 position = Input.mousePosition;
            Vector2 size = pieTransform.sizeDelta;
            
            // Make sure the pie menu fits inside the screen
            position.x = Mathf.Clamp(position.x, size.x / 2, Screen.width - size.x / 2);
            position.y = Mathf.Clamp(position.y, size.y / 2, Screen.height - size.y / 2);
            
            pieTransform.anchoredPosition = position;
            pieMenu.Init(selectedPony, actionProvider.GetActions(selectedPony));
        }
    }
}

}