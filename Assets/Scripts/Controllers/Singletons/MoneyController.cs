using Assets.Scripts.Util;
using Controllers.Playmode;

namespace Controllers.Singletons {

public class MoneyController : SingletonMonoBehaviour<MoneyController> {
    public bool UseFunds => PropertyController.Instance.property.household != null;
    public int Funds => PropertyController.Instance.property.household?.money ?? 0;

    private void Start() {
        HUDController.GetInstance().UpdateFunds();
    }

    public void ChangeFunds(int change) {
        if (!UseFunds)
            return;
        SetFunds(Funds + change);
    }

    public void SetFunds(int amount) {
        if (!UseFunds)
            return;
        PropertyController.Instance.property.household.money = amount;
        HUDController.GetInstance().UpdateFunds();
    }

    public bool CanAfford(int amount) {
        return !UseFunds || amount <= Funds;
    }
}

}