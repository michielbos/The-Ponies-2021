using System;
using Model.Property;
using PoneCrafter.Model;
using UnityEngine;

/// <summary>
/// Preset for furniture items. These are the objects that are displayed in the catalog.
/// </summary>
[Serializable]
public class FurniturePreset : Preset {
    private static readonly int ShaderCutoutProperty = Shader.PropertyToID("_AlphaCutout");
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
            materials[skin][0] = new Material(Shader.Find("Cel Shading/RegularV2"));
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

    public void ApplyToPropertyObject(PropertyObject propertyObject) {
        ApplyToModel(propertyObject.model.gameObject, propertyObject.skin);
    }

    /// <summary>
    /// Update a GameObject by applying the rotation/position offsets, model and materials of this furniture preset to it.
    /// </summary>
    /// <param name="model">The GameObject to apply the update to.</param>
    /// <param name="skin">The skin of the furniture item.</param>
    public void ApplyToModel(GameObject model, int skin) {
        if (GetMesh() != null) {
            model.GetComponent<MeshFilter>().mesh = GetMesh();
            model.GetComponent<MeshRenderer>().materials = GetMaterials(skin);
            MeshCollider meshCollider = model.GetComponent<MeshCollider>();
            if (meshCollider != null) {
                meshCollider.sharedMesh = GetMesh();
            }
        }
    }

    /// <summary>
    /// Fix the position and rotation of a model object.
    /// This assumes the position of the parent is the exact tile position.
    /// </summary>
    public void FixModelTransform(Transform model, ObjectRotation rotation) {
        Transform parent = model.parent;
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
        model.localPosition = new Vector3(widthTiles * 0.5f, 0, heightTiles * 0.5f);
        model.rotation = Quaternion.identity;
        Vector3 pivot = parent.position + new Vector3(0.5f, 0, 0.5f);
        model.RotateAround(pivot, Vector3.up, ObjectRotationUtil.GetRotationAngle(rotation));
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