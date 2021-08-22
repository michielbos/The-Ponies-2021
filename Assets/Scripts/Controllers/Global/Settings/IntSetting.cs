using UnityEngine;

namespace Controllers.Global.Settings {

/// <summary>
/// Game setting that stores an int value.
/// </summary>
public class IntSetting : Setting<int> {
    public IntSetting(string setting, int defaultValue) : base(setting, defaultValue) { }
    
    /// <summary>
    /// Write an int setting to the PlayerPrefs.
    /// This stores the value as 1 (true) or 0 (false).
    /// </summary>
    protected override void WriteSetting(string setting, int value) {
        PlayerPrefs.SetInt(setting, value);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Read an int setting from the PlayerPrefs.
    /// If the setting does not exist, the default value is returned.
    /// </summary>
    protected override int ReadSetting(string setting, int defaultValue) {
        return PlayerPrefs.GetInt(setting, defaultValue);
    }
}

}