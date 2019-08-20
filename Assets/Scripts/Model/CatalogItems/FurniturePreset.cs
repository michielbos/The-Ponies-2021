using System;
using System.Collections.Generic;
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
    private static readonly int ShaderCutoutProperty = Shader.PropertyToID("_AlphaCutout");
    public readonly bool pickupable;
    public readonly bool sellable;
    public readonly Vector2Int[] occupiedTiles;
    public readonly PlacementType placementType;

    public InstantiatedGLTFObject prefab;

    // TODO: Bring back skins.
    //private Texture2D texture;
    //private Material[][] materials;
    private RenderTexture[] previewTextures;

    // TODO: Import missing fields
    public FurniturePreset(Furniture furniture) :
        base(furniture.uuid, furniture.name, furniture.description, furniture.price, furniture.category,
            furniture.needStats, furniture.skillStats, furniture.requiredAge) {
        pickupable = furniture.pickupable;
        sellable = furniture.sellable;
        occupiedTiles = furniture.occupiedTiles;
        placementType = furniture.placementType;
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
    /// <param name="model">The GameObject to apply the update to.</param>
    /// <param name="skin">The skin of the furniture item.</param>
    public void ApplyToModel(ModelContainer modelContainer, int skin) {
        modelContainer.InstantiateModel(prefab);
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
        Vector3 pivot = parent.position + new Vector3(0.5f, 0, 0.5f);
        model.RotateAround(pivot, Vector3.up, ObjectRotationUtil.GetRotationAngle(rotation));
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
            occupied[i] = position + RotateTile(occupiedTiles[i], objectRotation);
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

    private Vector2Int RotateTile(Vector2Int tile, ObjectRotation rotation) {
        // Sorry for the yuckiness, but at least it works now. And it's actually performance efficiënt-ish.
        if (rotation == ObjectRotation.SouthWest) {
            return new Vector2Int(tile.y, -tile.x);
        }
        if (rotation == ObjectRotation.NorthWest) {
            return new Vector2Int(-tile.x, -tile.y);
        }
        if (rotation == ObjectRotation.NorthEast) {
            return new Vector2Int(-tile.y, tile.x);
        }
        // SouthEast
        return tile;
    }
}