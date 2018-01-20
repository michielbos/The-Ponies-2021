using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller class for the catalog part of buy and build mode.
/// </summary>
public class CatalogController : MonoBehaviour {
	public AudioClip clickSound;
	public AudioClip whooshSound;
	public Button buttonPrefab;
	public RectTransform catalogBar;
	public Button catalogPreviousButton;
	public Button catalogNextButton;
	public BuyController buyController;
	public ObjectInfoPanel objectInfoPanel;
	public AudioSource audioSource;
	private ObjectCategory category = ObjectCategory.None;
	public int tab;
	private List<Button> catalogItemButtons;
	private Button pressedButton;

	public CatalogController () {
		catalogItemButtons = new List<Button>();
	}

	private void Update () {
		if (pressedButton != null) {
			RectTransform buttonTransform = pressedButton.GetComponent<RectTransform>();
			Vector3 buttonPos = buttonTransform.position;
			Vector3 buttonSize = buttonTransform.sizeDelta;
			Vector3 mousePos = Input.mousePosition;
			if (mousePos.x < buttonPos.x || mousePos.x > buttonPos.x + buttonSize.x ||
			    mousePos.y > buttonPos.y || mousePos.y < buttonPos.y - buttonSize.y) {
				objectInfoPanel.SetVisible(false);
				pressedButton = null;
				audioSource.PlayOneShot(whooshSound);
			}
		}
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
		if (this.category == category) {
			CloseCatalog();
		} else {
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
	public void CloseCatalog () {
		category = ObjectCategory.None;
		catalogBar.gameObject.SetActive(false);
		catalogBar.localScale = new Vector3(0, 0, 0);
	}

	private void EmptyCatalog () {
		foreach (Button button in catalogItemButtons) {
			Destroy(button.gameObject);
		}
		catalogItemButtons.Clear();
	}

	private void UpdateCatalogButtons () {
		EmptyCatalog();
		List<FurniturePreset> furniturePresets = FurniturePresets.Instance.GetFurniturePresets(category);
		int barMargin = 35;
		int buttonMargin = 5;
		Vector2 buttonSize = buttonPrefab.GetComponent<RectTransform>().sizeDelta;
		float buttonAreaWidth = catalogBar.sizeDelta.x - barMargin * 2;
		float spareWidth = buttonAreaWidth % (buttonSize.x + buttonMargin);
		int maxHorizontal = Mathf.FloorToInt(buttonAreaWidth / (buttonSize.x + buttonMargin));
		int maxButtons = maxHorizontal * 2;
		UpdatePreviousAndNextButtons(furniturePresets.Count, maxButtons);
		for (int i = tab * maxButtons; i < (tab + 1) * maxButtons && i < furniturePresets.Count; i++) {
			FurniturePreset preset = furniturePresets[i];
			Button button = Instantiate(buttonPrefab, transform);
			RectTransform rectTransform = button.GetComponent<RectTransform>();
			Vector3 pos = rectTransform.anchoredPosition;
			pos.x = barMargin + i % maxHorizontal * (buttonSize.x + buttonMargin) + spareWidth / 2;
			if (i >= tab * maxButtons + maxHorizontal)
				pos.y -= buttonSize.y + buttonMargin;
			rectTransform.anchoredPosition = pos;
			button.onClick.AddListener(delegate { OnCatalogItemClicked(button, preset); });
			catalogItemButtons.Add(button);
		}
	}

	private void UpdatePreviousAndNextButtons (int numberOfButtons, int buttonsPerTab) {
		catalogPreviousButton.transform.localScale = tab <= 0 ? new Vector3(0, 0, 0) : new Vector3(1, 1, 1);
		int numberOfTabs = Mathf.CeilToInt((float)numberOfButtons / buttonsPerTab);
		catalogNextButton.transform.localScale = tab >= numberOfTabs - 1 ? new Vector3(0, 0, 0) : new Vector3(1, 1, 1);
	}

	private void OnCatalogItemClicked (Button button, FurniturePreset preset) {
		buyController.SetPlacingPreset(preset);
		objectInfoPanel.DisplayItem(preset);
		objectInfoPanel.SetVisible(true);
		audioSource.PlayOneShot(clickSound);
		audioSource.PlayOneShot(whooshSound);
		pressedButton = button;
	}

	/// <summary>
	/// When the previous button was pressed on the catalog tab.
	/// </summary>
	public void CatalogPreviousButton () {
		tab--;
		UpdateCatalogButtons();
	}
	
	/// <summary>
	/// When the next button was pressed on the catalog tab.
	/// </summary>
	public void CatalogNextButton () {
		tab++;
		UpdateCatalogButtons();
	}
}
