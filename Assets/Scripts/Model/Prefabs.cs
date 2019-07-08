using Assets.Scripts.Util;
using Model.Ponies;
using Model.Property;
using UnityEngine;

namespace Model {

public class Prefabs : SingletonMonoBehaviour<Prefabs> {
    public TerrainTile terrainTilePrefab;
    public FloorTile floorTilePrefab;
    public Wall wallPrefab;
    public PropertyObject propertyObjectPrefab;
    public Pony ponyPrefab;

    public Texture wallIcon;
}

}