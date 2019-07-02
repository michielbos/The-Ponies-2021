using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

/// <summary>
/// Simple temporary class for scaling down the UI on smaller resolutions.
/// In the future, we might want to replace this with a better system, with for example folding.
/// </summary>
public class UiScaler : MonoBehaviour {
    public CanvasScaler canvasScaler;
    private int lastWidth;

    private void Start() {
        UpdateSize();
    }

    private void Update() {
        if (lastWidth != Screen.width) {
            UpdateSize();
        }
    }

    private void UpdateSize() {
        lastWidth = Screen.width;
        if (lastWidth <= 1024) {
            canvasScaler.scaleFactor = 0.6f;
        } else if (lastWidth <= 1280) {
            canvasScaler.scaleFactor = 0.7f;
        } else if (lastWidth <= 1600) {
            canvasScaler.scaleFactor = 0.8f;
        } else {
            canvasScaler.scaleFactor = 1f;
        }
    }
}

}