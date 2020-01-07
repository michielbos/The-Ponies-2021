using System.Collections.Generic;
using System.Linq;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using UnityEngine;
using Util;

namespace ThePoniesBehaviour.Actions {

public class SeatActionProvider : IObjectActionProvider {
    private const string SitIdentifier = "seatSit";
    private const string ChairType = "chair";
    private const string CouchType = "couch";

    public IEnumerable<ObjectAction> GetActions(Pony pony, PropertyObject target) {
        if (target.Type == ChairType || target.Type == CouchType) {
            return new[] {new SitAction(pony, target)};
        }
        return new ObjectAction[0];
    }

    public ObjectAction LoadAction(string identifier, Pony pony, PropertyObject target) {
        return identifier == SitIdentifier ? new SitAction(pony, target) : null;
    }

    private class SitAction : ObjectAction {
        public SitAction(Pony pony, PropertyObject target) : base(SitIdentifier, pony, target, "Sit") { }

        protected override void OnStart() {
            data["targetSeat"] = GetClosestFreeSeatTile();
        }

        public override bool Tick() {
            if (target.users.Contains(pony))
                return HandleSitting();
            return HandleMoveToSeat();
        }

        /// <summary>
        /// Find a seat tile and move to the seat.
        /// </summary>
        /// <returns>True if the action failed.</returns>
        private bool HandleMoveToSeat() {
            if (canceled)
                return true;

            // Quit if we have no target seat.
            Vector2Int? targetSeat = data["targetSeat"] as Vector2Int?;
            if (targetSeat == null)
                return true;

            // Find a new seat if the old one was moved.
            if (!target.GetOccupiedTiles().Contains(targetSeat.Value)) {
                if (!FindNewSeat())
                    return true;
            }

            // If the current target seat is taken, search for a new seat.
            // If there is none, cancel the action.
            foreach (Pony user in target.users) {
                if (user.TilePosition == targetSeat) {
                    if (!FindNewSeat())
                        return true;
                    break;
                }
            }

            // Walk to the seat.
            if (!pony.WalkTo(targetSeat.Value.GetNeighbourTile(target.Rotation))) {
                return pony.WalkingFailed;
            }
            
            target.users.Add(pony);
            pony.TilePosition = targetSeat.Value;

            return false;
        }

        /// <summary>
        /// Find a new target seat.
        /// </summary>
        /// <returns>Returns true if a new seat was found.</returns>
        private bool FindNewSeat() {
            Vector2Int? targetSeat = GetClosestFreeSeatTile();
            data["targetSeat"] = targetSeat;
            return targetSeat != null;
        }

        /// <summary>
        /// Return the closest free seat tile of this chair/couch.
        /// Returns null if there is none available.
        /// </summary>
        private Vector2Int? GetClosestFreeSeatTile() {
            // Chairs have only one seat tile.
            if (target.Type == ChairType)
                return target.users.Count == 0 ? target.TilePosition : (Vector2Int?) null;

            // Get all free seats.
            IEnumerable<Vector2Int> freeSeats = target.GetOccupiedTiles()
                .Except(target.users.Select(user => user.TilePosition));

            // Get the nearest (reachable) tile in front of a seat.
            Path pathToNearest = Pathfinding.PathToNearest(pony.TilePosition, 
                freeSeats.Select(tile => tile.GetNeighbourTile(target.Rotation)));
            if (pathToNearest == null)
                return null;
            Vector2Int nearestFrontTile = pathToNearest.Destination;

            // Return the tile behind the front tile, which is the seat tile.
            return nearestFrontTile.GetNeighbourTile(target.Rotation.Inverse());
        }

        /// <summary>
        /// Handle the actual sitting on the seat.
        /// </summary>
        /// <returns>True when the action was finished.</returns>
        private bool HandleSitting() {
            Vector2Int seatPosition = pony.TilePosition;
            Vector2Int cancelPosition = seatPosition.GetNeighbourTile(target.Rotation);
            if (canceled && PropertyController.Property.CanPassBorder(pony.TilePosition, cancelPosition)) {
                target.users.Remove(pony);
                pony.TilePosition = cancelPosition;
                return true;
            }
            // Hourly comfort gain is 10% * comfort score.
            pony.needs.Comfort += 0.1f * target.preset.needStats.comfort / 60f;
            return false;
        }

        protected override void OnFinish() {
            if (target != null) {
                target.users.Remove(pony);
            }
        }
    }
}

}