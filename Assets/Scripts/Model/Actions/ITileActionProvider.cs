using System.Collections.Generic;
using Model.Ponies;
using Model.Property;

namespace Model.Actions {

public interface ITileActionProvider {
    IEnumerable<TileAction> GetActions(Pony pony, TerrainTile target);

    TileAction LoadAction(string identifier, Pony pony, TerrainTile target);
}

}