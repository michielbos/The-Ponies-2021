using Model.Data;
using UnityEngine;

namespace Model.Ponies {

public class Needs {
    private const int TicksPerDay = 24 * 60; // 1440

    private float _hunger;
    private float _energy;
    private float _comfort;
    private float _fun;
    private float _hygiene;
    private float _social;
    private float _bladder;
    private float _room;

    public float Hunger {
        get { return _hunger; }
        set { _hunger = Mathf.Clamp01(value); }
    }

    public float Energy {
        get { return _energy; }
        set { _energy = Mathf.Clamp01(value); }
    }

    public float Comfort {
        get { return _comfort; }
        set { _comfort = Mathf.Clamp01(value); }
    }

    public float Fun {
        get { return _fun; }
        set { _fun = Mathf.Clamp01(value); }
    }

    public float Hygiene {
        get { return _hygiene; }
        set { _hygiene = Mathf.Clamp01(value); }
    }

    public float Social {
        get { return _social; }
        set { _social = Mathf.Clamp01(value); }
    }

    public float Bladder {
        get { return _bladder; }
        set { _bladder = Mathf.Clamp01(value); }
    }

    public float Room {
        get { return _room; }
        set { _room = Mathf.Clamp01(value); }
    }

    public Needs(float hunger, float energy, float comfort, float fun, float hygiene, float social, float bladder,
        float room) {
        Hunger = hunger;
        Energy = energy;
        Comfort = comfort;
        Fun = fun;
        Hygiene = hygiene;
        Social = social;
        Bladder = bladder;
        Room = room;
    }

    public Needs(GamePonyData.NeedsData needsData) : this(needsData.hunger, needsData.energy, needsData.comfort,
        needsData.fun, needsData.hygiene, needsData.social, needsData.bladder, needsData.room) { }

    public GamePonyData.NeedsData GetNeedsData() {
        return new GamePonyData.NeedsData(Hunger, Energy, Comfort, Fun, Hygiene, Social, Bladder, Room);
    }

    public float GetMood() {
        float total = Hunger + Energy + Comfort + Fun + Hunger + Social + Bladder + Room;
        return total / 8f;
    }

    public void ApplyDecay() {
        // WIP minimalistic decay.
        // Every bar drains from 1 to 0 in 24 hours.
        // Comfort and bladder drain in 12 hours.
        // Environment is special.
        Hunger -= 1f / TicksPerDay;
        Energy -= 1f / TicksPerDay;
        Comfort -= 2f / TicksPerDay;
        Fun -= 1f / TicksPerDay;
        Hygiene -= 1f / TicksPerDay;
        Social -= 1f / TicksPerDay;
        Bladder -= 2f / TicksPerDay;
        Room = 0.5f;
    }

    public void SetAll(float value) {
        Hunger = value;
        Energy = value;
        Comfort = value;
        Fun = value;
        Hygiene = value;
        Social = value;
        Bladder = value;
        Room = value;
    }
}

}