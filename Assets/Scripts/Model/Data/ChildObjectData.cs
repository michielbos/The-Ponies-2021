using System;
using Model.Data;
using Model.Property;

/// <summary>
/// An almost exact copy of PropertyObjectData because Unity's serializer is rubbish.
/// Does not contain a children array.
/// </summary>
[Serializable]
public class ChildObjectData {
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

    public ChildObjectData(int id, int x, int y, int rotation, string furnitureGuid, int skin, int value,
        DataPair[] data, string animation, string[] users) {
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
    }

    /// <summary>
    /// Construct a ChildObjectData from a PropertyObjectData, without the children.
    /// </summary>
    /// <param name="pod"></param>
    public ChildObjectData(PropertyObjectData pod) : this(pod.id, pod.x, pod.y, pod.rotation, pod.furnitureGuid,
        pod.skin, pod.value, pod.data, pod.animation, pod.users) { }

    public ObjectRotation GetObjectRotation() {
        return (ObjectRotation) rotation;
    }
}