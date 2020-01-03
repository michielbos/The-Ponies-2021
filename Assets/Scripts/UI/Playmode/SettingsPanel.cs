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

    /// <summary>
    /// Shortcut to the instance of the SettingsController.
    /// </summary>
    private SettingsController Settings => SettingsController.Instance;

    private void Start() {
        twelveHourClockToggle.onValueChanged.AddListener(value => {
            Settings.twelveHourClock.Value = value;
            HUDController.Instance.UpdateTime();
        });
    }

    /// <summary>
    /// Refresh all settings every time the panel is opened.
    /// </summary>
    private void OnEnable() {
        twelveHourClockToggle.isOn = Settings.twelveHourClock.Value;
    }
}

}