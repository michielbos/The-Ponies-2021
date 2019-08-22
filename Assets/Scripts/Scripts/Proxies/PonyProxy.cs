using System;
using Model.Ponies;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Scripts.Proxies {

public class PonyProxy {
    private Pony pony;

    [MoonSharpHidden]
    public PonyProxy(Pony pony) {
        this.pony = pony;
    }

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
    
    public float x {
        get => pony.transform.position.x;
        set {
            Transform transform = pony.transform;
            Vector3 position = transform.position;
            position.x = value;
            transform.position = position;
        }
    }
    
    public float y {
        get => pony.transform.position.y;
        set {
            Transform transform = pony.transform;
            Vector3 position = transform.position;
            position.z = value;
            transform.position = position;
        }
    }
    
    public int tileX {
        get => pony.TilePosition.x;
        set {
            Vector2Int tilePosition = pony.TilePosition;
            tilePosition.x = value;
            pony.TilePosition = tilePosition;
        }
    }
    
    public int tileY {
        get => pony.TilePosition.y;
        set {
            Vector2Int tilePosition = pony.TilePosition;
            tilePosition.y = value;
            pony.TilePosition = tilePosition;
        }
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
}

}