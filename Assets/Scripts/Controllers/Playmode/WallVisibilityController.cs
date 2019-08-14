using Assets.Scripts.Util;
using Controllers.Singletons;
using Model.Property;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.Playmode {

public class WallVisibilityController : SingletonMonoBehaviour<WallVisibilityController> {
    public WallVisibility wallVisibility = WallVisibility.Partially;
    public Sprite[] wallVisibilityIcons;
    
    public Button wallVisibilityButton;
    
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
        foreach(Wall wall in PropertyController.Instance.property.walls.Values) {
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