using System.Collections.Generic;
using Controllers.Playmode;
using Model.Ponies;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

[RequireComponent(typeof(RectTransform))]
public class PlayableCharactersView : MonoBehaviour {
    private const int PortraitsMargin = 10;
    private const int SpaceBetweenPortraits = 35;
    private const int MinPaneWidth = 180;

    public Button portraitPrefab;
    public RectTransform charactersButtonsPane;
    public RectTransform statsPane;
    public GameObject selectedCharacterPortrait;

    private GameObject[] portraits = new GameObject[0];

    private void Start() {
        UpdateHousehold();
    }

    public void UpdateHousehold() {
        List<Pony> ponies = PropertyController.Instance.property.household.ponies;
        UpdatePortraits(ponies);
        UpdatePaneSize();
    }

    private void UpdatePortraits(List<Pony> ponies) {
        foreach (GameObject portrait in portraits) {
            Destroy(portrait);
        }

        int numberOfPortraits = ponies.Count;
        if (HouseholdController.Instance.selectedPony != null) {
            numberOfPortraits--;
        }

        portraits = new GameObject[numberOfPortraits];
        int index = 0;
        foreach (Pony pony in ponies) {
            if (pony.IsSelected)
                continue;
            Button portrait = Instantiate(portraitPrefab, charactersButtonsPane);
            portrait.GetComponent<RectTransform>().anchoredPosition = new Vector2(index * SpaceBetweenPortraits, 0);
            portrait.onClick.AddListener(() => HouseholdController.Instance.SetSelectedPony(pony));
            portraits[index] = portrait.gameObject;
            index++;
        }
    }

    private void UpdatePaneSize() {
        RectTransform charactersPane = GetComponent<RectTransform>();
        float buttonWidth = portraitPrefab.GetComponent<RectTransform>().sizeDelta.x;
        float totalButtonWidth = (portraits.Length - 1) * SpaceBetweenPortraits + buttonWidth;
        charactersButtonsPane.sizeDelta = new Vector2(totalButtonWidth, charactersButtonsPane.sizeDelta.y);
        float paneWidth = Mathf.Max(totalButtonWidth + PortraitsMargin * 2, MinPaneWidth);
        charactersPane.sizeDelta = new Vector2(paneWidth, charactersPane.sizeDelta.y);
        statsPane.anchoredPosition =
            new Vector2(charactersPane.anchoredPosition.x + paneWidth, statsPane.anchoredPosition.y);
    }
}

}