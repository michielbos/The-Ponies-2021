using Model.Actions;

namespace ThePoniesBehaviour.Actions {

public static class PonyActions {
    public static void RegisterActionProviders() {
        ActionManager.AddObjectActionProviders(new IObjectActionProvider[] {
            new SeatActionProvider(),
            new ToiletActionProvider(),
            new ShowerBathActionProvider(),
            new BedActionProvider(),
            new FoodActionProvider(),
        });
        ActionManager.AddTileActionProviders(new [] {
            new MoveActionProvider()
        });
    }
}

}