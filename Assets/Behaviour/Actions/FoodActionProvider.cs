using System.Collections.Generic;
using Model;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using ThePoniesBehaviour.Extensions;

namespace ThePoniesBehaviour.Actions {

/// <summary>
/// Provides actions for food items.
/// </summary>
public class FoodActionProvider : IObjectActionProvider {
    //board: 0000418e-663a-c38b-4deb-182df032981e
    //pancakes: 000001ee-ffd5-c794-1cda-cf42be44ae72
    private const string EatIdentifier = "foodEat";
    private const string PutAwayFoodIdentifier = "foodPutAway";
    private const string CleanUpFoodIdentifier = "foodCleanUp";
    private const string FoodType = "food";
    
    private const string DataFoodLeft = "foodLeft";
    private const string DataNutrition = "nutrition";

    public IEnumerable<ObjectAction> GetActions(Pony pony, PropertyObject target) {
        if (target.Type == FoodType) {
            List<ObjectAction> actions = new List<ObjectAction>(2);
            if (target.data.GetFloat(DataFoodLeft, 1f) > 0) {
                actions.Add(new EatAction(pony, target));
            }
            actions.Add(new CleanUpAction(CleanUpFoodIdentifier, pony, target));
            return actions;
        }
        return new ObjectAction[0];
    }

    public ObjectAction LoadAction(string identifier, Pony pony, PropertyObject target) {
        if (identifier == EatIdentifier)
            return new EatAction(pony, target);
        if (identifier == PutAwayFoodIdentifier)
            return new PutAwayAction(PutAwayFoodIdentifier, pony, target);
        if (identifier == CleanUpFoodIdentifier)
            return new CleanUpAction(CleanUpFoodIdentifier, pony, target);
        return null;
    }

    internal class EatAction : ObjectAction {
        
        public EatAction(Pony pony, PropertyObject target) : base(EatIdentifier, pony, target, "Eat") { }

        protected override void OnStart() {
            if (pony.HoofSlot.SlotObject == target && !target.users.Contains(pony)) {
                target.users.Add(pony);
            }
        }

        public override bool Tick() {
            if (!target.users.Contains(pony)) {
                return MoveToFood();
            }
            return EatFood();
        }

        /// <summary>
        /// Move to the food.
        /// </summary>
        /// <returns>True if the action failed.</returns>
        private bool MoveToFood() {
            ActionResult walkResult = this.WalkNextTo(target.TilePosition, maxUsers: 1);
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
                pony.QueueActionFirst(new PutAwayAction(PutAwayFoodIdentifier, pony, target));
                return true;
            }

            DataMap data = target.data;

            const float timeToEat = 30f;
            float foodLeft = data.GetFloat(DataFoodLeft, 1f);
            float nutrition = data.GetFloat(DataNutrition, 0.60f);
            
            if (foodLeft <= 0) {
                pony.QueueActionFirst(new CleanUpAction(CleanUpFoodIdentifier, pony, target));
                return true;
            }

            pony.needs.Hunger += nutrition / timeToEat;
            foodLeft -= 1f / timeToEat;
            
            data.Put(DataFoodLeft, foodLeft);

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