using UnityEngine;
using Util;

/// <summary>
/// Class for generating preview images for furniture. 
/// </summary>
public class PreviewGenerator : MonoBehaviour {
    private int previewLayer;
    public ModelContainer previewObject;

    private void Awake() {
        previewLayer = LayerMask.NameToLayer("Preview");
    }

    /// <summary>
    /// Create a preview for the given furniture preset.
    /// </summary>
    /// <param name="furniturePreset">The preset to create a preview from.</param>
    /// <param name="skin">The skin to create a preview for.</param>
    /// <returns>A RenderTexture that contains the generated preview.</returns>
    public RenderTexture CreatePreview (FurniturePreset furniturePreset, int skin) {
        Camera camera = GetComponent<Camera>();
        furniturePreset.ApplyToModel(previewObject, skin);
        previewObject.Model.layer = previewLayer;
        foreach (Transform child in previewObject.Model.GetComponentsInChildren<Transform>()) {
            child.gameObject.layer = previewLayer;
        }
        RenderTexture renderTexture = new RenderTexture(180, 180, 24, RenderTextureFormat.ARGB32);
        camera.enabled = true;
        camera.targetTexture = renderTexture;
        camera.Render();
        camera.enabled = false;
        camera.targetTexture = null;
        return renderTexture;
    }
}
