using Model.Data;

namespace Model.Ponies {

public class Needs {
    public float hunger;
    public float energy;
    public float comfort;
    public float fun;
    public float hygiene;
    public float social;
    public float bladder;
    public float room;

    public Needs(float hunger, float energy, float comfort, float fun, float hygiene, float social, float bladder,
        float room) {
        this.hunger = hunger;
        this.energy = energy;
        this.comfort = comfort;
        this.fun = fun;
        this.hygiene = hygiene;
        this.social = social;
        this.bladder = bladder;
        this.room = room;
    }

    public Needs(PonyData.NeedsData needsData) : this(needsData.hunger, needsData.energy, needsData.comfort,
        needsData.fun, needsData.hygiene, needsData.social, needsData.bladder, needsData.room) { }

    public PonyData.NeedsData GetNeedsData() {
        return new PonyData.NeedsData(hunger, energy, comfort, fun, hygiene, social, bladder, room);
    }
}

}