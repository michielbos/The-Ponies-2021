using Model.Ponies;

namespace Model.Actions.Actions {

/// <summary>
/// An action that can be performed on another pony.
/// These will also add a receiver action to the pony on which the action is performed. (TODO)
/// </summary>
public class SocialAction : PonyAction {
    private readonly Pony target;
    
    public SocialAction(Pony pony, string name, Pony target) : base(pony, name) {
        this.target = target;
    }
    public override void Tick() {
        if (tickCount == 1) {
            if (!pony.SetWalkTargetNextTo(target.TilePosition)) {
                Finish();
            }
        }
        if (!pony.IsWalking) {
            Finish();
        }
    }

    protected override void OnCancel() {
        pony.CancelWalking();
    }
}

}