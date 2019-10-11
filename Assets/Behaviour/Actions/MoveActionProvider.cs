using System.Collections.Generic;
using Model.Actions;
using Model.Ponies;
using Model.Property;

namespace ThePoniesBehaviour.Actions {

public class MoveActionProvider : ITileActionProvider {
    private const string GoHereIdentifier = "goHere";

    public IEnumerable<TileAction> GetActions(Pony pony, TerrainTile target) {
        return new[] {new GoHereAction(pony, target)};
    }

    public TileAction LoadAction(string identifier, Pony pony, TerrainTile target) {
        return identifier == GoHereIdentifier ? new GoHereAction(pony, target) : null;
    }

    private class GoHereAction : TileAction {
        public GoHereAction(Pony pony, TerrainTile target) : base(GoHereIdentifier, pony, target, "Go Here") { }

        // I know this would've been more readable if written out, but I couldn't resist making it a one-liner.
        // I'm sorry.
        public override bool Tick() => canceled || pony.WalkTo(target.TilePosition) || pony.WalkingFailed;
    }
}

}