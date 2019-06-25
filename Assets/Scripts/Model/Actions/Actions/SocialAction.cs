using Model.Ponies;

namespace Model.Actions.Actions {

/// <summary>
/// An action that can be performed on another pony.
/// These will also add a receiver action to the pony on which the action is performed.
/// </summary>
public class SocialAction : PonyAction {
    private readonly Pony target;
    private SocialReceiveAction receiverAction;
    private bool started;

    public SocialAction(Pony pony, string name, Pony target) : base(pony, name) {
        this.target = target;
    }

    public override void Tick() {
        if (tickCount == 1) {
            receiverAction = new SocialReceiveAction(target, "Receive " + name, this);
            target.QueueAction(receiverAction);
        }

        if (receiverAction.active) {
            if (!started) {
                started = true;
                if (!pony.SetWalkTargetNextTo(target.TilePosition)) {
                    Finish();
                }
            } else if (!pony.IsWalking) {
                Finish();
            }
        }

        if (receiverAction.finished) {
            Finish();
        }
    }

    protected override void OnCancel() {
        receiverAction.Cancel();
        pony.CancelWalking();
    }

    protected override void OnFinish() {
        receiverAction.Finish();
    }
}

}