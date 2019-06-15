using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Controllers {

public class ModeController : SingletonMonoBehaviour<ModeController> {
    public GuiButtonController LiveModeGuiButtonController;
    public GuiButtonController BuyModeGuiButtonController;
    public GuiButtonController BuildModeGuiButtonController;
    public GuiButtonController CameraModeGuiButtonController;
    public GuiButtonController OptionsModeGuiButtonController;

    private HudPanel currentMode;

    public void SwitchMode(HudPanel mode) {
        if (mode == currentMode)
            mode = HudPanel.None;
        currentMode = mode;
        CatalogController.GetInstance().CloseCatalog();

        // Quit any tools that may be open from the last panel
        ToolController.Instance.SetTool(ToolType.None);

        if (mode == HudPanel.Buy || mode == HudPanel.Build) {
            // Note that we intentionally set the tool to None first.
            // This is to ensure the buy tool is always disabling when switching from build to buy. 
            ToolController.Instance.SetTool(ToolType.Buy);
        }

        LiveModeGuiButtonController.enabled = mode == HudPanel.Live;
        BuyModeGuiButtonController.enabled = mode == HudPanel.Buy;
        BuildModeGuiButtonController.enabled = mode == HudPanel.Build;
        CameraModeGuiButtonController.enabled = mode == HudPanel.Camera;
        OptionsModeGuiButtonController.enabled = mode == HudPanel.Options;

        switch (mode) {
            case HudPanel.None:
                MusicController.Instance.SwitchMusic(MusicType.NoMusic);
                break;
            case HudPanel.Live:
                MusicController.Instance.SwitchMusic(MusicType.NoMusic);
                break;
            case HudPanel.Buy:
                MusicController.Instance.SwitchMusic(MusicType.BuyMode);
                break;
            case HudPanel.Build:
                MusicController.Instance.SwitchMusic(MusicType.BuildMode);
                break;
            case HudPanel.Camera:
                MusicController.Instance.SwitchMusic(MusicType.NoMusic);
                break;
            case HudPanel.Options:
                MusicController.Instance.SwitchMusic(MusicType.NoMusic);
                break;
        }

        // Only pause for Buy Mode and up, excluding Live mode and whenever no mode is selected.
        // No pannel = -1
        //      Live = 0
        SpeedController.Instance.ForcePause(mode > 0);
        HUDController.GetInstance().UpdateSpeed();
    }

    public void LockLiveMode(bool locked) {
        if (locked && currentMode == HudPanel.Live) {
            currentMode = HudPanel.None;
        }
        LiveModeGuiButtonController.Locked = locked;
    }
}

}