using Assets.Scripts.Controllers;
using Assets.Scripts.Util;
using Controllers.Singletons;
using UI;

namespace Controllers.Playmode {

public class ModeController : SingletonMonoBehaviour<ModeController> {
    public ModeButton liveModeButton;
    public ModeButton buyModeButton;
    public ModeButton buildModeButton;
    public ModeButton cameraModeButton;
    public ModeButton optionsModeButton;

    public HudPanel CurrentMode { get; private set; } = HudPanel.None;

    public void SwitchMode(HudPanel mode) {
        if (mode == CurrentMode)
            mode = HudPanel.None;
        CurrentMode = mode;
        CatalogController.GetInstance().CloseCatalog();

        // Quit any tools that may be open from the last panel
        ToolController.Instance.SetTool(ToolType.None);

        if (mode == HudPanel.Buy || mode == HudPanel.Build) {
            // Note that we intentionally set the tool to None first.
            // This is to ensure the buy tool is always disabling when switching from build to buy. 
            ToolController.Instance.SetTool(ToolType.Buy);
        }

        liveModeButton.SetModeActive(mode == HudPanel.Live);
        buyModeButton.SetModeActive(mode == HudPanel.Buy);
        buildModeButton.SetModeActive(mode == HudPanel.Build);
        cameraModeButton.SetModeActive(mode == HudPanel.Camera);
        optionsModeButton.SetModeActive(mode == HudPanel.Options);

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
        TimeController.Instance.ForcePause(mode > 0);
        HUDController.Instance.UpdateSpeed();
        HUDController.Instance.OnModeUpdate(mode);
    }

    public void LockLiveMode(bool locked) {
        if (locked && CurrentMode == HudPanel.Live) {
            CurrentMode = HudPanel.None;
        }
        liveModeButton.Locked = locked;
    }
}

}