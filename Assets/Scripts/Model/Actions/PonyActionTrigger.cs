namespace Model.Actions {
/// <summary>
/// The source that triggered a pony action.
/// </summary>
public enum PonyActionTrigger {
    /// <summary>
    /// An external source triggered this action (another pony, an object or an event).
    /// This is the "default" trigger, meaning it is also used when the trigger is unknown.
    /// </summary>
    External = 0,
    /// <summary>
    /// Action was manually triggered by the player.
    /// </summary>
    Player = 1,
    /// <summary>
    /// The pony queued this action autonomously.
    /// </summary>
    FreeWill = 2
}
}