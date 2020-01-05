namespace Controllers.Global.Settings {

/// <summary>
/// Abstract game setting, which is a key-value pair that represents a setting in the PlayerPrefs.
/// </summary>
/// <typeparam name="T">The type of the setting value.</typeparam>
public abstract class Setting<T> {
    /// <summary>
    /// The setting key that is used in the preferences file.
    /// </summary>
    private readonly string setting;
    
    /// <summary>
    /// The current value of the setting.
    /// </summary>
    private T _value;

    /// <summary>
    /// The value of the setting.
    /// Writing to this value will write it to the preferences file.
    /// </summary>
    public T Value {
        get => _value;
        set {
            _value = value;
            WriteSetting(setting, value);
        }
    }

    /// <summary>
    /// Construct a new game setting.
    /// </summary>
    /// <param name="setting">The setting key that is used in the preferences file.</param>
    /// <param name="defaultValue">The default value that is used if there is no saved value.</param>
    protected Setting(string setting, T defaultValue) {
        this.setting = setting;
        _value = defaultValue;
    }

    /// <summary>
    /// Load this setting from the preferences file, if it exists.
    /// If no value exists in the preferences file, the current (default) value is used.
    /// </summary>
    public void Load() {
        _value = ReadSetting(setting, _value);
    }

    /// <summary>
    /// Write the given value to preferences file under the given setting.
    /// </summary>
    /// <param name="setting">The setting key in the preferences file.</param>
    /// <param name="value">The value to write to the preferences file.</param>
    protected abstract void WriteSetting(string setting, T value);

    /// <summary>
    /// Read a setting from the preferences file.
    /// If the setting does not exist, the defaultValue should be returned.
    /// </summary>
    /// <param name="setting">The setting key to read from the preferences file.</param>
    /// <param name="defaultValue">The default value to return if no values exists.</param>
    /// <returns>The value that was read.</returns>
    protected abstract T ReadSetting(string setting, T defaultValue);
}

}