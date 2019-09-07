using System;
using System.Collections.Generic;
using System.Linq;
using Model.Ponies;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Scripts.Proxies {

public class PonyProxy {
    private readonly Pony pony;

    [MoonSharpHidden]
    public PonyProxy(Pony pony) {
        this.pony = pony;
    }

    public string uuid => pony.uuid.ToString();

    public string name {
        get => pony.ponyName;
        set => pony.ponyName = value;
    }
    
    public int gender {
        get => (int) pony.gender;
        set => pony.gender = Enum.IsDefined(typeof(Gender), value) ? (Gender) value : Gender.Female;
    }
    
    public int race {
        get => (int) pony.race;
        set => pony.race = Enum.IsDefined(typeof(PonyRace), value) ? (PonyRace) value : PonyRace.Unicorn;
    }
    
    public int age {
        get => (int) pony.age;
        set => pony.age = Enum.IsDefined(typeof(PonyAge), value) ? (PonyAge) value : PonyAge.Adult;
    }
    
    public Vector2Wrapper position {
        get {
            Vector3 pos = pony.transform.position;
            return new Vector2Wrapper(pos.x, pos.z);
        }
        set {
            Transform transform = pony.transform;
            Vector3 pos = transform.position;
            transform.position = value != null ? new Vector3(value.x, pos.y, value.y) : new Vector3(0, pos.y, 0);
        }
    }

    public Vector2Wrapper tilePosition {
        get => new Vector2Wrapper(pony.TilePosition);
        set => pony.TilePosition = value.GetVector2Int();
    }
    
    public float rotation {
        get => pony.transform.eulerAngles.y;
        set {
            Transform transform = pony.transform;
            Vector3 euler = transform.eulerAngles;
            euler.y = value;
            transform.eulerAngles = euler;
        }
    }

    public float hunger {
        get => pony.needs.Hunger;
        set => pony.needs.Hunger = value;
    }
    
    public float energy {
        get => pony.needs.Energy;
        set => pony.needs.Energy = value;
    }
    
    public float comfort {
        get => pony.needs.Comfort;
        set => pony.needs.Comfort = value;
    }
    
    public float fun {
        get => pony.needs.Fun;
        set => pony.needs.Fun = value;
    }
    
    public float hygiene {
        get => pony.needs.Hygiene;
        set => pony.needs.Hygiene = value;
    }
    
    public float social {
        get => pony.needs.Social;
        set => pony.needs.Social = value;
    }
    
    public float bladder {
        get => pony.needs.Bladder;
        set => pony.needs.Bladder = value;
    }
    
    public float room {
        get => pony.needs.Room;
        set => pony.needs.Room = value;
    }

    public bool setWalkTarget(Vector2Wrapper target) => pony.SetWalkTarget(target.GetVector2Int());

    public bool setWalkTargetToNearest(IEnumerable<Vector2Wrapper> targets) =>
        pony.SetWalkTargetToNearest(targets.Select(target => target.GetVector2Int()));
    
    public bool setWalkTargetNextTo(Vector2Wrapper target) => pony.SetWalkTargetNextTo(target.GetVector2Int());
    
    public void clearWalkTarget() => pony.ClearWalkTarget();

    public Vector2Wrapper walkTarget => pony.WalkTarget != null ? new Vector2Wrapper(pony.WalkTarget.Value) : null;

    public bool walkingFailed => pony.WalkingFailed;

    public bool walkTo(Vector2Wrapper target) {
        if (position.Equals(target))
            return true;
        if (!Equals(walkTarget, target))
            setWalkTarget(target);
        return false;
    }

    public bool walkToNearest(Vector2Wrapper[] targets) {
        if (targets.Any(target => target.Equals(position)))
            return true;
        if (!targets.Any(target => Equals(walkTarget, target)))
            setWalkTargetToNearest(targets);
        return false;
    }

    protected bool Equals(PonyProxy other) {
        return Equals(pony, other.pony);
    }

    public override bool Equals(object obj) {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((PonyProxy) obj);
    }

    public override int GetHashCode() {
        return (pony != null ? pony.GetHashCode() : 0);
    }
}

}