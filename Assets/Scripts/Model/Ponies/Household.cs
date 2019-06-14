using System.Collections.Generic;
using Model.Data;

namespace Model.Ponies {

public class Household {
    public string householdName;
    public int money;
    public readonly List<Pony> ponies;

    public Household(string householdName, List<Pony> ponies) {
        this.householdName = householdName;
        this.ponies = ponies;
    }

    public HouseholdData GetHouseholdData() {
        PonyData[] ponyDatas = new PonyData[ponies.Count];
        for (int i = 0; i < ponies.Count; i++) {
            ponyDatas[i] = ponies[i].GetPonyData();
        }
        return new HouseholdData(householdName, money, ponyDatas);
    }
}

}