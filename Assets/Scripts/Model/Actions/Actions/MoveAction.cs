using Model.Ponies;
using UnityEngine;

namespace Model.Actions.Actions {

public class MoveAction : PonyAction {
    public MoveAction(Pony pony, string name) : base(pony, name) { }
    public override void Tick() {
        Debug.Log("Moving...");
        if (canceled) {
            Finish();
        }
    }
}

}