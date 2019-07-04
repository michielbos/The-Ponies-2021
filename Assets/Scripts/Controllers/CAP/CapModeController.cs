using Assets.Scripts.Util;
using UI;

namespace Controllers.CAP {

public class CapModeController : SingletonMonoBehaviour<CapModeController> {
    public ModeButton bodyModeButton;
    public ModeButton headModeButton;
    public ModeButton eyesModeButton;
    public ModeButton maneModeButton;
    public ModeButton tailModeButton;
    public ModeButton outfitModeButton;
    public ModeButton accessoryModeButton;
    public ModeButton personalityModeButton;

    public CapHudPanel CurrentMode { get; private set; } = CapHudPanel.None;

    public void SwitchMode(CapHudPanel mode) {
        if (mode == CurrentMode)
            mode = CapHudPanel.None;
        CurrentMode = mode;

        bodyModeButton.SetModeActive(mode == CapHudPanel.Body);
        headModeButton.SetModeActive(mode == CapHudPanel.Head);
        eyesModeButton.SetModeActive(mode == CapHudPanel.Eyes);
        maneModeButton.SetModeActive(mode == CapHudPanel.Mane);
        tailModeButton.SetModeActive(mode == CapHudPanel.Tail);
        outfitModeButton.SetModeActive(mode == CapHudPanel.Outfit);
        accessoryModeButton.SetModeActive(mode == CapHudPanel.Accessory);
        personalityModeButton.SetModeActive(mode == CapHudPanel.Personality);

        CapHudController.Instance.OnModeUpdate(mode);
    }
}

}