using Model.Ponies;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

public class NeedsPanel : MonoBehaviour {
    private const float BarLength = 131;
    
    public Image hungerFill;
    public Image energyFill;
    public Image comfortFill;
    public Image funFill;
    public Image hygieneFill;
    public Image socialFill;
    public Image bladderFill;
    public Image roomFill;

    public Color positiveColor;
    public Color neutralColor;
    public Color negativeColor;

    public void UpdateNeeds(Needs needs) {
        UpdateNeed(hungerFill, needs.hunger);
        UpdateNeed(energyFill, needs.energy);
        UpdateNeed(comfortFill, needs.comfort);
        UpdateNeed(funFill, needs.fun);
        UpdateNeed(hygieneFill, needs.hygiene);
        UpdateNeed(socialFill, needs.social);
        UpdateNeed(bladderFill, needs.bladder);
        UpdateNeed(roomFill, needs.room);
    }

    private void UpdateNeed(Image fill, float needValue) {
        RectTransform rectTransform = fill.rectTransform;
        rectTransform.sizeDelta = new Vector2(needValue * BarLength, rectTransform.sizeDelta.y);
        if (needValue >= 0.67f) {
            fill.color = positiveColor;
        } else if (needValue >= 0.33) {
            fill.color = neutralColor;
        } else {
            fill.color = negativeColor;
        }
    }
}

}