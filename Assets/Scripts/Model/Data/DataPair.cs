using System;
using System.Globalization;
using System.Linq;
using Model.Ponies;
using Model.Property;
using MoonSharp.Interpreter;

namespace Model.Data {

[Serializable]
public class DataPair {
    private static readonly DataType[] SupportedTypes =
        {DataType.String, DataType.Number, DataType.Boolean, DataType.Nil};
    public string keyType;
    public string key;
    public string valueType;
    public string value;

    private DataPair(string keyType, string key, string valueType, string value) {
        this.keyType = keyType;
        this.key = key;
        this.valueType = valueType;
        this.value = value;
    }

    public static bool IsTypeSupported(DynValue dynValue) {
        if (SupportedTypes.Contains(dynValue.Type))
            return true;
        if (dynValue.Type == DataType.UserData) {
            object obj = dynValue.UserData.Object;
            return obj is PropertyObject || obj is Pony;
        }
        return false;
    }

    public static DataPair FromDynValues(DynValue keyValue, DynValue valueValue) {
        return new DataPair(
            GetDynValueType(keyValue),
            GetDynValueValue(keyValue),
            GetDynValueType(valueValue),
            GetDynValueValue(valueValue)
        );
    }

    private static string GetDynValueType(DynValue dynValue) {
        switch (dynValue.Type) {
            case DataType.String:
                return "string";
            case DataType.Number:
                return "number";
            case DataType.Boolean:
                return "boolean";
            case DataType.Nil:
                return "null";
            case DataType.UserData when dynValue.UserData.Object is PropertyObject:
                return "propertyObject";
            case DataType.UserData when dynValue.UserData.Object is Pony:
                return "pony";
            default:
                throw new ArgumentOutOfRangeException(dynValue.Type.ToString());
        }
    }
    
    private static string GetDynValueValue(DynValue dynValue) {
        switch (dynValue.Type) {
            case DataType.String:
                return dynValue.String;
            case DataType.Number:
                return dynValue.Number.ToString(CultureInfo.InvariantCulture);
            case DataType.Boolean:
                return dynValue.Boolean.ToString();
            case DataType.Nil:
                return "null";
            case DataType.UserData when dynValue.UserData.Object is PropertyObject propertyObject:
                return propertyObject.id.ToString(CultureInfo.InvariantCulture);
            case DataType.UserData when dynValue.UserData.Object is Pony pony:
                return pony.uuid.ToString();
            default:
                throw new ArgumentOutOfRangeException(dynValue.Type.ToString());
        }
    }

    public DynValue GetDynKey(Property.Property property) => GetDynValue(keyType, key, property);
    
    public DynValue GetDynValue(Property.Property property) => GetDynValue(valueType, value, property);

    private DynValue GetDynValue(string type, string value, Property.Property property) {
        switch (type) {
            case "string":
                return DynValue.NewString(value);
            case "number":
                return DynValue.NewNumber(double.Parse(value));
            case "boolean":
                return DynValue.NewBoolean(bool.Parse(value));
            case "null":
                return DynValue.NewNil();
            case "propertyObject":
                return UserData.Create(property.GetPropertyObject(int.Parse(value)));
            case "pony":
                return UserData.Create(property.GetPony(new Guid(value)));
            default:
                throw new ArgumentOutOfRangeException(type);
        }
    }
}

}