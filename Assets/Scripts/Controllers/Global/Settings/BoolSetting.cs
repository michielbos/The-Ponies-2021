using UnityEngine;

namespace Controllers.Global.Settings {

/// <summary>
/// Game setting that stores a bool value.
/// </summary>
public class BoolSetting : Setting<bool> {
    public BoolSetting(string setting, bool defaultValue) : base(setting, defaultValue) { }
    
    /// <summary>
    /// Write a boolean setting to the PlayerPrefs.
    /// This stores the value as 1 (true) or 0 (false).
    /// </summary>
    protected override void WriteSetting(string setting, bool value) {
        PlayerPrefs.SetInt(setting, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Read a boolean setting from the PlayerPrefs.
    /// A setting is considered true if the value is 1 (or higher).
    /// If the setting does not exist, the default value is returned.
    /// </summary>
    protected override bool ReadSetting(string setting, bool defaultValue) {
        return PlayerPrefs.GetInt(setting, defaultValue ? 1 : 0) > 0;
    }
}

}