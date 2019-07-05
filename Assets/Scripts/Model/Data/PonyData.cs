using System;
using Model.Ponies;

namespace Model.Data {

[Serializable]
public class PonyData {
    public string ponyName;
    public int race;
    public int gender;
    public int age;
    public NeedsData needs;

    public PonyRace Race => (PonyRace) race;
    public Gender Gender => (Gender) gender;
    public PonyAge Age => (PonyAge) age;

    public PonyData(string ponyName, int race, int gender, int age) :
        this(ponyName, race, gender, age, new NeedsData()) { }

    public PonyData(string ponyName, int race, int gender, int age, NeedsData needs) {
        this.ponyName = ponyName;
        this.race = race;
        this.gender = gender;
        this.age = age;
        this.needs = needs;
    }

    [Serializable]
    public class NeedsData {
        public float hunger;
        public float energy;
        public float comfort;
        public float fun;
        public float hygiene;
        public float social;
        public float bladder;
        public float room;

        public NeedsData() : this(0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f) { }

        public NeedsData(float hunger, float energy, float comfort, float fun, float hygiene, float social,
            float bladder, float room) {
            this.hunger = hunger;
            this.energy = energy;
            this.comfort = comfort;
            this.fun = fun;
            this.hygiene = hygiene;
            this.social = social;
            this.bladder = bladder;
            this.room = room;
        }
    }
}

}