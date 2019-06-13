﻿using Assets.Scripts.Util;
using Model.Property;
using UnityEngine;

/// <summary>
/// Controller for managing the currently loaded lot.
/// </summary>
public class PropertyController : SingletonMonoBehaviour<PropertyController> {
    public Property propertyPrefab;
    public GameObject terrainTilePrefab;
    public GameObject propertyObjectPrefab;
    public Property property;
    private int nextObjectId;

    /// <summary>
    /// Called when the scene is just started and a property should be loaded.
    /// </summary>
    /// <param name="propertyId">The id of the property to load.</param>
    public void Initialize(int propertyId) {
        PropertyData propertyData = new PropertyLoader().LoadOrCreateProperty(propertyId);
        property = Instantiate(propertyPrefab);
        property.Init(propertyData.id, propertyData.name, propertyData.description, propertyData.streetName,
            propertyData.GetPropertyType());
        property.SpawnObjects(propertyData);
        foreach (PropertyObject propertyObject in property.propertyObjects) {
            if (propertyObject.id >= nextObjectId) {
                nextObjectId = propertyObject.id + 1;
            }
        }
        PlacePropertyObjects();
    }

    /// <summary>
    /// Save the currently open property.
    /// </summary>
    public void SaveProperty() {
        new PropertyLoader().SaveProperty(property.GetPropertyData());
    }

    /// <summary>
    /// Place "dummy" instances of all loaded property objects, so they are visible and interactable.
    /// </summary>
    private void PlacePropertyObjects() {
        foreach (PropertyObject po in property.propertyObjects) {
            po.PlaceObject(propertyObjectPrefab);
        }
    }

    /// <summary>
    /// Add a new property object to the property. This will also instantiate a dummy to make the object visible and interactable.
    /// </summary>
    /// <param name="x">The X position of the object.</param>
    /// <param name="y">The Y position of the object.</param>
    /// <param name="objectRotation">The rotation of the object.</param>
    /// <param name="preset">The FurniturePreset that this object is based on.</param>
    /// <param name="skin">The number of the skin to apply to this object.</param>
    public void PlacePropertyObject(int x, int y, ObjectRotation objectRotation, FurniturePreset preset, int skin) {
        PropertyObject propertyObject = new PropertyObject(nextObjectId++, x, y, objectRotation, preset, skin);
        property.propertyObjects.Add(propertyObject);
        propertyObject.PlaceObject(propertyObjectPrefab);
    }

    /// <summary>
    /// Remove a property object from the property. This will also clean up the dummy object in the scene.
    /// </summary>
    /// <param name="propertyObject">The PropertyObject to remove.</param>
    public void RemovePropertyObject(PropertyObject propertyObject) {
        propertyObject.RemoveObject();
        property.propertyObjects.Remove(propertyObject);
    }
}