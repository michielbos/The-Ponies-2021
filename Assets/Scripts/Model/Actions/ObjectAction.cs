using Model.Data;
using Model.Ponies;
using Model.Property;

namespace Model.Actions {

public abstract class ObjectAction : PonyAction {
    protected readonly PropertyObject target;

    protected ObjectAction(string identifier, Pony pony, PropertyObject target, string name) :
        base(identifier, pony, name) {
        this.target = target;
    }

    protected ObjectAction(string identifier, Pony pony, PropertyObject target, string name, int tickCount, bool canceled)
        : base(identifier, pony, name, tickCount, canceled) {
        this.target = target;
    }

    public override PonyActionData GetData() {
        return new PonyActionData(identifier, tickCount, canceled, target.id);
    }
}

}