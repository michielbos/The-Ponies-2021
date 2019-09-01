﻿using System;
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

    public PropertyObjectData(int id, int x, int y, int rotation, string furnitureGuid, int skin, int value,
        DataPair[] data) {
        this.id = id;
        this.x = x;
        this.y = y;
        this.rotation = rotation;
        this.furnitureGuid = furnitureGuid;
        this.skin = skin;
        this.value = value;
        this.data = data;
    }

    public ObjectRotation GetObjectRotation() {
        return (ObjectRotation) rotation;
    }
}