using System.Collections.Generic;
using Model.Data;

namespace Model.Ponies {

public class Household {
    public string householdName;
    public int money;
    public readonly List<Pony> ponies;

    public Household(string householdName, int money, List<Pony> ponies) {
        this.householdName = householdName;
        this.money = money;
        this.ponies = ponies;
    }

    public HouseholdData GetHouseholdData() {
        PonyInfoData[] ponyDatas = new PonyInfoData[ponies.Count];
        for (int i = 0; i < ponies.Count; i++) {
            ponyDatas[i] = ponies[i].GetPonyInfoData();
        }
        return new HouseholdData(householdName, money, ponyDatas);
    }
}

}