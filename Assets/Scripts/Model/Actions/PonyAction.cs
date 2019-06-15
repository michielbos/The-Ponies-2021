namespace Model.Actions {

public abstract class PonyAction {
    public string name;

    protected PonyAction(string name) {
        this.name = name;
    }
}

}