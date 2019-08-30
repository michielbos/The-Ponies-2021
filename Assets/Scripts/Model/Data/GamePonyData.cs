using System;

namespace Model.Data {

[Serializable]
public class GamePonyData {
    public float x;
    public float y;
    public NeedsData needs;

    public GamePonyData(float x, float y, NeedsData needs) {
        this.x = x;
        this.y = y;
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