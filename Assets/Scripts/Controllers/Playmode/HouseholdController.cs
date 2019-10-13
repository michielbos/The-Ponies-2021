using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;
using Controllers.Singletons;
using JetBrains.Annotations;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using UI;
using UnityEngine;

namespace Controllers.Playmode {

public class HouseholdController : SingletonMonoBehaviour<HouseholdController> {
    private int interactionLayer;
    
    public PieMenu pieMenuPrefab;
    public ActionQueue actionQueue;
    public PlayableCharactersView playableCharactersView;
    public NeedsPanel needsPanel;
    [CanBeNull] public Household Household => PropertyController.Instance.property.household;
    [CanBeNull] public Pony selectedPony;

    private PieMenu pieMenu;

    private void Start() {
        interactionLayer = LayerMask.GetMask("Default", "Terrain");
        if (selectedPony == null && Household?.ponies.Count > 0) {
            SetSelectedPony(Household.ponies.Values.First());
        }
    }

    public void SetSelectedPony(Pony pony) {
        if (selectedPony != null) {
            selectedPony.SetSelected(false);
        }
        selectedPony = pony;
        selectedPony.SetSelected(true);
        actionQueue.UpdateActions(selectedPony.queuedActions);
        playableCharactersView.UpdateHousehold();
        needsPanel.UpdateNeeds(pony.needs);
    }

    /// <summary>
    /// Called when all ponies (and other tick listeners) have been ticked.
    /// </summary>
    public void AfterTick() {
        if (selectedPony != null) {
            needsPanel.UpdateNeeds(selectedPony.needs);
        }
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
        if (HUDController.GetInstance().IsMouseOverGui() || !Physics.Raycast(ray, out hit, 1000, interactionLayer)) {
            cursorController.SetCursor(CursorType.Normal);
            return;
        }
        TerrainTile terrainTile = hit.transform.GetComponent<TerrainTile>();
        if (terrainTile != null) {
            ICollection<PonyAction> actions = terrainTile.GetActions(selectedPony);
            cursorController.SetCursor(actions.Count > 0 ? CursorType.GoHere : CursorType.Unavailable);
            HandleHover(actions);
            return;
        }
        Transform hitParent = hit.transform.parent;
        if (hitParent != null) {
            PropertyObject propertyObject = hit.transform.GetComponentInParent<PropertyObject>();
            if (propertyObject != null) {
                ICollection<PonyAction> actions = propertyObject.GetActions(selectedPony);
                cursorController.SetCursor(actions.Count > 0 ? CursorType.Interact : CursorType.Unavailable);
                HandleHover(actions);
                return;
            }
            Pony pony = hit.transform.parent.GetComponent<Pony>();
            if (pony != null) {
                ICollection<PonyAction> actions = pony.GetActions(selectedPony);
                cursorController.SetCursor(actions.Count > 0 ? CursorType.InteractPony : CursorType.Unavailable);
                HandleHover(actions);
                return;
            }
        }
        cursorController.SetCursor(CursorType.Normal);
    }

    private void HandleHover(ICollection<PonyAction> actions) {
        if (Input.GetMouseButtonUp(0)) {
            if (pieMenu != null) {
                Destroy(pieMenu.gameObject);
            }
            if (actions.Count == 0) {
                SoundController.Instance.PlaySound(SoundType.Deny);
                return;
            }
            pieMenu = Instantiate(pieMenuPrefab, transform.parent);
            RectTransform pieTransform = pieMenu.GetComponent<RectTransform>();
            Vector2 position = UiScaler.ScalePointToCanvas(Input.mousePosition);
            Vector2 size = pieTransform.sizeDelta;
            
            // Make sure the pie menu fits inside the screen
            Vector2 canvasSize = UiScaler.CanvasSize;
            position.x = Mathf.Clamp(position.x, size.x / 2, canvasSize.x - size.x / 2);
            position.y = Mathf.Clamp(position.y, size.y / 2, canvasSize.y - size.y / 2);
            
            pieTransform.anchoredPosition = position;
            pieMenu.Init(selectedPony, actions);
        }
    }
}

}