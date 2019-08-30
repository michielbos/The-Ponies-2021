using System;
using Model.Ponies;

namespace Model.Data {

[Serializable]
public class PonyData {
    public string uuid;
    public string ponyName;
    public int race;
    public int gender;
    public int age;
    public GamePonyData gamePony;

    public PonyRace Race => (PonyRace) race;
    public Gender Gender => (Gender) gender;
    public PonyAge Age => (PonyAge) age;

    public PonyData(string uuid, string ponyName, int race, int gender, int age, GamePonyData gamePony) {
        this.uuid = uuid;
        this.ponyName = ponyName;
        this.race = race;
        this.gender = gender;
        this.age = age;
        this.gamePony = gamePony;
    }
}

}