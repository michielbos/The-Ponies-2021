using Model.Data;
using Model.Ponies;

namespace Model.Actions {

public abstract class PonyAction {
    public readonly string identifier;
    public readonly Pony pony;
    public readonly string name;
    public bool active;
    public bool canceled;
    public bool finished;
    public int tickCount;

    protected PonyAction(string identifier, Pony pony, string name) {
        this.identifier = identifier;
        this.pony = pony;
        this.name = name;
    }
    
    protected PonyAction(string identifier, Pony pony, string name, int tickCount, bool canceled) {
        this.identifier = identifier;
        this.pony = pony;
        this.name = name;
        this.tickCount = tickCount;
        this.canceled = canceled;
        if (tickCount > 0)
            active = true;
    }

    internal void TickAction() {
        tickCount++;
        Tick();
    }
    
    /// <summary>
    /// Called every ingame second to execute this action's behaviour.
    /// Execution will start after the action becomes active.
    /// The frequency is dependent on the game speed and execution will pause when the game is paused.
    /// </summary>
    public abstract void Tick();

    internal void SetActive() {
        if (!active) {
            active = true;
            OnActivated();
        }
    }
    
    protected virtual void OnActivated() {
        // Override for additional activation behaviour.
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
        if (!finished) {
            finished = true;
            OnFinish();
        }
    }
    
    protected virtual void OnFinish() {
        // Override for additional finish behaviour.
    }

    public abstract PonyActionData GetData();
}

}