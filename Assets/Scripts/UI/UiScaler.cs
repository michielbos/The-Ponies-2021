using Assets.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

/// <summary>
/// Simple temporary class for scaling down the UI on smaller resolutions.
/// In the future, we might want to replace this with a better system, with for example folding.
/// </summary>
public class UiScaler : SingletonMonoBehaviour<UiScaler> {
    public RectTransform canvasTransform;
    public CanvasScaler canvasScaler;
    private int lastWidth;
    private static float uiScale = 1f;

    /// <summary>
    /// The size of the canvas, which may be larger than the screen size when scaling is applied.
    /// </summary>
    public static Vector2 CanvasSize {
        get {
            Rect pixelRect = Instance.canvasTransform.rect;
            return new Vector2(pixelRect.width, pixelRect.height);
        }
    } 

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
            uiScale = 0.6f;
        } else if (lastWidth <= 1280) {
            uiScale = 0.7f;
        } else if (lastWidth <= 1600) {
            uiScale = 0.8f;
        } else if (lastWidth <= 1920) {
            uiScale = 0.85f;
        } else {
            uiScale = 1f;
        }
        canvasScaler.scaleFactor = uiScale;
    }

    /// <summary>
    /// Scale a point on the screen, to match the canvas scale.
    /// This is useful for converting mouse positions to canvas positions.
    /// </summary>
    public static Vector2 ScalePointToCanvas(Vector2 point) {
        return point / uiScale;
    }
}

}