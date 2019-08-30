using System.Collections.Generic;
using Model.Property;
using MoonSharp.Interpreter;

namespace Scripts.Proxies {

public class PropertyObjectProxy {
    private readonly PropertyObject propertyObject;

    [MoonSharpHidden]
    public PropertyObjectProxy(PropertyObject propertyObject) {
        this.propertyObject = propertyObject;
    }
    
    public int id => propertyObject.id;
    
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

    public IDictionary<DynValue, DynValue> data => propertyObject.data;

    public void putData(DynValue key, DynValue value) => propertyObject.data[key] = value;
    
    public void removeData(DynValue key) => propertyObject.data.Remove(key);

    protected bool Equals(PropertyObjectProxy other) {
        return Equals(propertyObject, other.propertyObject);
    }

    public override bool Equals(object obj) {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((PropertyObjectProxy) obj);
    }

    public override int GetHashCode() {
        return (propertyObject != null ? propertyObject.GetHashCode() : 0);
    }
}

}