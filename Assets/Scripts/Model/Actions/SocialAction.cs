using Model.Data;
using Model.Ponies;

namespace Model.Actions {

/// <summary>
/// An action that can be invoked on a pony.
/// </summary>
public abstract class SocialAction : PonyAction {
    public readonly Pony target;

    protected SocialAction(string identifier, Pony pony, Pony target, string name) :
        base(identifier, pony, name) {
        this.target = target;
    }

    public override PonyActionData GetData() {
        return new PonyActionData(identifier, tickCount, canceled, target.uuid.ToString(), data.GetDataPairs());
    }
    
    protected internal override bool TargetExists() {
        return target != null;
    }
}

}