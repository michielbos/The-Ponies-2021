using System.Collections.Generic;
using Model.Property;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Scripts.Proxies {

class PropertyObjectProxy {
    private PropertyObject propertyObject;

    [MoonSharpHidden]
    public PropertyObjectProxy(PropertyObject propertyObject) {
        this.propertyObject = propertyObject;
    }
    
    public string uuid => propertyObject.preset.guid.ToString();

    public int x {
        get => propertyObject.TilePosition.x;
        set {
            Vector2Int tilePosition = propertyObject.TilePosition;
            tilePosition.x = value;
            propertyObject.TilePosition = tilePosition;
        }
    }
    
    public int y {
        get => propertyObject.TilePosition.y;
        set {
            Vector2Int tilePosition = propertyObject.TilePosition;
            tilePosition.y = value;
            propertyObject.TilePosition = tilePosition;
        }
    }

    public int rotation {
        get => ObjectRotationUtil.GetRotationAngle(propertyObject.Rotation);
        set => propertyObject.Rotation = ObjectRotationUtil.FromRotationAngle(value);
    }
    
    public int value {
        get => propertyObject.value;
        set => propertyObject.value = value;
    }
}

}