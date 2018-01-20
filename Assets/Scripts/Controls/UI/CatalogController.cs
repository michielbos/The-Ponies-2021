using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller class for the catalog part of buy and build mode.
/// </summary>
public class CatalogController : MonoBehaviour {
	public RectTransform catalogBar;
	public Button catalogPreviousButton;
	public Button catalogNextButton;
	public BuyController buyController;
	public Button buttonPrefab;
	private ObjectCategory catalogCategory = ObjectCategory.None;
	private List<Button> catalogItemButtons;

	public CatalogController () {
		catalogItemButtons = new List<Button>();
	}

	/// <summary>
	/// Open the furniture catalog with the given category.
	/// Used by the UI event system.
	/// </summary>
	/// <param name="category">The id of the category.</param>
	public void OpenCatalog (int category) {
		OpenCatalog((ObjectCategory) category);
	}

	/// <summary>
	/// Open the furniture catalog with the given category.
	/// </summary>
	/// <param name="category">The category to open.</param>
	public void OpenCatalog (ObjectCategory category) {
		if (catalogCategory == category) {
			CloseCatalog();
		} else {
			catalogCategory = category;
			catalogBar.gameObject.SetActive(true);
			catalogBar.localScale = new Vector3(1, 1, 1);
			CreateCatalogItems();
		}
	}

	/// <summary>
	/// Close the catalog tab.
	/// </summary>
	public void CloseCatalog () {
		catalogCategory = ObjectCategory.None;
		catalogBar.gameObject.SetActive(false);
		catalogBar.localScale = new Vector3(0, 0, 0);
	}

	private void EmptyCatalog () {
		foreach (Button button in catalogItemButtons) {
			Destroy(button.gameObject);
		}
		catalogItemButtons.Clear();
	}

	private void CreateCatalogItems () {
		EmptyCatalog();
		List<FurniturePreset> furniturePresets = FurniturePresets.Instance.GetFurniturePresets(catalogCategory);
		int barMargin = 35;
		int buttonMargin = 5;
		Vector2 buttonSize = buttonPrefab.GetComponent<RectTransform>().sizeDelta;
		float buttonAreaWidth = catalogBar.sizeDelta.x - barMargin * 2;
		float spareWidth = buttonAreaWidth % (buttonSize.x + buttonMargin);
		int maxHorizontal = Mathf.FloorToInt(buttonAreaWidth / (buttonSize.x + buttonMargin));
		for (int i = 0; i < maxHorizontal * 2 && i < furniturePresets.Count; i++) {
			FurniturePreset preset = furniturePresets[i];
			Button button = Instantiate(buttonPrefab, transform);
			RectTransform rectTransform = button.GetComponent<RectTransform>();
			Vector3 pos = rectTransform.anchoredPosition;
			pos.x = barMargin + i % maxHorizontal * (buttonSize.x + buttonMargin) + spareWidth / 2;
			if (i >= maxHorizontal)
				pos.y -= buttonSize.y + buttonMargin;
			rectTransform.anchoredPosition = pos;
			button.onClick.AddListener(delegate { buyController.SetPlacingPreset(preset); });
			catalogItemButtons.Add(button);
		}
	}

	/// <summary>
	/// When the previous button was pressed on the catalog tab.
	/// </summary>
	public void CatalogPreviousButton () {
		
	}
	
	/// <summary>
	/// When the next button was pressed on the catalog tab.
	/// </summary>
	public void CatalogNextButton () {
		
	}
}
