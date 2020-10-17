namespace ThePoniesBehaviour {

public static class BehaviourManager {
    public static void LoadBehaviour() {
        PonyActions.RegisterActionProviders();
        Model.Behaviours.BehaviourManager.AddObjectBehaviourProvider(new ObjectBehaviours());
    }
}

}