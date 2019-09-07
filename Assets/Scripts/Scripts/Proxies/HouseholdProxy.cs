using System.Collections.Generic;
using System.Linq;
using Controllers.Singletons;
using Model.Ponies;
using MoonSharp.Interpreter;

namespace Scripts.Proxies {

class HouseholdProxy {
    private Household household;

    [MoonSharpHidden]
    public HouseholdProxy(Household household) {
        this.household = household;
    }
    
    public string name {
        get => household.householdName;
        set => household.householdName = value;
    } 

    public int funds {
        get => MoneyController.Instance.Funds;
        set => MoneyController.Instance.SetFunds(value);
    }

    public Dictionary<string, Pony> ponies => household.ponies.Values.ToDictionary(pony => pony.uuid.ToString());
}

}
