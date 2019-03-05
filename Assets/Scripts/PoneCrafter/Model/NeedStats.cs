namespace PoneCrafter.Model {

/// <summary>
/// Class for keeping track of need stats, for example for displaying on the furniture info tab.
/// </summary>
[System.Serializable]
public class NeedStats {
    public int hunger;
    public int energy;
    public int comfort;
    public int fun;
    public int hygiene;
    public int social;
    public int bladder;
    public int room;
    
    /// <summary>
    /// Get the text to display the content of this class to the user.
    /// </summary>
    /// <returns>The text representation.</returns>
    public string GetDisplayText () {
        string text = "";
        text = AddToDisplayText(text, "Hunger", hunger);
        text = AddToDisplayText(text, "Energy", energy);
        text = AddToDisplayText(text, "Comfort", comfort);
        text = AddToDisplayText(text, "Fun", fun);
        text = AddToDisplayText(text, "Hygiene", hygiene);
        text = AddToDisplayText(text, "Social", social);
        text = AddToDisplayText(text, "Bladder", bladder);
        text = AddToDisplayText(text, "Room", room);
        return text;
    }

    /// <summary>
    /// Add an item to the display text if its value is not 0.
    /// </summary>
    /// <param name="text">The current display text.</param>
    /// <param name="name">The name of the item to add.</param>
    /// <param name="value">The value of the item to add.</param>
    /// <returns>The new display text.</returns>
    private string AddToDisplayText (string text, string name, int value) {
        if (value == 0)
            return text;
        if (text.Length > 0) {
            text += "\n";
        }
        return text + name + ": " + value;
    } 
}

}