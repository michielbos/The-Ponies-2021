using System;

namespace Model.Data {

[Serializable]
public class GamePonyData {
    public string uuid;
    public float x;
    public float y;
    public NeedsData needs;
    public PonyActionData[] actionQueue;

    /// <summary>
    /// Constructor for creating a GamePonyData with all fields.
    /// </summary>
    public GamePonyData(string uuid, float x, float y, NeedsData needs, PonyActionData[] actionQueue) {
        this.uuid = uuid;
        this.x = x;
        this.y = y;
        this.needs = needs;
        this.actionQueue = actionQueue;
    }
    
    /// <summary>
    /// Shorter constructor that fills optional fields with default data.
    /// </summary>
    public GamePonyData(string uuid, float x, float y) {
        this.uuid = uuid;
        this.x = x;
        this.y = y;
        needs = new NeedsData();
        actionQueue = new PonyActionData[0];
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