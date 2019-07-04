using Assets.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.Playmode {

public class CAPHUDController : SingletonMonoBehaviour<CAPHUDController> {
    public Image cpanel;

    public Sprite noModeCpanel;
    public Sprite bodyModeCpanel;
    public Sprite headModeCpanel;
    public Sprite eyesModeCpanel;
    public Sprite maneModeCpanel;
    public Sprite tailModeCpanel;
    public Sprite outfitModeCpanel;
    public Sprite accessoryModeCpanel;
    public Sprite personalityModeCpanel;

    // Called from Unity GUI Button
    public void ActivatePanel(int index) {
        ActivatePanel((CapHudPanel) index);
    }

    private static void ActivatePanel(CapHudPanel panel) {
        CAPModeController.GetInstance().SwitchMode(panel);
    }

    public void OnModeUpdate(CapHudPanel mode) {
        if (mode == CapHudPanel.Body) {
            cpanel.sprite = bodyModeCpanel;
        } else if (mode == CapHudPanel.Head) {
            cpanel.sprite = headModeCpanel;
        } else if (mode == CapHudPanel.Eyes) {
            cpanel.sprite = eyesModeCpanel;
        } else if (mode == CapHudPanel.Mane) {
            cpanel.sprite = maneModeCpanel;
        } else if (mode == CapHudPanel.Tail) {
            cpanel.sprite = tailModeCpanel;
        } else if (mode == CapHudPanel.Outfit) {
            cpanel.sprite = outfitModeCpanel;
        } else if (mode == CapHudPanel.Accessory) {
            cpanel.sprite = accessoryModeCpanel;
        } else if (mode == CapHudPanel.Personality) {
            cpanel.sprite = personalityModeCpanel;
        } else {
            cpanel.sprite = noModeCpanel;
        }
    }
}

public enum CapHudPanel {
    None = -1,
    Body = 0,
    Head = 1,
    Eyes = 2,
    Mane = 3,
    Tail = 4,
    Outfit = 5,
    Accessory = 6,
    Personality = 7,
}

}