using UnityEngine;

namespace Model.CatalogItems {

public class WallPreset : CatalogItem {
    public WallPreset(string name, string description, int price) :
        base(name, description, price, ObjectCategory.Wall) { }

    public override Texture[] GetPreviewTextures() {
        return new[] {Prefabs.Instance.wallIcon};
    }
}

}