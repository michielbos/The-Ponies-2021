using Model.Data;
using UnityEngine;

namespace Model.Ponies {

public class Pony: MonoBehaviour {
    public string ponyName;
    public PonyRace race;
    public Gender gender;
    public PonyAge age;

    public void Init(string ponyName, PonyRace race, Gender gender, PonyAge age) {
        this.ponyName = ponyName;
        this.race = race;
        this.gender = gender;
        this.age = age;
    }

    public PonyData GetPonyData() {
        return new PonyData(ponyName, (int) race, (int) gender, (int) age);
    }
}

}