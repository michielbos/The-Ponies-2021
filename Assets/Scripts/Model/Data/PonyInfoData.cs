using System;
using Model.Ponies;

namespace Model.Data {

[Serializable]
public class PonyInfoData {
    public string uuid;
    public string ponyName;
    public int race;
    public int gender;
    public int age;

    public PonyRace Race => (PonyRace) race;
    public Gender Gender => (Gender) gender;
    public PonyAge Age => (PonyAge) age;

    public PonyInfoData(string uuid, string ponyName, int race, int gender, int age) {
        this.uuid = uuid;
        this.ponyName = ponyName;
        this.race = race;
        this.gender = gender;
        this.age = age;
    }
}

}