using Assets.Scripts.Util;
using Model.Ponies;
using Model.Property;

namespace Model {

public class Prefabs : SingletonMonoBehaviour<Prefabs> {
    public TerrainTile terrainTilePrefab;
    public FloorTile floorTilePrefab;
    public Wall wallPrefab;
    public PropertyObject propertyObjectPrefab;
    public Pony ponyPrefab;
}

}