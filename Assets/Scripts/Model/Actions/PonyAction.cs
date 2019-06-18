using Model.Ponies;

namespace Model.Actions {

public abstract class PonyAction {
    public Pony pony;
    public string name;
    public bool active;
    public bool canceled;
    public bool finished;
    protected int tickCount;

    protected PonyAction(Pony pony, string name) {
        this.pony = pony;
        this.name = name;
    }

    public void TickAction() {
        tickCount++;
        Tick();
    }
    
    public abstract void Tick();

    public void SetActive() {
        active = true;
    }

    public void Cancel() {
        if (!canceled) {
            canceled = true;
            OnCancel();
        }
    }

    protected virtual void OnCancel() {
        // Override for additional cancel behaviour.
    }

    protected void Finish() {
        finished = true;
    }
}

}