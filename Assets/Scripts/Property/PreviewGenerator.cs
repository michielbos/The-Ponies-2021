using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for generating preview images for furniture. 
/// </summary>
public class PreviewGenerator : MonoBehaviour {
    public GameObject previewObject;

    /// <summary>
    /// Create a preview for the given furniture preset.
    /// </summary>
    /// <param name="furniturePreset">The preset to create a preview from.</param>
    /// <returns>A RenderTexture that contains the generated preview.</returns>
    public RenderTexture CreatePreview (FurniturePreset furniturePreset) {
        Camera camera = GetComponent<Camera>();
        furniturePreset.ApplyToGameObject(previewObject);
        RenderTexture renderTexture = new RenderTexture(180, 180, 24, RenderTextureFormat.ARGB32);
        camera.enabled = true;
        camera.targetTexture = renderTexture;
        camera.Render();
        camera.enabled = false;
        camera.targetTexture = null;
        return renderTexture;
    }
}
