using Assets.Scripts.Util;
using JetBrains.Annotations;
using Model.Ponies;

namespace Assets.Scripts.Controllers {

public class HouseholdController : SingletonMonoBehaviour<HouseholdController> {
    [CanBeNull] public Household Household => PropertyController.Instance.property.household;
    [CanBeNull] public Pony selectedPony; 

    public void SetSelectedPony(int index) {
        if (selectedPony != null) {
            selectedPony.SetSelected(false);
        }
        selectedPony = Household.ponies[index];
        selectedPony.SetSelected(true);
    }
}

}