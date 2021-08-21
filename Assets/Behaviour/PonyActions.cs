using System.Collections.Generic;
using Model.Actions;
using ThePoniesBehaviour.Actions;

namespace ThePoniesBehaviour {

public static class PonyActions {
    public static void RegisterActionProviders() {
        ActionManager.AddObjectActionProviders(new IObjectActionProvider[] {
            new SeatActionProvider(),
            new ToiletActionProvider(),
            new ShowerBathActionProvider(),
            new BedActionProvider(),
            new FoodActionProvider(),
            new PrepareFoodActionProvider(),
            new StereoActionProvider(), 
        });
        ActionManager.AddTileActionProviders(new [] {
            new MoveActionProvider()
        });
        ActionManager.AddSocialActionProviders(new ISocialActionProvider[0]);
    }
}

}