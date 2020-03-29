using System.Collections.Generic;
using Model;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using ThePoniesBehaviour.Extensions;
using UnityEngine;
using Util;

namespace ThePoniesBehaviour.Actions {

/// <summary>
/// Provides actions for toilets.
/// </summary>
public class FoodActionProvider : IObjectActionProvider {
    //board: 0000418e-663a-c38b-4deb-182df032981e
    //pancakes: 000001ee-ffd5-c794-1cda-cf42be44ae72
    private const string EatIdentifier = "foodEat";
    private const string FoodType = "food";

    public IEnumerable<ObjectAction> GetActions(Pony pony, PropertyObject target) {
        if (target.Type == FoodType) {
            return new[] {new EatAction(pony, target)};
        }
        return new ObjectAction[0];
    }

    public ObjectAction LoadAction(string identifier, Pony pony, PropertyObject target) {
        return identifier == EatIdentifier ? new EatAction(pony, target) : null;
    }

    private class EatAction : ObjectAction {
        public EatAction(Pony pony, PropertyObject target) : base(EatIdentifier, pony, target, "Eat") { }

        public override bool Tick() {
            if (!target.users.Contains(pony))
                return MoveToFood();
            return EatFood();
        }

        /// <summary>
        /// Move to the food.
        /// </summary>
        /// <returns>True if the action failed.</returns>
        private bool MoveToFood() {
            ActionResult walkResult = this.WalkToClosestNeighbour(target.TilePosition, maxUsers: 1);
            if (walkResult == ActionResult.Busy)
                return false;
            if (walkResult == ActionResult.Failed)
                return true;

            target.users.Add(pony);
            pony.PickUp(target);

            return false;
        }

        /// <summary>
        /// Handle eating of the food.
        /// When there is no food left, the plate is dropped.
        /// </summary>
        /// <returns>True when the action was finished.</returns>
        private bool EatFood() {
            if (canceled) {
                pony.DropHoofItem();
                return true;
            }

            DataMap data = target.data;

            const float timeToEat = 30f;
            float foodLeft = data.GetFloat("foodLeft", 1f);
            float nutrition = data.GetFloat("nutrition", 0.60f);

            pony.needs.Hunger += nutrition / timeToEat;
            foodLeft -= 1f / timeToEat;
            
            data.Put("foodLeft", foodLeft);

            if (foodLeft <= 0) {
                pony.DropHoofItem();
                return true;
            }

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