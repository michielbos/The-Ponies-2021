using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Model.Actions {

public class ActionQueue : MonoBehaviour {
    private const int WidthBetweenButtons = 5;
    public Button queueButtonPrefab;
    public Color itemColor;
    public Color activeItemColor;
    public Color canceledItemColor;
    private List<Button> queueButtons = new List<Button>();

    public void UpdateActions(List<PonyAction> actions) {
        foreach (Button button in queueButtons) {
            Destroy(button.gameObject);
        }
        queueButtons.Clear();
        int number = 0;
        foreach (PonyAction action in actions) {
            Button button = Instantiate(queueButtonPrefab, transform);
            button.image.color = GetActionButtonColor(action);
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(number * (rectTransform.sizeDelta.x + WidthBetweenButtons), 0);
            button.onClick.AddListener(() => { action.pony.CancelAction(action); });
            queueButtons.Add(button);
            number++;
        }
    }

    private Color GetActionButtonColor(PonyAction action) {
        if (action.canceled)
            return canceledItemColor;
        if (action.active)
            return activeItemColor;
        return itemColor;
    }
}

}