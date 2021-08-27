using Model.Data;
using Model.Ponies;
using Model.Property;

namespace Model.Actions {
public abstract class ObjectAction : PonyAction {
    public readonly PropertyObject target;

    protected ObjectAction(string identifier, Pony pony, PropertyObject target, string name) :
        base(identifier, pony, name) {
        this.target = target;
    }

    public override PonyActionData GetData() {
        return new PonyActionData(identifier, tickCount, canceled, (int)trigger, target.id, data.GetDataPairs());
    }

    protected internal override bool TargetExists() {
        return target != null;
    }
}
}