using Model.Ponies;
using UnityEngine;

namespace Model.Actions.Actions {

public class FakeAction : PonyAction {
    public FakeAction(Pony pony, string name) : base(pony, name) { }
    public override void Tick() {
        Debug.Log("fake action");
        Finish();
    }
}

}