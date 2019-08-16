using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;
using Controllers.Singletons;
using Model.Property;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.Playmode {

public class WallVisibilityController : SingletonMonoBehaviour<WallVisibilityController> {
    private int wallLayer;
    
    public WallVisibility wallVisibility = WallVisibility.Partially;
    public Sprite[] wallVisibilityIcons;

    public Button wallVisibilityButton;
    private Wall[] hoveredWalls = new Wall[0];

    private void Start() {
        wallLayer = LayerMask.GetMask("Walls");
    }

    private void Update() {
        if (wallVisibility == WallVisibility.Partially) {
            HandleWallHover();
        }
    }

    private void HandleWallHover() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = !HUDController.GetInstance().IsMouseOverGui()
            ? Physics.RaycastAll(ray, 1000, wallLayer)
            : new RaycastHit[0];
        Wall[] walls = hits.SelectMany(hit => hit.collider.GetComponent<Wall>()?.GetConnectedWalls(true) ?? new Wall[0])
            .ToArray();
        IEnumerable<Wall> addedWalls = walls.Except(hoveredWalls);
        IEnumerable<Wall> removedWalls = hoveredWalls.Except(walls);
        foreach (Wall addedWall in addedWalls) {
            addedWall.SetLowered(true);
        }
        foreach (Wall removedWall in removedWalls) {
            removedWall.UpdateVisibility();
        }
        hoveredWalls = walls;
    }

    // Called from Unity GUI Button
    public void ToggleWallVisibility() {
        SoundController.Instance.PlaySound(SoundType.Click);
        SetWallVisibility(wallVisibility < WallVisibility.Full ? wallVisibility + 1 : WallVisibility.Low);
    }

    public void SetWallVisibility(WallVisibility visibility) {
        wallVisibility = visibility;
        wallVisibilityButton.image.sprite = wallVisibilityIcons[(int) visibility];
        UpdateWallVisibility();
    }

    public void UpdateWallVisibility() {
        hoveredWalls = new Wall[0];
        foreach (Wall wall in PropertyController.Instance.property.walls.Values) {
            wall.UpdateVisibility(wallVisibility);
        }
    }
}

public enum WallVisibility {
    Low,
    Partially,
    Full
}

}