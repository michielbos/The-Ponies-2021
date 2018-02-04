
/// <summary>
/// Data class that contains the information of a mod.
/// </summary>
[System.Serializable]
public class ModInfoData {
    public string name;
    public string version;
    public string author;
    public string description;
    public uint packId;

    public ModInfoData (string name, string version, string author, string description, uint packId) {
        this.name = name;
        this.version = version;
        this.author = author;
        this.description = description;
        this.packId = packId;
    }
}
