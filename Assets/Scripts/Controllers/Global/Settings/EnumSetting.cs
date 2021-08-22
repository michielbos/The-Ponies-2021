using System;
using UnityEngine;

namespace Controllers.Global.Settings {

/// <summary>
/// Game setting that stores an enum value.
/// </summary>
public class EnumSetting<T> : Setting<T> where T : Enum {
    public EnumSetting(string setting, T defaultValue) : base(setting, defaultValue) { }
    
    /// <summary>
    /// Write an enum setting to the PlayerPrefs as int.
    /// This stores the value as 1 (true) or 0 (false).
    /// </summary>
    protected override void WriteSetting(string setting, T value) {
        PlayerPrefs.SetInt(setting, (int)(object)value);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Read an int from the PlayerPrefs as enum setting.
    /// If the setting does not exist, the default value is returned.
    /// </summary>
    protected override T ReadSetting(string setting, T defaultValue) {
        return (T)(object)PlayerPrefs.GetInt(setting, Convert.ToInt32(defaultValue));
    }
}

}