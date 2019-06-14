using System.Collections.Generic;

namespace Model.Ponies {

public class Household {
    public string householdName;
    public readonly List<Pony> ponies;

    public Household(string householdName, List<Pony> ponies) {
        this.householdName = householdName;
        this.ponies = ponies;
    }
}

}