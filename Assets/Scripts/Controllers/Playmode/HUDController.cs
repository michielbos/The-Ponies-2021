using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Util;
using Controllers.Singletons;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Controllers.Playmode {

public class HUDController : SingletonMonoBehaviour<HUDController>, IPointerEnterHandler, IPointerExitHandler {
    public List<GameObject> speedButtons;

    public Text fundsText;
    public Text timeText;
    private bool touchingGui;

    void Start() {
        UpdateSpeed();
        UpdateFunds();
    }

    // Called from Unity GUI Button
    public void ActivatePanel(int index) {
        ActivatePanel((HudPanel) index);
    }

    private static void ActivatePanel(HudPanel panel) {
        SoundController.Instance.PlaySound(SoundType.Click);
        SoundController.Instance.PlaySound(SoundType.Woosh);
        ModeController.GetInstance().SwitchMode(panel);
    }

    // Called from Unity GUI Button
    public void SetSpeed(int index) {
        TimeController.Instance.SetSpeed(index);
    }

    // Called from Unity GUI Button
    public void Zoom(int direction) {
        SoundController.Instance.PlaySound(SoundType.Click);
        CameraController.Instance.Zoom(direction);
    }

    // Called from Unity GUI Button
    public void Rotate(bool counterClockwise) {
        SoundController.Instance.PlaySound(SoundType.Click);
        CameraController.Instance.Rotate(counterClockwise);
    }

    // Called from Unity GUI Button
    public void SetClocks() {
        if (TimeController.Instance.twelveHourClock) {
            TimeController.Instance.twelveHourClock = false;
        } else {
            TimeController.Instance.twelveHourClock = true;
        }

        UpdateTime();
    }

    public void UpdateSpeed() {
        speedButtons[0].GetComponent<Image>().overrideSprite = speedButtons[0].GetComponent<Image>().sprite;
        foreach (GameObject g in speedButtons) {
            bool active = speedButtons.IndexOf(g) == TimeController.Instance.GetCurrentSpeed().GetIndex();
            g.GetComponent<Button>().interactable = !active;
        }
    }

    public void UpdateFunds() {
        if (MoneyController.Instance.UseFunds) {
            fundsText.text = MoneyController.Instance.Funds.ToString();
        } else {
            fundsText.text = "";
        }
    }

    public void UpdateTime() {
        timeText.text = TimeController.Instance.GetTimeText();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        touchingGui = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        touchingGui = false;
    }

    public bool IsMouseOverGui() {
        //I doubt how reliable this is, but it's not like we have anything better at the moment.
        return touchingGui;
    }
}

public enum HudPanel {
    None = -1,
    Live = 0,
    Buy = 1,
    Build = 2,
    Camera = 3,
    Options = 4
}

}