using Model.Ponies;

namespace Model.Actions.Actions {

public class SocialReceiveAction : PonyAction {
    private readonly SocialAction socialAction;

    public SocialReceiveAction(Pony pony, string name, SocialAction socialAction) : base(pony, name) {
        this.socialAction = socialAction;
    }

    public override void Tick() {
        // The leading social action will handle all behaviour.
    }

    protected override void OnCancel() {
        socialAction.Cancel();
    }
}

}