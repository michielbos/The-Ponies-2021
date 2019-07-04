using Model.Ponies;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

public class NeedsPanel : MonoBehaviour {
    private const float BarLength = 131f;
    private const float MoodBarLength = 177f;
    // The distance from the center where the bar starts. This is the size of the icon, divided by two.
    private const float MoodBarOffset = 20f;
    // The minimum mood offset for the mood bar to display a change further than the yellow round in the center.
    private const float MinMoodOffset = MoodBarOffset / MoodBarLength;
    
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
    public Image moodFill;

    public void UpdateNeeds(Needs needs) {
        UpdateNeed(hungerFill, needs.hunger);
        UpdateNeed(energyFill, needs.energy);
        UpdateNeed(comfortFill, needs.comfort);
        UpdateNeed(funFill, needs.fun);
        UpdateNeed(hygieneFill, needs.hygiene);
        UpdateNeed(socialFill, needs.social);
        UpdateNeed(bladderFill, needs.bladder);
        UpdateNeed(roomFill, needs.room);
        UpdateMood(needs.GetMood());
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

    private void UpdateMood(float mood) {
        RectTransform rectTransform = moodFill.rectTransform;

        float fillSize;
        if (Mathf.Abs(0.5f - mood) < MinMoodOffset) {
            fillSize = MoodBarOffset * 2;
            rectTransform.anchoredPosition = new Vector2(0, -MoodBarOffset);
            rectTransform.pivot = new Vector2(0.5f, 0f);
        } else if (mood >= 0.5f) {
            fillSize = (mood - 0.5f) * MoodBarLength + MoodBarOffset;
            rectTransform.anchoredPosition = new Vector2(0, -MoodBarOffset);
            rectTransform.pivot = new Vector2(0.5f, 0f);
        } else {
            fillSize = (0.5f - mood) * MoodBarLength + MoodBarOffset;
            rectTransform.anchoredPosition = new Vector2(0, MoodBarOffset);
            rectTransform.pivot = new Vector2(0.5f, 1f);
        }
        
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, fillSize);
        
        if (mood >= 0.67f) {
            moodFill.color = positiveColor;
        } else if (mood >= 0.33) {
            moodFill.color = neutralColor;
        } else {
            moodFill.color = negativeColor;
        }
    }
}

}