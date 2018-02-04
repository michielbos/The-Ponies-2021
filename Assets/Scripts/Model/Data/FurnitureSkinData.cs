
/// <summary>
/// Data class that holds the materials of a furniture skin.
/// Used because Unity can't serialize 2D arrays. In the future it could be used to add more data to skins, if needed.
/// </summary>
[System.Serializable]
public class FurnitureSkinData {
    public string[] materialPaths;
    
    public FurnitureSkinData (string[] materialPaths) {
        this.materialPaths = materialPaths;
    }
}