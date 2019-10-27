using System;
using System.Collections.Generic;
using System.Linq;
using Model.Property;
using PoneCrafter.Model;
using UnityEngine;
using UnityGLTF;
using Util;

/// <summary>
/// Preset for furniture items. These are the objects that are displayed in the catalog.
/// </summary>
[Serializable]
public class FurniturePreset : Preset {
    public readonly bool pickupable;
    public readonly bool sellable;
    public readonly Vector2Int[] occupiedTiles;
    public readonly PlacementType placementType;
    public readonly Dictionary<string, string> tags;

    public InstantiatedGLTFObject prefab;

    // TODO: Bring back skins.
    //private Texture2D texture;
    //private Material[][] materials;
    private RenderTexture[] previewTextures;
    
    public string Type => tags.Get("type") ?? "";
    
    public bool IsSurface => Type.Let(it => it == "table" || it == "endTable" || it == "counter" || it == "desk");

    // TODO: Import missing fields
    public FurniturePreset(Furniture furniture) :
        base(furniture.uuid, furniture.name, furniture.description, furniture.price, furniture.category,
            furniture.needStats, furniture.skillStats, furniture.requiredAge) {
        pickupable = furniture.pickupable;
        sellable = furniture.sellable;
        occupiedTiles = furniture.occupiedTiles;
        placementType = furniture.placementType;
        tags = furniture.tags;
        prefab = furniture.prefab;
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

    /// <summary>
    /// Instantiate this preset's prefab into the given model container.
    /// </summary>
    public void ApplyToModel(ModelContainer modelContainer, int skin) {
        modelContainer.InstantiateModel(prefab);
        // It's a bit dirty to do it here, but models from Blender and Max are rotated by 180 degrees.
        modelContainer.Model.transform.Rotate(new Vector3(0, 180, 0));
    }

    /// <summary>
    /// Fix the position and rotation of a model object.
    /// This assumes the position of the parent is the exact tile position.
    /// </summary>
    public void FixModelTransform(Transform model, ObjectRotation rotation) {
        Transform parent = model.parent;
        Vector2Int tileSize = GetTileSize();
        model.localPosition = new Vector3(tileSize.x * 0.5f, 0, tileSize.y * 0.5f);
        model.rotation = Quaternion.identity;
        // It's a bit dirty to do it here, but models from Blender and Max are rotated by 180 degrees.
        model.Rotate(new Vector3(0, 180, 0));
        Vector3 pivot = parent.position + new Vector3(0.5f, 0, 0.5f);
        model.RotateAround(pivot, Vector3.up, rotation.GetRotationAngle());
    }

    public Vector2Int GetTileSize() {
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
        return new Vector2Int(widthTiles, heightTiles);
    }

    /// <summary>
    /// Get the coordinates of the tiles that would be occupied by this PropertyObject if it was on the given position.
    /// </summary>
    /// <returns>A Vector2Int array of all coordinates that would be occupied.</returns>
    public Vector2Int[] GetOccupiedTiles(Vector2Int position, ObjectRotation objectRotation) {
        Vector2Int[] occupied = new Vector2Int[occupiedTiles.Length];
        for (int i = 0; i < occupied.Length; i++) {
            occupied[i] = position + occupiedTiles[i].RotateTileInGroup(objectRotation);
        }
        return occupied;
    }
    
    /// <summary>
    /// Get the tile borders on which this PropertyObject would require walls to be placed on the given tiles.
    /// </summary>
    public IEnumerable<TileBorder> GetRequiredWallBorders(IEnumerable<Vector2Int> tiles, ObjectRotation objectRotation) {
        if (placementType == PlacementType.Wall) {
            return TileUtils.GetBorders(tiles, objectRotation);
        }
        if (placementType == PlacementType.ThroughWall) {
            return TileUtils.GetBorders(tiles, objectRotation, 1);
        }
        return new TileBorder[0];
    }

    public ICollection<Vector3> GetSurfaceSlots() {
        if (!IsSurface)
            return new Vector3[0];
        // TODO: Calculate heights
        return occupiedTiles.Select(tile => new Vector3(tile.x, 0.8f, tile.y)).ToArray();
    }
}