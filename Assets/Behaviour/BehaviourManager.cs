using ThePoniesBehaviour.Actions;

namespace ThePoniesBehaviour {

public static class BehaviourManager {
    public static void LoadBehaviour() {
        PonyActions.RegisterActionProviders();
    }
}

}