using System;
using System.Collections.Generic;
using System.Linq;
using Model.Data;

namespace Model.Ponies {

public class Household {
    public string householdName;
    public int money;
    public readonly Dictionary<Guid, Pony> ponies;

    public Household(string householdName, int money, Dictionary<Guid, Pony> ponies) {
        this.householdName = householdName;
        this.money = money;
        this.ponies = ponies;
    }

    public HouseholdData GetHouseholdData() {
        PonyInfoData[] ponyDatas = ponies.Values.Select(pony => pony.GetPonyInfoData()).ToArray();
        return new HouseholdData(householdName, money, ponyDatas);
    }
}

}