using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using JetBrains.Annotations;
using Model.Actions;
using Model.Data;
using Model.Ponies;
using UnityEngine;
using Util;

namespace Model.Property {

/// <summary>
/// An object that can be placed on a lot, usually a piece of furniture.
/// </summary>
[System.Serializable]
[RequireComponent(typeof(ModelContainer))]
public class PropertyObject : MonoBehaviour, IActionTarget {
    public int id;
    public FurniturePreset preset;
    public int skin;
    public int value;

    public readonly DataMap data = new DataMap();
    public readonly HashSet<Pony> users = new HashSet<Pony>();

    private AudioSource audioSource;
    private string lastAnimation;
    private string lastSound;
    private SurfaceSlot[] surfaceSlots = new SurfaceSlot[0];

    public Transform Model => GetComponent<ModelContainer>().Model.transform;

    public string Type => preset.Type;

    /// <summary>
    /// Shortcut to get all child objects that are filling this object's surface slots.
    /// </summary>
    public IEnumerable<PropertyObject> Children => surfaceSlots.Where(it => it.SlotObject != null)
        .Select(it => it.SlotObject);

    /// <summary>
    /// The parent object that has this object as a child, if any.
    /// </summary>
    [CanBeNull]
    public IObjectSlot ParentSlot => transform.parent.GetComponent<IObjectSlot>();

    public Vector2Int TilePosition {
        get {
            Vector3 position = transform.position;
            return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        }
        set { transform.position = new Vector3(value.x, 0, value.y); }
    }

    public ObjectRotation Rotation {
        get => Model.transform.GetObjectRotation();
        set => preset.FixModelTransform(Model, value, IsChild);
    }

    public bool IsChild => ParentSlot != null;

    /// <summary>
    /// Whether the object has been picked up by a pony.
    /// </summary>
    private bool IsPickedUp => ParentSlot is HoofSlot;

    /// <summary>
    /// Whether the object can be picked up in buy mode.
    /// </summary>
    public bool Pickupable => preset.pickupable && !IsPickedUp && users.Count == 0;

    public void Init(int id, IObjectSlot objectSlot, ObjectRotation rotation, FurniturePreset preset, int skin, int value,
        string animation) {
        this.id = id;
        this.preset = preset;
        this.skin = skin;
        this.value = value;
        preset.ApplyToModel(GetComponent<ModelContainer>(), skin);
        objectSlot.PlaceObject(this);
        Rotation = ObjectRotation.SouthEast;
        surfaceSlots = preset.GetSurfaceSlots().Select(pos => SurfaceSlot.CreateSlot(this, pos)).ToArray();
        foreach (SurfaceSlot slot in surfaceSlots) {
            slot.MatchHeightWithOwner();
        }
        Rotation = rotation;
        if (!string.IsNullOrEmpty(animation))
            PlayAnimation(animation);
        AddCutoutsToWalls();
    }

    public void InitScriptData(DataPair[] data, IEnumerable<Pony> users, Property property) {
        foreach (DataPair pair in data) {
            this.data.Put(pair.key, pair.GetValue(property));
        }
        foreach (Pony user in users) {
            this.users.Add(user);
        }
    }

    public PropertyObjectData GetPropertyObjectData() {
        Vector2Int tilePosition = TilePosition;
        return new PropertyObjectData(id,
            tilePosition.x,
            tilePosition.y,
            (int) Rotation,
            preset.guid.ToString(),
            skin,
            value,
            data.GetDataPairs(),
            GetAnimation(),
            users.Select(pony => pony.uuid.ToString()).ToArray(),
            surfaceSlots.Select(child => child.SlotObject != null ? child.SlotObject.GetChildObjectData() : null)
                .ToArray());
    }

    private ChildObjectData GetChildObjectData() {
        return new ChildObjectData(GetPropertyObjectData());
    }

    /// <summary>
    /// Get the coordinates of the tiles occupied by this PropertyObject.
    /// </summary>
    /// <returns>A Vector2Int array of all occupied coordinates.</returns>
    public Vector2Int[] GetOccupiedTiles() {
        return preset.GetOccupiedTiles(TilePosition, Rotation);
    }

    /// <summary>
    /// Get the coordinates of the tiles occupied by this PropertyObject.
    /// Excludes the ones that ponies can walk through, such as doors.
    /// </summary>
    public Vector2Int[] GetImpassableTiles() {
        string type = preset.tags.Get("type");
        if (type == "door")
            return new Vector2Int[0];
        return preset.GetOccupiedTiles(TilePosition, Rotation);
    }

    /// <summary>
    /// Get the tile borders on which this PropertyObject requires walls to be placed.
    /// </summary>
    /// <returns>A collection of all required wall borders.</returns>
    public IEnumerable<TileBorder> GetRequiredWallBorders() {
        return preset.GetRequiredWallBorders(GetOccupiedTiles(), Rotation);
    }

    public ICollection<PonyAction> GetActions(Pony pony) {
        return ActionManager.GetActionsForObject(pony, this);
    }

    public bool PlaySound(string name) {
        AudioClip audioClip = ContentController.Instance.GetAudioClip(name);
        if (audioClip == null)
            return false;
        PlaySound(audioClip);
        lastSound = name;
        return true;
    }

    private void PlaySound(AudioClip clip) {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StopSound() {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    public string GetPlayingSound() {
        if (audioSource == null || !audioSource.isPlaying)
            return null;
        return lastSound;
    }

    public bool PlayAnimation(string name) {
        Animation animation = GetComponentInChildren<Animation>();
        if (animation == null || !animation.Play(name))
            return false;
        lastAnimation = name;
        return true;
    }

    public string GetAnimation() {
        Animation animation = GetComponentInChildren<Animation>();
        if (animation != null && animation.IsPlaying(lastAnimation))
            return lastAnimation;
        return null;
    }

    /// <summary>
    /// Get the surface slot with the given slot index.
    /// </summary>
    public SurfaceSlot GetSurfaceSlot(int slotIndex) {
        return surfaceSlots[slotIndex];
    }

    [CanBeNull]
    public IObjectSlot GetClosestSurfaceSlot(Vector3 worldPosition) {
        SurfaceSlot closest = null;
        float bestDistance = 9999f;
        foreach (SurfaceSlot slot in surfaceSlots) {
            float distance = Vector3.Distance(worldPosition, slot.SlotPosition);
            if (closest == null || distance < bestDistance) {
                closest = slot;
                bestDistance = distance;
            }
        }
        return closest;
    }

    /// <summary>
    /// Remove this object from its parent slot, if it has one.
    /// </summary>
    public void ClearParent() {
        IObjectSlot parentSlot = ParentSlot;
        if (parentSlot == null)
            return;
        parentSlot.SlotObject = null;
        transform.parent = PropertyController.Property.transform;
    }
    
    /// <summary>
    /// Called when an object is placed. Can be either from buying, moving or loading.
    /// </summary>
    public void OnPlaced() {
        preset.FixModelTransform(Model, Rotation, IsChild);
        AddCutoutsToWalls();
    }
    
    /// <summary>
    /// Called when an object is picked up in buy mode.
    /// This can be followed by either OnPlaced() if the object is moved, or by OnDestroy() if the object is sold.
    /// </summary>
    public void OnPickedUp() {
        RemoveCutoutsFromWalls();
    }
    
    /// <summary>
    /// Called by Unity when the object is destroyed.
    /// This usually means the item is sold or destroyed by script (for example by fire).
    /// </summary>
    private void OnDestroy() {
        RemoveCutoutsFromWalls();
    }

    /// <summary>
    /// Add cutouts to all covered walls if this object provides cutout textures.
    /// </summary>
    private void AddCutoutsToWalls() {
        if (preset.placementType != PlacementType.ThroughWall)
            return;
        if (preset.cutoutTextures.Length == 0)
            return;
        IOrderedEnumerable<Wall> coveredWalls = PropertyController.Property
            .GetWalls(GetRequiredWallBorders())
            .OrderBy(wall => wall.TileBorder.x + wall.TileBorder.y);

        int counter = 0;
        foreach (Wall wall in coveredWalls) {
            if (counter >= preset.cutoutTextures.Length)
                return;
            wall.cutoutTexture = preset.cutoutTextures[counter++];
            wall.RefreshMaterials();
        }
    }

    /// <summary>
    /// Removes the cutouts that were placed by AddCutoutsToWalls().
    /// </summary>
    private void RemoveCutoutsFromWalls() {
        if (preset.placementType != PlacementType.ThroughWall)
            return;
        if (preset.cutoutTextures.Length == 0)
            return;
        // This prevents warnings when exiting play mode.
        if (!PropertyController.HasInstance)
            return;
        IEnumerable<Wall> coveredWalls = PropertyController.Property.GetWalls(GetRequiredWallBorders());
        foreach (Wall wall in coveredWalls) {
            wall.cutoutTexture = null;
            wall.RefreshMaterials();
        }
    }

    
}

}