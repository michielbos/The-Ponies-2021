using System.Collections.Generic;
using Controllers.Singletons;
using Model.Ponies;
using UnityEngine;
using UnityEngine.UI;

namespace Model.Actions {

public class PieMenu : MonoBehaviour {
    private const int ButtonDistance = 100;
    
    public Button buttonPrefab;
    private Pony pony;
    private List<PonyAction> actions;

    public void Init(Pony pony, List<PonyAction> actions) {
        this.pony = pony;
        this.actions = actions;
        int index = 0;
        foreach (PonyAction action in actions) {
            CreateActionButton(action, index++, actions.Count);
        }
        SoundController.Instance.PlaySound(SoundType.PieAppear);
    }

    private void CreateActionButton(PonyAction action, int index, int totalActions) {
        Button button = Instantiate(buttonPrefab, transform);
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = GetPositionInCircle(index, totalActions);
        Text text = button.GetComponentInChildren<Text>();
        text.text = action.name;
        button.onClick.AddListener(() => {
            SoundController.Instance.PlaySound(SoundType.PieSelect);
            pony.QueueAction(action);
            Destroy(gameObject);
        });
    }

    private Vector2 GetPositionInCircle(int index, int totalActions) {
        float radians = index * Mathf.PI * 2 / totalActions;
        return new Vector2(Mathf.Sin(radians), Mathf.Cos(radians)) * ButtonDistance;
    }
}

}