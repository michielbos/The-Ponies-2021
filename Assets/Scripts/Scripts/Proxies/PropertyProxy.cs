using System.Collections.Generic;
using Model.Property;
using MoonSharp.Interpreter;

namespace Scripts.Proxies {

class PropertyProxy {
    private Property property;

    [MoonSharpHidden]
    public PropertyProxy(Property property) {
        this.property = property;
    }

    public string name {
        get => property.propertyName;
        set => property.propertyName = value;
    }

    public string description {
        get => property.description;
        set => property.description = value;
    }
    
    public string street {
        get => property.streetName;
        set => property.streetName = value;
    }

    public IEnumerable<PropertyObject> objects => property.propertyObjects;

    public long time {
        get => property.GameTime;
        set => property.GameTime = value;
    }
}

}