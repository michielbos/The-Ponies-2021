using Assets.Scripts.Util;
using Model.Property;
using UnityEngine;

/// <summary>
/// Controller for managing the currently loaded lot.
/// </summary>
public class PropertyController : SingletonMonoBehaviour<PropertyController> {
    public Property propertyPrefab;
    public Property property;

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
    }

    /// <summary>
    /// Save the currently open property.
    /// </summary>
    public void SaveProperty() {
        new PropertyLoader().SaveProperty(property.GetPropertyData());
    }
}