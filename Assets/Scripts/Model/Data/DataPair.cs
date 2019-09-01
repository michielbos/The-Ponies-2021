using System;
using System.Globalization;
using System.Linq;
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

    public static bool IsTypeSupported(DynValue dynValue) => SupportedTypes.Contains(dynValue.Type);

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
            default:
                throw new ArgumentOutOfRangeException(dynValue.Type.ToString());
        }
    }

    public DynValue GetDynKey() => GetDynValue(keyType, key);
    
    public DynValue GetDynValue() => GetDynValue(valueType, value);

    private DynValue GetDynValue(string type, string value) {
        switch (type) {
            case "string":
                return DynValue.NewString(value);
            case "number":
                return DynValue.NewNumber(double.Parse(value));
            case "boolean":
                return DynValue.NewBoolean(bool.Parse(value));
            case "null":
                return DynValue.NewNil();
            default:
                throw new ArgumentOutOfRangeException(type);
        }
    }
}

}