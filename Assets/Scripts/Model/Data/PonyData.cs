using Model.Ponies;

namespace Model.Data {

[System.Serializable]
public class PonyData {
    public string ponyName;
    public int race;
    public int gender;
    public int age;

    public PonyRace Race => (PonyRace) race;
    public Gender Gender => (Gender) gender;
    public PonyAge Age => (PonyAge) age;

    public PonyData(string ponyName, int race, int gender, int age) {
        this.ponyName = ponyName;
        this.race = race;
        this.gender = gender;
        this.age = age;
    }
}

}