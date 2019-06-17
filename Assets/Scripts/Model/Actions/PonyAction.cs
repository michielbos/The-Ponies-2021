using Model.Ponies;

namespace Model.Actions {

public abstract class PonyAction {
    public Pony pony;
    public string name;
    public bool active;
    public bool canceled;
    public bool finished;

    protected PonyAction(Pony pony, string name) {
        this.pony = pony;
        this.name = name;
    }

    public abstract void Tick();

    public void SetActive() {
        active = true;
    }

    public void Cancel() {
        canceled = true;
    }

    protected void Finish() {
        finished = true;
    }
}

}