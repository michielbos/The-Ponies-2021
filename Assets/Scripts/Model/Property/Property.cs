using System;
using System.Collections.Generic;
using System.Linq;
using Controllers.Playmode;
using JetBrains.Annotations;
using Model.Actions;
using Model.Data;
using Model.Ponies;
using UnityEngine;

namespace Model.Property {

[Serializable]
public class Property : MonoBehaviour {
    public int id;
    public string propertyName;
    public string description;
    public string streetName;
    private long time;
    public PropertyType propertyType;
    public TerrainTile[,] terrainTiles;
    public FloorTile[,,] floorTiles;
    public Dictionary<TileBorder, Wall> walls;
    public List<Roof> roofs;
    public List<PropertyObject> propertyObjects;
    public List<Pony> ponies;
    public Room[] Rooms { get; private set; } = new Room[0];
    [CanBeNull] public Household household;
    private int nextObjectId;

    public int TerrainWidth => terrainTiles.GetLength(1);
    public int TerrainHeight => terrainTiles.GetLength(0);
    
    /// <summary>
    /// The current game time, in ingame minutes.
    /// If time does not matter for this property (because there is no household), 0 is returned.
    /// </summary>
    public long GameTime {
        get { return household != null ? time : 0; }
        set {
            if (household != null) {
                time = value;
            }
        }
    }
    
    public void Init(int id, string propertyName, string description, string streetName, PropertyType propertyType,
        long time) {
        this.id = id;
        this.propertyName = propertyName;
        this.streetName = streetName;
        this.description = description;
        this.propertyType = propertyType;
        this.time = time;
        walls = new Dictionary<TileBorder, Wall>();
        roofs = new List<Roof>();
        propertyObjects = new List<PropertyObject>();
    }

    public void SpawnObjects(PropertyData propertyData) {
        LoadTerrainTiles(propertyData.terrainTileDatas);
        LoadWalls(propertyData.wallDatas);
        LoadFloorTiles(propertyData.floorTileDatas);
        LoadPropertyObjects(propertyData.propertyObjectDatas);
        LoadPonies(propertyData.ponies, propertyData.householdData);
    }

    public void PlaceFloor(int x, int y, FloorPreset preset) {
        //TODO: Floor level
        if (floorTiles[0, y, x] != null) {
            RemoveFloor(floorTiles[0, y, x]);
        }
        FloorTile floorTile = Instantiate(Prefabs.Instance.floorTilePrefab, transform);
        floorTile.Init(x, y, preset);
        floorTiles[0, y, x] = floorTile;
        terrainTiles[y, x].SetVisible(false);
    }

    public Wall PlaceWall(int x, int y, WallDirection wallDirection, bool updateRooms) {
        Wall wall = Instantiate(Prefabs.Instance.wallPrefab, transform);
        wall.Init(x, y, wallDirection);
        walls.Add(wall.TileBorder, wall);
        if (updateRooms) {
            UpdateRooms();
        }
        return wall;
    }

    public void PlacePropertyObject(int x, int y, ObjectRotation objectRotation, FurniturePreset preset, int skin) {
        PlacePropertyObject(nextObjectId++, x, y, objectRotation, preset, skin);
    }

    private void PlacePropertyObject(int id, int x, int y, ObjectRotation objectRotation, FurniturePreset preset,
        int skin) {
        if (id >= nextObjectId) {
            nextObjectId = id + 1;
        }
        PropertyObject propertyObject = Instantiate(Prefabs.Instance.propertyObjectPrefab, transform);
        propertyObject.Init(id, x, y, objectRotation, preset, skin);
        propertyObjects.Add(propertyObject);
    }

    public void RemoveFloor(FloorTile floorTile) {
        Destroy(floorTile.gameObject);
        //TODO: Floor level
        Vector2Int tilePosition = floorTile.TilePosition;
        floorTiles[0, tilePosition.y, tilePosition.x] = null;
        terrainTiles[tilePosition.y, tilePosition.x].SetVisible(true);
    }

    public void RemoveWall(Wall wall, bool updateRooms) {
        Destroy(wall.gameObject);
        walls.Remove(wall.TileBorder);
        if (updateRooms) {
            UpdateRooms();
        }
    }

    public void RemovePropertyObject(PropertyObject propertyObject) {
        Destroy(propertyObject.gameObject);
        propertyObjects.Remove(propertyObject);
    }

    private void LoadTerrainTiles(TerrainTileData[] terrainTileDatas) {
        int width = 0;
        int height = 0;
        foreach (TerrainTileData ttd in terrainTileDatas) {
            if (ttd.x >= width) {
                width = ttd.x + 1;
            }
            if (ttd.y >= height) {
                height = ttd.y + 1;
            }
        }
        terrainTiles = new TerrainTile[height, width];
        foreach (TerrainTileData ttd in terrainTileDatas) {
            if (terrainTiles[ttd.y, ttd.x] == null) {
                TerrainTile terrainTile = Instantiate(Prefabs.Instance.terrainTilePrefab, transform);
                terrainTile.Init(ttd.x, ttd.y, ttd.height, ttd.type);
                terrainTiles[ttd.y, ttd.x] = terrainTile;
            } else {
                Debug.LogWarning("There is already a terrain tile for (" + ttd.x + ", " + ttd.y +
                                 "). Not loading another one.");
            }
        }
    }

    private void LoadWalls(WallData[] wallDatas) {
        foreach (WallData wd in wallDatas) {
            Wall wall = PlaceWall(wd.x, wd.y, wd.Direction, false);
            if (!string.IsNullOrEmpty(wd.coverFrontUuid))
                wall.CoverFront = WallCoverPresets.Instance.GetWallCoverPreset(new Guid(wd.coverFrontUuid));
            if (!string.IsNullOrEmpty(wd.coverBackUuid))
                wall.CoverBack = WallCoverPresets.Instance.GetWallCoverPreset(new Guid(wd.coverBackUuid));
        }
        UpdateRooms();
    }

    private void LoadFloorTiles(FloorTileData[] floorTileDatas) {
        int width = terrainTiles.GetLength(1);
        int height = terrainTiles.GetLength(0);
        floorTiles = new FloorTile[1, height, width];
        //TODO: Floor levels
        foreach (FloorTileData ftd in floorTileDatas) {
            if (floorTiles[0, ftd.y, ftd.x] == null) {
                try {
                    FloorPreset preset = FloorPresets.Instance.GetFloorPreset(new Guid(ftd.floorGuid));
                    if (preset != null) {
                        PlaceFloor(ftd.x, ftd.y, preset);
                    } else {
                        Debug.LogWarning("No floor preset for GUID " + ftd.floorGuid + ". Not loading floor at (" +
                                         ftd.x + ", " + ftd.y + ").");
                    }
                } catch (Exception e) {
                    Debug.LogError("Exception when trying to load floor at (" + ftd.x + ", " + ftd.y +
                                   ")! Not loading floor.");
                    Debug.LogException(e);
                }
            } else {
                Debug.LogWarning("There is already a floor tile for (" + ftd.x + ", " + ftd.y +
                                 "). Not loading another one.");
            }
        }
    }

    private void LoadPropertyObjects(PropertyObjectData[] propertyObjectDatas) {
        foreach (PropertyObjectData pod in propertyObjectDatas) {
            try {
                FurniturePreset preset = FurniturePresets.Instance.GetFurniturePreset(new Guid(pod.furnitureGuid));
                if (preset != null) {
                    PlacePropertyObject(pod.id, pod.x, pod.y, pod.GetObjectRotation(), preset, pod.skin);
                } else {
                    Debug.LogWarning("No furniture preset for GUID " + pod.furnitureGuid +
                                     ". Not loading property object " + pod.id + ".");
                }
            } catch (Exception e) {
                Debug.LogError("Exception when trying to load property object with id " + pod.id +
                               "! Not loading property object.");
                Debug.LogException(e);
            }
        }
    }

    private void LoadPonies(GamePonyData[] ponyDatas, HouseholdData householdData) {
        // Pony loading currently depends completely on the household.
        // This method should be replaced when non-household ponies are implemented.
        if (householdData == null) {
            ModeController.Instance.LockLiveMode(true);
            return;
        }
        List<Pony> ponies = new List<Pony>();
        foreach (GamePonyData ponyData in ponyDatas) {
            PonyInfoData ponyInfo = householdData.ponies.First(householdPony => householdPony.uuid == ponyData.uuid);
            Pony pony = Instantiate(Prefabs.Instance.ponyPrefab);
            pony.InitInfo(new Guid(ponyData.uuid), ponyInfo.ponyName, ponyInfo.Race, ponyInfo.Gender, ponyInfo.Age);
            pony.InitGamePony(ponyData.x, ponyData.y, new Needs(ponyData.needs), ponyData.actionQueue);
            ponies.Add(pony);
        }
        this.ponies = ponies;
        household = new Household(householdData.householdName, householdData.money, ponies);
    }

    public PropertyData GetPropertyData() {
        return new PropertyData(id,
            propertyName,
            description,
            streetName,
            propertyType == PropertyType.RESIDENTIAL ? 0 : 1,
            time,
            CreateTerrainTileDataArray(terrainTiles),
            CreateFloorTileDataArray(floorTiles),
            CreateWallDataArray(walls),
            CreateRoofDataArray(roofs),
            CreatePropertyObjectDataArray(propertyObjects),
            ponies.Select(pony => pony.GetGamePonyData()).ToArray(),
            household?.GetHouseholdData());
    }

    private TerrainTileData[] CreateTerrainTileDataArray(TerrainTile[,] terrainTiles) {
        TerrainTileData[] terrainTileDataArr = new TerrainTileData[terrainTiles.Length];
        for (int y = 0; y < terrainTiles.GetLength(0); y++) {
            for (int x = 0; x < terrainTiles.GetLength(1); x++) {
                if (terrainTiles[y, x] != null) {
                    terrainTileDataArr[y * terrainTiles.GetLength(1) + x] = terrainTiles[y, x].GetTerrainTileData();
                }
            }
        }
        return terrainTileDataArr;
    }

    private FloorTileData[] CreateFloorTileDataArray(FloorTile[,,] floorTiles) {
        int floorCount = GetNumberOfFloors();
        FloorTileData[] floorTileDataArr = new FloorTileData[floorCount];
        int index = 0;
        for (int h = 0; h < floorTiles.GetLength(0); h++) {
            for (int y = 0; y < floorTiles.GetLength(1); y++) {
                for (int x = 0; x < floorTiles.GetLength(2); x++) {
                    if (floorTiles[h, y, x] != null) {
                        floorTileDataArr[index++] = floorTiles[h, y, x].GetFloorTileData();
                    }
                }
            }
        }
        return floorTileDataArr;
    }

    private int GetNumberOfFloors() {
        int floorCount = 0;
        for (int h = 0; h < floorTiles.GetLength(0); h++) {
            for (int y = 0; y < floorTiles.GetLength(1); y++) {
                for (int x = 0; x < floorTiles.GetLength(2); x++) {
                    if (floorTiles[h, y, x] != null) {
                        floorCount++;
                    }
                }
            }
        }
        return floorCount;
    }

    private WallData[] CreateWallDataArray(Dictionary<TileBorder, Wall> walls) {
        return walls.Values.Select(wall => wall.GetWallData()).ToArray();
    }

    private RoofData[] CreateRoofDataArray(List<Roof> roofs) {
        RoofData[] roofDataArr = new RoofData[roofs.Count];
        for (int i = 0; i < roofs.Count; i++) {
            roofDataArr[i] = roofs[i].GetRoofData();
        }
        return roofDataArr;
    }

    private PropertyObjectData[] CreatePropertyObjectDataArray(List<PropertyObject> propertyObjects) {
        PropertyObjectData[] propertyObjectDataArr = new PropertyObjectData[propertyObjects.Count];
        for (int i = 0; i < propertyObjects.Count; i++) {
            propertyObjectDataArr[i] = propertyObjects[i].GetPropertyObjectData();
        }
        return propertyObjectDataArr;
    }

    /// <summary>
    /// Get all property objects that have a tile overlapping the given position.
    /// </summary>
    /// <param name="x">The X coordinate of the tile.</param>
    /// <param name="y">The Y coordinate of the tile.</param>
    /// <returns>A list of all PropertyObjects with a tile overlapping the given X and Y.</returns>
    public List<PropertyObject> GetObjectsOnTile(int x, int y) {
        return GetObjectsOnTiles(new Vector2Int[] {new Vector2Int(x, y)});
    }

    /// <summary>
    /// Get all property objects that have a tile overlapping one of the given positions.
    /// </summary>
    /// <param name="tiles">The coordinates of the tiles.</param>
    /// <returns>A list of all PropertyObjects with a tile overlapping at least one of the given positions.</returns>
    public List<PropertyObject> GetObjectsOnTiles(Vector2Int[] tiles) {
        //This might come with a performance overhead when there are a lot of objects.
        //If performance becomes an issue, we could remember all overlapping objects inside each terrain tile.
        List<PropertyObject> objectsOnTiles = new List<PropertyObject>();
        foreach (PropertyObject propertyObject in propertyObjects) {
            foreach (Vector2Int occupiedTile in propertyObject.GetOccupiedTiles()) {
                bool overlaps = false;
                foreach (Vector2Int tile in tiles) {
                    if (occupiedTile.x == tile.x && occupiedTile.y == tile.y) {
                        objectsOnTiles.Add(propertyObject);
                        overlaps = true;
                        break;
                    }
                }
                if (overlaps)
                    break;
            }
        }
        return objectsOnTiles;
    }

    /// <summary>
    /// Get all property objects that have two tiles that overlap one of the given borders.
    /// </summary>
    public List<PropertyObject> GetObjectsOnBorders(IEnumerable<TileBorder> borders) {
        TileBorder[] tileBorders = borders as TileBorder[] ?? borders.ToArray();
        List<PropertyObject> objectsOnBorders = new List<PropertyObject>();

        foreach (PropertyObject propertyObject in propertyObjects) {
            HashSet<TileBorder> occupiedBorders = GetOccupiedBorders(propertyObject.GetOccupiedTiles());
            if (occupiedBorders.Intersect(tileBorders).Any()) {
                objectsOnBorders.Add(propertyObject);
            }
        }

        return objectsOnBorders;
    }

    /// <summary>
    /// Get all walls that are inside the given array of tiles.
    /// This includes only the inner borders (those between the tiles).
    /// </summary>
    public List<Wall> GetOccupiedWalls(IEnumerable<Vector2Int> tiles) {
        Vector2Int[] tilesArray = tiles as Vector2Int[] ?? tiles.ToArray();
        HashSet<TileBorder> borders = GetOccupiedBorders(tilesArray);
        List<Wall> occupiedWalls = new List<Wall>();

        foreach (Wall wall in walls.Values) {
            if (borders.Any(border => border.Equals(wall.TileBorder))) {
                occupiedWalls.Add(wall);
            }
        }

        return occupiedWalls;
    }
    
    /// <summary>
    /// Get all tile borders that are occupied by the given array of tiles.
    /// This includes only the inner borders (those between the tiles).
    /// </summary>
    private HashSet<TileBorder> GetOccupiedBorders(Vector2Int[] tiles) {
        HashSet<TileBorder> tileBorders = new HashSet<TileBorder>();
        
        foreach (Vector2Int tile in tiles) {
            if (tiles.Contains(new Vector2Int(tile.x - 1, tile.y))) {
                tileBorders.Add(new TileBorder(tile.x, tile.y, WallDirection.NorthWest));
            }
            if (tiles.Contains(new Vector2Int(tile.x, tile.y - 1))) {
                tileBorders.Add(new TileBorder(tile.x, tile.y, WallDirection.NorthEast));
            }
        }
        
        return tileBorders;
    }
    
    /// <summary>
    /// Returns true if every border in the given collection contains a wall.
    /// False if one or more items to not have a matching walls.
    /// </summary>
    public bool AllBordersContainWalls(IEnumerable<TileBorder> tileBorders) {
        return tileBorders.All(tileBorder => GetWall(tileBorder) != null);
    }
    
    public bool CanRemoveWalls(List<TileBorder> wallPositions) {
        foreach (PropertyObject propertyObject in propertyObjects) {
            foreach (TileBorder requiredWallBorder in propertyObject.GetRequiredWallBorders()) {
                if (wallPositions.Contains(requiredWallBorder)) {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Returns a map that specifies which tiles have an object on them.
    /// All free tiles are represented by a 0. All occupied tiles are represented by a -1.
    /// </summary>
    public int[,] GetTileOccupancyMap() {
        int[,] occupancyMap = new int[TerrainHeight, TerrainWidth];
        foreach (PropertyObject propertyObject in propertyObjects) {
            foreach (Vector2Int occupiedTile in propertyObject.GetOccupiedTiles()) {
                occupancyMap[occupiedTile.y, occupiedTile.x] = -1;
            }
        }
        return occupancyMap;
    }

    [CanBeNull]
    public FloorTile GetFloorTile(int x, int y) {
        //TODO: Add floor level
        return floorTiles[0, y, x];
    }

    public TerrainTile GetTerrainTile(int x, int y) {
        return terrainTiles[y, x];
    }
    
    public IActionProvider GetPropertyObject(int objectId) {
        return propertyObjects.Find(propertyObject => propertyObject.id == objectId);
    }
    
    public Pony GetPony(Guid uuid) => ponies.Find(pony => pony.uuid == uuid);

    public bool WallExists(TileBorder tileBorder) {
        return walls.ContainsKey(tileBorder);
    }

    [CanBeNull]
    public Wall GetWall(TileBorder tileBorder) {
        return WallExists(tileBorder) ? walls[tileBorder] : null;
    }

    public IEnumerable<Wall> GetWalls(IEnumerable<TileBorder> tileBorders) {
        return tileBorders.Select(GetWall).Where(wall => wall != null);
    }

    /// <summary>
    /// Attempt to get a loop with the given wallside.
    /// This is used by the wallcover fill tool to paint whole rooms.
    /// Returns the list of walls in the loop, or null if the loop was incomplete.
    /// </summary>
    [CanBeNull]
    public List<WallSide> GetWallSideLoop(WallSide start) {
        List<WallSide> wallList = new List<WallSide>();
        return AddNextWallInLoop(start.wall, start.wall, start.front, wallList) ? wallList : null;
    }

    private bool AddNextWallInLoop(Wall start, Wall current, bool front, List<WallSide> wallList) {
        wallList.Add(new WallSide(current, front));
        TileBorder currentBorder = current.TileBorder;
        Wall nextWall;
        bool nextFront;
        
        nextWall = GetWall(currentBorder.GetRightBorder(front));
        if (nextWall != null) {
            nextFront = front && current.Direction == WallDirection.NorthWest || !front && current.Direction == WallDirection.NorthEast;
        } else {
            nextWall = nextWall ?? GetWall(currentBorder.GetForwardBorder(front));
            if (nextWall != null) {
                nextFront = front;
            } else {
                nextWall = nextWall ?? GetWall(currentBorder.GetLeftBorder(front));
                if (nextWall != null) {
                    nextFront = front && current.Direction == WallDirection.NorthEast || !front && current.Direction == WallDirection.NorthWest;
                } else
                    return false;
            }
        }
        
        if (nextWall == start) {
            return true;
        }
        if (wallList.Any(side => side.wall == nextWall)) {
            return false;
        }
        if (wallList.Count > 2000) {
            Debug.LogWarning("Very large room. Canceling wall loop calculation.");
            return false;
        }
        
        return AddNextWallInLoop(start, nextWall, nextFront, wallList);
    }

    /// <summary>
    /// Returns the room that the given tile is part of, if any.
    /// Returns null if the tile is not inside a room.
    /// </summary>
    [CanBeNull]
    public Room GetRoom(Vector2Int tile) {
        return Rooms.FirstOrDefault(room => room.tiles.Contains(tile));
    }
    
    /// <summary>
    /// Returns true if the given tile is inside a room.
    /// </summary>
    public bool IsInsideRoom(Vector2Int tile) {
        return GetRoom(tile) != null;
    }

    /// <summary>
    /// Update the rooms list.
    /// This method should be called each time walls are added or removed to the property.
    /// The add/remove wall functions have a parameter for doing this automatically. 
    /// </summary>
    public void UpdateRooms() {
        int[,] roomMap = new int[TerrainHeight, TerrainWidth];
        int roomNumber = 0;
        Vector2Int? nextRoomless = FindNextRoomlessTile(roomMap);
        while (nextRoomless != null) {
            MarkConnectedTiles(roomMap, nextRoomless.Value, ++roomNumber);
            nextRoomless = FindNextRoomlessTile(roomMap);
        }

        Rooms = new Room[roomNumber - 1];
        for (int i = 0; i < Rooms.Length; i++) {
            Rooms[i] = new Room();
        }
        for (int y = 0; y < TerrainHeight; y++) {
            for (int x = 0; x < TerrainWidth; x++) {
                if (roomMap[y, x] > 1) {
                    Rooms[roomMap[y, x] - 2].tiles.Add(new Vector2Int(x, y));
                }
            }
        }
        
        WallVisibilityController.Instance.UpdateWallVisibility();
    }

    private Vector2Int? FindNextRoomlessTile(int[,] roomMap) {
        for (int y = 0; y < TerrainHeight; y++) {
            for (int x = 0; x < TerrainWidth; x++) {
                if (roomMap[y, x] == 0) {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }

    private void MarkConnectedTiles(int[,] roomMap, Vector2Int tile, int roomNumber) {
        roomMap[tile.y, tile.x] = roomNumber;
        foreach (Vector2Int neighbour in GetConnectedTiles(tile)) {
            if (roomMap[neighbour.y, neighbour.x] == 0) {
                MarkConnectedTiles(roomMap, neighbour, roomNumber);
            }
        }
    }
    
    private List<Vector2Int> GetConnectedTiles(Vector2Int tile) {
        List<Vector2Int> tiles = new List<Vector2Int>(4);
        if (tile.x > 0 && !WallExists(new TileBorder(tile.x, tile.y, WallDirection.NorthWest)))
            tiles.Add(new Vector2Int(tile.x - 1, tile.y));
        if (tile.x < TerrainWidth - 1 && !WallExists(new TileBorder(tile.x + 1, tile.y, WallDirection.NorthWest)))
            tiles.Add(new Vector2Int(tile.x + 1, tile.y));
        if (tile.y > 0 && !WallExists(new TileBorder(tile.x, tile.y, WallDirection.NorthEast)))
            tiles.Add(new Vector2Int(tile.x, tile.y - 1));
        if (tile.y < TerrainHeight - 1 && !WallExists(new TileBorder(tile.x, tile.y + 1, WallDirection.NorthEast)))
            tiles.Add(new Vector2Int(tile.x, tile.y + 1));
        return tiles;
    }
}

}