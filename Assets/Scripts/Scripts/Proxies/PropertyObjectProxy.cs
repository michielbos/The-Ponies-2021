using Model.Property;
using MoonSharp.Interpreter;

namespace Scripts.Proxies {

public class PropertyObjectProxy {
    private PropertyObject propertyObject;

    [MoonSharpHidden]
    public PropertyObjectProxy(PropertyObject propertyObject) {
        this.propertyObject = propertyObject;
    }
    
    public string uuid => propertyObject.preset.guid.ToString();

    public Vector2Wrapper position {
        get => new Vector2Wrapper(propertyObject.TilePosition);
        set => propertyObject.TilePosition = value.GetVector2Int();
    }

    public int rotation {
        get => ObjectRotationUtil.GetRotationAngle(propertyObject.Rotation);
        set => propertyObject.Rotation = ObjectRotationUtil.FromRotationAngle(value);
    }
    
    public int value {
        get => propertyObject.value;
        set => propertyObject.value = value;
    }

    public FurniturePreset preset => propertyObject.preset;
}

}