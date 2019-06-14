using UnityEngine;

namespace Model.Ponies {

public class Pony: MonoBehaviour {
    public string firstName;
    public PonyRace race;
    public Gender gender;
    public PonyAge age;

    public void Init(string firstName, PonyRace race, Gender gender, PonyAge age) {
        this.firstName = firstName;
        this.race = race;
        this.gender = gender;
        this.age = age;
    }
}

}