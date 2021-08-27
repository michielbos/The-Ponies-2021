using System;
using Assets.Scripts.Util;
using Discord;
using UnityEngine;

namespace Controllers.Global {

/// <summary>
/// Controller for Discord integration.
/// This is a global singleton that is created on startup by the GameController. It is accessible in every scene.
/// </summary>
public class DiscordController : SingletonMonoBehaviour<DiscordController> {
    /// <summary>
    /// The Discord application Client ID.
    /// </summary>
    private const long ClientId = 662705592351326229L;
    private Discord.Discord discord;

    /// <summary>
    /// The state that is displayed on the player's Discord profile.
    /// </summary>
    public enum DiscordState {
        None,
        Neighbourhood,
        CreateAPony,
        LiveMode,
        BuyMode,
        BuildMode,
        Settings
    }

    private void Start() {
        // This throws a ResultException that says "InternalError" when Discord is not running (on Linux editor).
        // Currently doesn't break anything. Probably a bug in the SDK?
        if (Application.platform != RuntimePlatform.LinuxEditor) {
            discord = new Discord.Discord(ClientId, (ulong) CreateFlags.NoRequireDiscord);
        }
        if (SettingsController.Instance.discordIntegration.Value) {
            UpdateActivity(DiscordState.None);
        } else {
            InitWithoutActivity();
        }
    }

    private void OnDestroy() {
        // End the play session on Discord.
        discord?.Dispose();
    }

    private void Update() {
        discord?.RunCallbacks();
    }

    /// <summary>
    /// Update the current activity on the player's Discord profile.
    /// </summary>
    /// <param name="state">The current activity that the player is doing.</param>
    /// <param name="details">Details on the player's activity (such as a household name).</param>
    public void UpdateActivity(DiscordState state, string details = "") {
        // Ignore activity updates if Discord was not started or if Discord integration has been disabled.
        if (discord == null || !SettingsController.Instance.discordIntegration.Value)
            return;
        
        Activity activity = new Activity {
            State = GetStateString(state),
            Details = details,
            Assets = new ActivityAssets {
                LargeImage = "textlogo",
                LargeText = "The Ponies"
            }
        };
        discord.GetActivityManager().UpdateActivity(activity, result => {
            if (result != Result.Ok) {
                Debug.LogWarning("Failed to update Discord activity: " + result);
            }
        });
    }

    /// <summary>
    /// Clear the player's activity, to replace the rich presence view by the default game title and icon.
    /// </summary>
    public void ClearActivity() {
        discord.GetActivityManager().ClearActivity(result => {
            if (result != Result.Ok) {
                Debug.LogWarning("Failed to clear Discord activity: " + result);
            }
        });
    }

    /// <summary>
    /// Add an empty activity and then clear it.
    /// This will make Discord display that the user is playing the game, without enabling Rich Presence.
    /// </summary>
    private void InitWithoutActivity() {
        discord.GetActivityManager().UpdateActivity(new Activity(), result => {
            if (result != Result.Ok) {
                Debug.LogWarning("Failed to create empty Discord activity: " + result);
            }
            ClearActivity();
        });
    }

    /// <summary>
    /// Translate a DiscordState to the string that should be displayed on Discord.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If an invalid value was passed.</exception>
    private string GetStateString(DiscordState state) {
        switch (state) {
            case DiscordState.None:
                return "";
            case DiscordState.Neighbourhood:
                return "Neighbourhood";
            case DiscordState.CreateAPony:
                return "Create a Pony";
            case DiscordState.LiveMode:
                return "Live mode";
            case DiscordState.BuyMode:
                return "Buy mode";
            case DiscordState.BuildMode:
                return "Build mode";
            case DiscordState.Settings:
                return "Settings";
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}

}