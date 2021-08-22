using Controllers.Global;
using Controllers.Playmode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Playmode {

/// <summary>
/// Panel for displaying and manipulating the settings in the SettingsController.
/// </summary>
public class SettingsPanel : MonoBehaviour {
    public Toggle twelveHourClockToggle;
    public Toggle discordIntegrationToggle;
    public FreeWillToggleGroup freeWillSelectedPonyGroup;
    public FreeWillToggleGroup freeWillHousehold;

    /// <summary>
    /// Shortcut to the instance of the SettingsController.
    /// </summary>
    private SettingsController Settings => SettingsController.Instance;

    private void Start() {
        twelveHourClockToggle.onValueChanged.AddListener(value => {
            Settings.twelveHourClock.Value = value;
            HUDController.Instance.UpdateTime();
        });
        
        discordIntegrationToggle.onValueChanged.AddListener(value => {
            Settings.discordIntegration.Value = value;
            if (!value) {
                DiscordController.Instance.ClearActivity();
            }
        });
        
        freeWillSelectedPonyGroup.onSelectionChanged.AddListener(option => {
            Settings.freeWillSelectedPony.Value = option;
        });
        freeWillHousehold.onSelectionChanged.AddListener(option => {
            Settings.freeWillHousehold.Value = option;
        });
    }

    /// <summary>
    /// Refresh all settings every time the panel is opened.
    /// </summary>
    private void OnEnable() {
        twelveHourClockToggle.isOn = Settings.twelveHourClock.Value;
        discordIntegrationToggle.isOn = Settings.discordIntegration.Value;
        freeWillSelectedPonyGroup.SetSelection(Settings.freeWillSelectedPony.Value);
        freeWillHousehold.SetSelection(Settings.freeWillHousehold.Value);
    }
}

}