namespace Model.Data {

[System.Serializable]
public class HouseholdData {
    public string householdName;
    public int money;
    public PonyInfoData[] ponies;

    public HouseholdData(string householdName, int money, PonyInfoData[] ponies) {
        this.householdName = householdName;
        this.money = money;
        this.ponies = ponies;
    }
}

}