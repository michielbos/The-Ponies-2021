using System;
using System.Collections.Generic;
using Model.Actions;
using Model.Ponies;
using Model.Property;

namespace ThePoniesBehaviour.Actions {
public class WetSelfActionProvider : ISocialActionProvider {
    private const string WetSelfIdentifier = "wetSelf";
    private const string PuddleUuid = "00000100-0000-0000-0022-000000034601";

    public IEnumerable<SocialAction> GetActions(Pony pony, Pony target) {
        List<SocialAction> actions = new List<SocialAction>(1);
        if (target == pony) {
            actions.Add(new WetSelfAction(WetSelfIdentifier, pony, target, "Wet self"));
        }
        return actions;
    }

    public SocialAction LoadAction(string identifier, Pony pony, Pony target) {
        if (identifier == WetSelfIdentifier)
            return new WetSelfAction(identifier, pony, target, "Wet self");
        return null;
    }

    private class WetSelfAction : SocialAction {
        public WetSelfAction(string identifier, Pony pony, Pony target, string name) : base(identifier,
            pony, target, name) { }

        public override bool Visible => false;

        public override bool Tick() {
            Property property = PropertyController.Property;

            // Only put a puddle if the ground is empty (also prevents stacked puddles)
            if (property.GetObjectsOnTile(pony.TilePosition.x, pony.TilePosition.y).Count == 0) {
                FurniturePreset puddlePreset = FurniturePresets.Instance.GetFurniturePreset(new Guid(PuddleUuid));
                property.PlacePropertyObject(property.GetTerrainTile(pony.TilePosition), ObjectRotation.SouthEast,
                    puddlePreset);
            }
            
            pony.needs.Bladder += 0.1f;
            pony.needs.Hygiene -= 0.2f;

            // Continue until empty.
            return pony.needs.Bladder >= 1f;
        }
    }
}
}