using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the object information panel that appears when selecting an item in the catalog.
/// </summary>
public class ObjectInfoPanel : MonoBehaviour {
    public Button buttonPrefab;
    public Text titleText;
    public Text descriptionText;
    public Text priceText;
    public RectTransform notesPanel;
    public RawImage previewImage;
    private CatalogItem catalogItem;
    private Button[] skinButtons;

    /// <summary>
    /// Update the panel with the information of the given item.
    /// </summary>
    /// <param name="catalogItem">The FurniturePreset to display.</param>
    public void DisplayItem (CatalogItem catalogItem) {
        this.catalogItem = catalogItem;
        titleText.text = catalogItem.name;
        descriptionText.text = catalogItem.description;
        priceText.text = "$" + catalogItem.price;
        UpdateNotes();
        PlaceSkinButtons();
        SetSelectedSkin(0);
    }

    private void UpdateNotes () {
        Text notesText = notesPanel.GetComponentInChildren<Text>();
        notesText.text = GetNotesText();
        int notesHeight = 0;
        foreach (char c in notesText.text) {
            if (c == '\n') {
                notesHeight += notesText.fontSize + 3;
            }
        }
        if (notesText.text.Length > 0) {
            notesHeight += notesText.fontSize + 13;
        }
        notesPanel.sizeDelta = new Vector2(notesPanel.sizeDelta.x, notesHeight);
    }

    private string GetNotesText () {
        string text = "";
        text = AddText(text, catalogItem.needStats.GetDisplayText());
        text = AddText(text, catalogItem.skillStats.GetDisplayText());
        if (catalogItem.requiredAge != RequiredAge.Any) {
            text = AddText(text, catalogItem.requiredAge + "s only");
        }
        return text;
    }

    private string AddText (string text, string add) {
        if (add.Length <= 0)
            return text;
        if (text.Length > 0)
            return text + "\n" + add;
        return add;
    }

    /// <summary>
    /// Set the skin that is selected on the panel.
    /// This determines which preview texture is displayed.
    /// </summary>
    /// <param name="skin">The skin to select.</param>
    public void SetSelectedSkin (int skin) {
        previewImage.texture = catalogItem.GetPreviewTextures()[skin];
        CatalogController.Instance.SelectCatalogItem(catalogItem, skin);
    }
    
    private void PlaceSkinButtons () {
        ClearButtons();
        Texture[] previewTextures = catalogItem.GetPreviewTextures();
        if (previewTextures.Length <= 1)
            return;
        skinButtons = new Button[previewTextures.Length];
        for (var i = 0; i < previewTextures.Length; i++) {
            Button button = Instantiate(buttonPrefab, transform);
            RectTransform descTransform = descriptionText.GetComponent<RectTransform>();
            RectTransform buttonTransform = button.GetComponent<RectTransform>();
            Vector2 position = new Vector2(
                descTransform.anchoredPosition.x + i * (buttonTransform.sizeDelta.x + 5),
                descTransform.anchoredPosition.y - 5);
            buttonTransform.anchoredPosition = position;
            button.GetComponentInChildren<RawImage>().texture = previewTextures[i];
            int skin = i;
            button.onClick.AddListener(delegate { SetSelectedSkin(skin); });
            skinButtons[i] = button;
        }
    }
    
    private void ClearButtons () {
        if (skinButtons == null)
            return;
        foreach (Button button in skinButtons) {
            Destroy(button.gameObject);
        }
        skinButtons = null;
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
