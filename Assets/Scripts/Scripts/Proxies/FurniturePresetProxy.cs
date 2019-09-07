using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;

namespace Scripts.Proxies {

public class FurniturePresetProxy {
    private readonly FurniturePreset preset;

    [MoonSharpHidden]
    public FurniturePresetProxy(FurniturePreset preset) {
        this.preset = preset;
    }

    public string name => preset.name;
    public string description => preset.description;
    public int price => preset.price;
    public int category => (int) preset.category;
    public bool pickupable => preset.pickupable;
    public bool sellable => preset.sellable;
    public int placementType => (int) preset.placementType;

    public Vector2Wrapper[] occupiedTiles =>
        preset.occupiedTiles.Select(tile => new Vector2Wrapper(tile.x, tile.y)).ToArray();

    public int hunger => preset.needStats.hunger;
    public int energy => preset.needStats.energy;
    public int comfort => preset.needStats.comfort;
    public int fun => preset.needStats.fun;
    public int hygiene => preset.needStats.hygiene;
    public int social => preset.needStats.social;
    public int bladder => preset.needStats.bladder;
    public int room => preset.needStats.room;

    public int requiredAge => (int) preset.requiredAge;
    public Dictionary<string, string> tags => preset.tags;
}

}