using Assets.Scripts.Util;
using Controllers.Global.Settings;

namespace Controllers.Global {
/// <summary>
/// Controller class for managing game settings.
/// This is a global singleton that is created on startup by the GameController. It is accessible in every scene.
/// </summary>
public class SettingsController : SingletonMonoBehaviour<SettingsController> {
    public readonly BoolSetting twelveHourClock = new BoolSetting("TwelveHourClock", false);
    public readonly BoolSetting discordIntegration = new BoolSetting("DiscordIntegration", false);
    public readonly EnumSetting<FreeWillOption> freeWillSelectedPony =
        new EnumSetting<FreeWillOption>("FreeWillSelectedPony", FreeWillOption.Full);
    public readonly EnumSetting<FreeWillOption> freeWillHousehold =
        new EnumSetting<FreeWillOption>("FreeWillHousehold", FreeWillOption.Full);

    private new void Awake() {
        base.Awake();
        twelveHourClock.Load();
        discordIntegration.Load();
        freeWillSelectedPony.Load();
        freeWillHousehold.Load();
    }
}
}