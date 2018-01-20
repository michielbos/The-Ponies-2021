using System.Collections;
using System.Collections.Generic;
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
    public Image previewImage;

    /// <summary>
    /// Update the panel with the information of the given item.
    /// </summary>
    /// <param name="furniturePreset">The FurniturePreset to display.</param>
    public void DisplayItem (FurniturePreset furniturePreset) {
        titleText.text = furniturePreset.name;
        descriptionText.text = furniturePreset.description;
        priceText.text = "$" + furniturePreset.price;
        notesText.text = "(no notes yet)";
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
