namespace ThePoniesBehaviour.Extensions {

/// <summary>
/// Result of a sub-action from the action extensions.
/// </summary>
public enum ActionResult {
    /// <summary>
    /// The action is still busy.
    /// </summary>
    Busy = 0,
    /// <summary>
    /// The action has been successfully completed (e.g. reached walk destination).
    /// </summary>
    Success = 1,
    /// <summary>
    /// The action was done but not completed (e.g. canceled or unable to reach target).
    /// </summary>
    Failed = 2
}

}