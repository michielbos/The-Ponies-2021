/// <summary>
/// Class for keeping track of skill stats, for example for displaying on the furniture info tab.
/// </summary>
[System.Serializable]
public class SkillStats {
    public int cooking;
    public int mechanical;
    public int charisma;
    public int body;
    public int logic;
    public int creativity;
    
    /// <summary>
    /// Get the text to display the content of this class to the user.
    /// </summary>
    /// <returns>The text representation.</returns>
    public string GetDisplayText () {
        string text = "";
        text = AddToDisplayText(text, "Cooking", cooking);
        text = AddToDisplayText(text, "Mechanical", mechanical);
        text = AddToDisplayText(text, "Charisma", charisma);
        text = AddToDisplayText(text, "Body", body);
        text = AddToDisplayText(text, "Logic", logic);
        text = AddToDisplayText(text, "Creativity", creativity);
        return text;
    }

    /// <summary>
    /// Add an item to the display text if its value is more than 0.
    /// </summary>
    /// <param name="text">The current display text.</param>
    /// <param name="name">The name of the item to add.</param>
    /// <param name="value">The value of the item to add.</param>
    /// <returns>The new display text.</returns>
    private string AddToDisplayText (string text, string name, int value) {
        if (value <= 0)
            return text;
        if (text.Length > 0) {
            text += "\n";
        }
        return text + "+ " + name;
    } 
}