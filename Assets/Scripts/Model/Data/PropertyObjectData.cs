using System;
using Model.Data;
using Model.Property;

[Serializable]
public class PropertyObjectData {
    public int id;
    public int x;
    public int y;
    public int rotation;
    public string furnitureGuid;
    public int skin;
    public int value;
    public DataPair[] data;
    public string animation;
    public string[] users;
    public ChildObjectData[] children;

    public PropertyObjectData(int id, int x, int y, int rotation, string furnitureGuid, int skin, int value,
        DataPair[] data, string animation, string[] users, ChildObjectData[] children) {
        this.id = id;
        this.x = x;
        this.y = y;
        this.rotation = rotation;
        this.furnitureGuid = furnitureGuid;
        this.skin = skin;
        this.value = value;
        this.data = data;
        this.animation = animation;
        this.users = users;
        this.children = children;
    }
    
    public PropertyObjectData(ChildObjectData cod) : this(cod.id, cod.x, cod.y, cod.rotation, cod.furnitureGuid,
        cod.skin, cod.value, cod.data, cod.animation, cod.users, new ChildObjectData[0]) { }

    public ObjectRotation GetObjectRotation() {
        return (ObjectRotation) rotation;
    }
}