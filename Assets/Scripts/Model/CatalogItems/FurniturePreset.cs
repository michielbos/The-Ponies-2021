﻿using System;
using Model.Property;
using PoneCrafter.Model;
using UnityEngine;

/// <summary>
/// Preset for furniture items. These are the objects that are displayed in the catalog.
/// </summary>
[Serializable]
public class FurniturePreset : Preset {
    private static readonly int ShaderCutoutProperty = Shader.PropertyToID("_AlphaCutOut");
    public readonly bool pickupable;
    public readonly bool sellable;
    public readonly Vector2Int[] occupiedTiles;
    public readonly PlacementRestriction[] placementRestrictions;

    private Mesh mesh;

    // TODO: Bring back skins.
    private Texture2D texture;
    private Material[][] materials;
    private RenderTexture[] previewTextures;

    // TODO: Import missing fields
    public FurniturePreset(Furniture furniture) :
        base(furniture.uuid, furniture.name, furniture.description, furniture.price, furniture.category,
            furniture.needStats, furniture.skillStats, furniture.requiredAge) {
        pickupable = furniture.pickupable;
        sellable = furniture.sellable;
        occupiedTiles = furniture.occupiedTiles;
        placementRestrictions = furniture.placementRestrictions;
        mesh = furniture.mesh;
        texture = furniture.texture;
        materials = new Material[1][];
    }

    public Mesh GetMesh() {
        return mesh;
    }

    /// <summary>
    /// Get the materials for the given skin.
    /// </summary>
    /// <param name="skin"></param>
    /// <returns></returns>
    public Material[] GetMaterials(int skin) {
        if (materials[skin] == null) {
            materials[skin] = new Material[1];
            //TODO: Use the right shader or use a source material.
            materials[skin][0] = new Material(Shader.Find("Cel Shading/Double Sided"));
            materials[skin][0].SetFloat(ShaderCutoutProperty, 0.75f);
            materials[skin][0].mainTexture = texture;
        }
        return materials[skin];
    }

    public override Texture[] GetPreviewTextures() {
        if (previewTextures == null) {
            // TODO: Bring back skins
            previewTextures = new RenderTexture[1];
        }
        Texture[] textures = new Texture[1];
        for (int i = 0; i < previewTextures.Length; i++) {
            if (previewTextures[i] == null || !previewTextures[i].IsCreated()) {
                GameObject previewGeneratorObj = GameObject.FindGameObjectWithTag("PreviewGenerator");
                PreviewGenerator previewGenerator = previewGeneratorObj.GetComponent<PreviewGenerator>();
                previewTextures[i] = previewGenerator.CreatePreview(this, i);
            }
            textures[i] = previewTextures[i];
        }
        return textures;
    }

    public void ApplyToPropertyObject(PropertyObject propertyObject, bool adjustToTiles) {
        ApplyToGameObject(propertyObject.model.gameObject, propertyObject.skin, adjustToTiles);
    }

    /// <summary>
    /// Update a GameObject by applying the rotation/position offsets, model and materials of this furniture preset to it.
    /// </summary>
    /// <param name="gameObject">The GameObject to apply the update to.</param>
    /// <param name="skin">The skin of the furniture item.</param>
    /// <param name="adjustToTiles">Whether AdjustToTiles should be called.
    /// If true, the object will be positioned with its lowest tile on the given position, instead of its center point.</param>
    public void ApplyToGameObject(GameObject gameObject, int skin, bool adjustToTiles) {
        if (adjustToTiles) {
            AdjustToTiles(gameObject.transform);
        }
        if (GetMesh() != null) {
            gameObject.GetComponent<MeshFilter>().mesh = GetMesh();
            gameObject.GetComponent<MeshRenderer>().materials = GetMaterials(skin);
            MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
            if (meshCollider != null) {
                meshCollider.sharedMesh = GetMesh();
            }
        }
    }

    /// <summary>
    /// Adjust the given transform to this preset's occupied tiles.
    /// This will move the lower-left tile of this object to the current center position.
    /// </summary>
    /// <param name="transform">The transform to adjust.</param>
    public void AdjustToTiles(Transform transform) {
        int widthTiles = 0;
        int heightTiles = 0;
        foreach (Vector2Int tile in occupiedTiles) {
            if (tile.x >= widthTiles) {
                widthTiles = tile.x + 1;
            }
            if (tile.y >= heightTiles) {
                heightTiles = tile.y + 1;
            }
        }
        transform.position += new Vector3((widthTiles - 1) * 0.5f, 0, (heightTiles - 1) * 0.5f);
    }

    /// <summary>
    /// Get the coordinates of the tiles that would be occupied by this PropertyObject if it was on the given position.
    /// </summary>
    /// <returns>A Vector2Int array of all coordinates that would be occupied.</returns>
    public Vector2Int[] GetOccupiedTiles(Vector2Int position) {
        Vector2Int[] occupied = new Vector2Int[occupiedTiles.Length];
        for (int i = 0; i < occupied.Length; i++) {
            occupied[i] = position + occupiedTiles[i];
        }
        return occupied;
    }

    /// <summary>
    /// Check whether the furniture preset supports the given type of placement.
    /// </summary>
    /// <param name="placementRestriction">The PlacementRestriction to check</param>
    /// <returns>Whether the given PlacementRestriction is allowed.</returns>
    public bool AllowsPlacement(PlacementRestriction placementRestriction) {
        foreach (PlacementRestriction pr in placementRestrictions) {
            if (placementRestriction == pr)
                return true;
        }
        return false;
    }
}