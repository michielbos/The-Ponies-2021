namespace Model.Data {

[System.Serializable]
public class HouseholdData {
    public string householdName;
    public int money;
    public PonyData[] ponies;

    public HouseholdData(string householdName, int money, PonyData[] ponies) {
        this.householdName = householdName;
        this.money = money;
        this.ponies = ponies;
    }
}

}