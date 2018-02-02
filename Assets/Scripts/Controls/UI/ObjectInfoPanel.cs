using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the object information panel that appears when selecting an item in the catalog.
/// </summary>
public class ObjectInfoPanel : MonoBehaviour {
    public Text titleText;
    public Text descriptionText;
    public Text priceText;
    public Text notesText;
    public RawImage previewImage;

    /// <summary>
    /// Update the panel with the information of the given item.
    /// </summary>
    /// <param name="catalogItem">The FurniturePreset to display.</param>
    public void DisplayItem (CatalogItem catalogItem) {
        titleText.text = catalogItem.name;
        descriptionText.text = catalogItem.description;
        priceText.text = "$" + catalogItem.price;
        notesText.text = "(no notes yet)";
        previewImage.texture = catalogItem.GetPreviewTexture();
    }

    /// <summary>
    /// Set whether the panel should be visible.
    /// </summary>
    /// <param name="visible">Whether the panel should be visible.</param>
    public void SetVisible (bool visible) {
        transform.localScale = visible ? new Vector3(1, 1, 1) : new Vector3(0, 0, 0);
        gameObject.SetActive(true);
    }
}
