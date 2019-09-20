using System;
using System.Globalization;
using System.Linq;
using Model.Ponies;
using Model.Property;

namespace Model.Data {

[Serializable]
public class DataPair {
    public string key;
    public string valueType;
    public string value;

    private DataPair(string key, string valueType, string value) {
        this.key = key;
        this.valueType = valueType;
        this.value = value;
    }

    public static bool IsTypeSupported(object value) {
        return value is string || value is int || value is float || value is bool;
    }

    public static DataPair FromValues(string key, object valueValue) {
        return new DataPair(
            key,
            GetValueType(valueValue),
            GetValueValue(valueValue)
        );
    }

    private static string GetValueType(object value) {
        switch (value) {
            case string v:
                return "string";
            case int v:
                return "int";
            case float v:
                return "float";
            case bool v:
                return "bool";
            case null:
                return "null";
            case PropertyObject v:
                return "propertyObject";
            case Pony v:
                return "pony";
            default:
                throw new ArgumentOutOfRangeException(value.GetType().ToString());
        }
    }
    
    private static string GetValueValue(object value) {
        switch (value) {
            case String v:
                return v;
            case int v:
                return v.ToString(CultureInfo.InvariantCulture);
            case float v:
                return v.ToString(CultureInfo.InvariantCulture);
            case bool v:
                return v.ToString();
            case null:
                return "null";
            case PropertyObject propertyObject:
                return propertyObject.id.ToString(CultureInfo.InvariantCulture);
            case Pony pony:
                return pony.uuid.ToString();
            default:
                throw new ArgumentOutOfRangeException(value.GetType().ToString());
        }
    }
    
    public object GetValue(Property.Property property) => GetValue(valueType, value, property);

    private object GetValue(string type, string value, Property.Property property) {
        switch (type) {
            case "string":
                return value;
            case "int":
                return int.Parse(value);
            case "float":
                return float.Parse(value);
            case "bool":
                return bool.Parse(value);
            case "null":
                return null;
            case "propertyObject":
                return property.GetPropertyObject(int.Parse(value));
            case "pony":
                return property.GetPony(new Guid(value));
            default:
                throw new ArgumentOutOfRangeException(type);
        }
    }
}

}