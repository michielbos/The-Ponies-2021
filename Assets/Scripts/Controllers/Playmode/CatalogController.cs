﻿using System.Collections.Generic;
using Assets.Scripts.Util;
using Controllers.Singletons;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.Playmode {

/// <summary>
/// Controller class for the catalog part of buy and build mode.
/// </summary>
public class CatalogController : SingletonMonoBehaviour<CatalogController> {
    public Button buttonPrefab;
    public RectTransform catalogBar;
    public Button catalogPreviousButton;
    public Button catalogNextButton;
    public ToolController toolController;
    public ObjectInfoPanel objectInfoPanel;
    private ObjectCategory category = ObjectCategory.None;
    private int tab;
    private List<Button> catalogItemButtons = new List<Button>();
    private Button pressedButton;

    private void Update() {
        if (pressedButton != null && ShouldCloseInfoPanel()) {
            objectInfoPanel.SetVisible(false);
            pressedButton = null;
            SoundController.Instance.PlaySound(SoundType.Woosh);
        }
    }

    private bool ShouldCloseInfoPanel() {
        RectTransform catalogTransform = catalogBar;
        RectTransform objectInfoTransform = objectInfoPanel.GetComponent<RectTransform>();
        Vector2 catalogPos = catalogTransform.position;
        Vector2 objectInfoPos = objectInfoTransform.position;
        Vector2 objectInfoSize = objectInfoTransform.sizeDelta;
        Vector3 mousePos = Input.mousePosition;

        float left = Mathf.Min(catalogPos.x, objectInfoPos.x);
        float top = objectInfoPos.y + objectInfoSize.y;
        return mousePos.x < left || mousePos.y > top;
    }

    private void OnApplicationFocus(bool hasFocus) {
        if (hasFocus) {
            //Render textures are wiped when we lose focus. This will reload them.
            UpdateCatalogButtons();
        }
    }

    /// <summary>
    /// Open the furniture catalog with the given category.
    /// Used by the UI event system.
    /// </summary>
    /// <param name="category">The id of the category.</param>
    public void OpenCatalog(int category) {
        OpenCatalog((ObjectCategory) category);
    }

    /// <summary>
    /// Open the furniture catalog with the given category.
    /// </summary>
    /// <param name="category">The category to open.</param>
    public void OpenCatalog(ObjectCategory category) {
        if (this.category == category) {
            CloseCatalog();
        } else {
            toolController.SetToolForCategory(category);
            this.category = category;
            tab = 0;
            catalogBar.gameObject.SetActive(true);
            catalogBar.localScale = new Vector3(1, 1, 1);
            UpdateCatalogButtons();
        }
    }

    /// <summary>
    /// Close the catalog tab.
    /// </summary>
    public void CloseCatalog() {
        category = ObjectCategory.None;
        catalogBar.gameObject.SetActive(false);
        catalogBar.localScale = new Vector3(0, 0, 0);
    }

    private void EmptyCatalog() {
        foreach (Button button in catalogItemButtons) {
            Destroy(button.gameObject);
        }

        catalogItemButtons.Clear();
    }

    private void UpdateCatalogButtons() {
        EmptyCatalog();
        List<CatalogItem> catalogItems = CatalogItemProvider.GetCatalogItems(category);
        if (CheatsController.Instance.showHiddenCatalog && category == ObjectCategory.Misc) {
            catalogItems.AddRange(CatalogItemProvider.GetHiddenItems());
        }
        int barMargin = 35;
        int buttonMargin = 5;
        Vector2 buttonSize = buttonPrefab.GetComponent<RectTransform>().sizeDelta;
        float buttonAreaWidth = catalogBar.rect.width - barMargin * 2;
        float spareWidth = buttonAreaWidth % (buttonSize.x + buttonMargin);
        int maxHorizontal = Mathf.FloorToInt(buttonAreaWidth / (buttonSize.x + buttonMargin));
        int maxButtons = maxHorizontal * 2;
        UpdatePreviousAndNextButtons(catalogItems.Count, maxButtons);
        for (int i = tab * maxButtons; i < (tab + 1) * maxButtons && i < catalogItems.Count; i++) {
            CatalogItem catalogItem = catalogItems[i];
            Button button = Instantiate(buttonPrefab, catalogBar);
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            Vector3 pos = new Vector3(barMargin + i % maxHorizontal * (buttonSize.x + buttonMargin) + spareWidth / 2, 0);
            if (i >= tab * maxButtons + maxHorizontal) {
                pos.y -= buttonSize.y + buttonMargin;
            }

            rectTransform.anchoredPosition = pos;
            button.GetComponentInChildren<RawImage>().texture = catalogItem.GetPreviewTextures()[0];
            button.onClick.AddListener(delegate { OnCatalogItemClicked(button, catalogItem); });
            catalogItemButtons.Add(button);
        }
    }

    private void UpdatePreviousAndNextButtons(int numberOfButtons, int buttonsPerTab) {
        catalogPreviousButton.gameObject.SetActive(tab > 0);
        int numberOfTabs = Mathf.CeilToInt((float) numberOfButtons / buttonsPerTab);
        catalogNextButton.gameObject.SetActive(tab < numberOfTabs - 1);
    }

    private void OnCatalogItemClicked(Button button, CatalogItem catalogItem) {
        objectInfoPanel.DisplayItem(catalogItem);
        objectInfoPanel.SetVisible(true);
        SoundController.Instance.PlaySound(SoundType.Click);
        SoundController.Instance.PlaySound(SoundType.Woosh);
        pressedButton = button;
    }

    /// <summary>
    /// Select an item from the catalog. This will notify the relevant tool/controller.
    /// </summary>
    /// <param name="catalogItem">The CatalogItem that was selected</param>
    /// <param name="skin">The skin that was selected. If none was explicitly selected, this is 0.</param>
    public void SelectCatalogItem(CatalogItem catalogItem, int skin) {
        if (toolController.HasActiveTool) {
            toolController.ActiveTool.OnCatalogSelect(catalogItem, skin);
        }
    }

    /// <summary>
    /// When the previous button was pressed on the catalog tab.
    /// </summary>
    public void CatalogPreviousButton() {
        tab--;
        UpdateCatalogButtons();
    }

    /// <summary>
    /// When the next button was pressed on the catalog tab.
    /// </summary>
    public void CatalogNextButton() {
        tab++;
        UpdateCatalogButtons();
    }
}

}