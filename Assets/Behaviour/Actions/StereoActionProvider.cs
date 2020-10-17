using System.Collections.Generic;
using Controllers;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using ThePoniesBehaviour.Behaviours;
using ThePoniesBehaviour.Extensions;

namespace ThePoniesBehaviour.Actions {

public class StereoActionProvider : IObjectActionProvider {
    private const string TurnOnIdentifier = "stereoTurnOn";
    private const string TurnOffIdentifier = "stereoTurnOff";
    private const string SwitchChannelIdentifier = "stereoSwitchChannel";
    private const string NextSongIdentifier = "stereoNextSong";
    private const string StereoType = "stereo";

    private const string TurnedOn = "turnedOn";
    private const string RadioChannel = "radioChannel";

    public IEnumerable<ObjectAction> GetActions(Pony pony, PropertyObject target) {
        if (target.Type == StereoType) {
            if (target.data.GetBool(TurnedOn, false)) {
                List<ObjectAction> actions = new List<ObjectAction>();
                actions.Add(new TurnOnOffAction(pony, target, false));
                actions.Add(new NextSongAction(pony, target));
                foreach (string folder in ContentController.Instance.GetTopLevelAudioFolders("Music/Radio/")) {
                    if (folder != target.data.GetString(RadioChannel, "")) {
                        actions.Add(new SwitchChannelAction(pony, target, folder));
                    }
                }
                return actions;
            } else {
                return new[] {new TurnOnOffAction(pony, target, true)};
            }
        }
        return new ObjectAction[0];
    }

    public ObjectAction LoadAction(string identifier, Pony pony, PropertyObject target) {
        if (identifier == TurnOnIdentifier)
            return new TurnOnOffAction(pony, target, true);
        if (identifier == TurnOffIdentifier)
            return new TurnOnOffAction(pony, target, false);
        if (identifier == SwitchChannelIdentifier)
            return new SwitchChannelAction(pony, target);
        if (identifier == NextSongIdentifier)
            return new NextSongAction(pony, target);
        return null;
    }

    private class TurnOnOffAction : ObjectAction {
        private readonly bool isTurnOn;

        public TurnOnOffAction(Pony pony, PropertyObject target, bool turnOn) :
            base(turnOn ? TurnOnIdentifier : TurnOffIdentifier, pony, target, turnOn ? "Turn on" : "Turn off") {
            isTurnOn = turnOn;
        }

        public override bool Tick() {
            if (target.users.Contains(pony))
                return HandleTurnOnOff();

            // Quit if the stereo is already on/off.
            if (isTurnOn == target.data.GetBool(TurnedOn, false)) {
                return true;
            }

            return MoveToStereo();
        }

        /// <summary>
        /// Move to the stereo.
        /// </summary>
        /// <returns>True if the action failed.</returns>
        private bool MoveToStereo() {
            ActionResult walkResult = this.WalkNextTo(target.TilePosition, target.Rotation, maxUsers: 1);
            if (walkResult == ActionResult.Busy)
                return false;
            if (walkResult == ActionResult.Failed)
                return true;

            // Add this pony as user when it reached the radio.
            target.users.Add(pony);

            return false;
        }

        /// <summary>
        /// Handle switching the stereo on/off.
        /// </summary>
        /// <returns>True when the action was finished.</returns>
        private bool HandleTurnOnOff() {
            if (identifier == TurnOnIdentifier) {
                target.GetComponent<StereoBehaviour>().TurnOn();
            } else {
                target.GetComponent<StereoBehaviour>().TurnOff();
            }
            return true;
        }

        protected override void OnFinish() {
            if (target != null) {
                target.users.Remove(pony);
            }
        }
    }
    
    private class SwitchChannelAction : ObjectAction {
        /// <summary>
        /// Create a loaded switch channel action, used when the channel is not yet known because it is loaded later.
        /// </summary>
        public SwitchChannelAction(Pony pony, PropertyObject target) :
            base(SwitchChannelIdentifier, pony, target, "Switch channel") {
        }
        
        /// <summary>
        /// Create a new switch channel action, used when the channel is known.
        /// </summary>
        public SwitchChannelAction(Pony pony, PropertyObject target, string channel) :
            base(SwitchChannelIdentifier, pony, target, $"Switch to {channel}") {
            data.Put(RadioChannel, channel);
        }

        public override bool Tick() {
            if (target.users.Contains(pony))
                return HandleSwitchChannel();

            // Quit if the stereo is off.
            if (!target.data.GetBool(TurnedOn, false)) {
                return true;
            }

            return MoveToStereo();
        }

        /// <summary>
        /// Move to the stereo.
        /// </summary>
        /// <returns>True if the action failed.</returns>
        private bool MoveToStereo() {
            ActionResult walkResult = this.WalkNextTo(target.TilePosition, target.Rotation, maxUsers: 1);
            if (walkResult == ActionResult.Busy)
                return false;
            if (walkResult == ActionResult.Failed)
                return true;

            // Add this pony as user when it reached the radio.
            target.users.Add(pony);

            return false;
        }

        /// <summary>
        /// Handle switching the channel.
        /// </summary>
        /// <returns>True when the action was finished.</returns>
        private bool HandleSwitchChannel() {
            target.GetComponent<StereoBehaviour>().SwitchChannel(data.GetString(RadioChannel, ""));
            return true;
        }

        protected override void OnFinish() {
            if (target != null) {
                target.users.Remove(pony);
            }
        }
    }
    
    private class NextSongAction : ObjectAction {
        /// <summary>
        /// Create a loaded switch channel action, used when the channel is not yet known because it is loaded later.
        /// </summary>
        public NextSongAction(Pony pony, PropertyObject target) :
            base(SwitchChannelIdentifier, pony, target, "Next song") {
        }

        public override bool Tick() {
            if (target.users.Contains(pony))
                return HandleSwitchSong();

            // Quit if the stereo is off.
            if (!target.data.GetBool(TurnedOn, false)) {
                return true;
            }

            return MoveToStereo();
        }

        /// <summary>
        /// Move to the stereo.
        /// </summary>
        /// <returns>True if the action failed.</returns>
        private bool MoveToStereo() {
            ActionResult walkResult = this.WalkNextTo(target.TilePosition, target.Rotation, maxUsers: 1);
            if (walkResult == ActionResult.Busy)
                return false;
            if (walkResult == ActionResult.Failed)
                return true;

            // Add this pony as user when it reached the radio.
            target.users.Add(pony);

            return false;
        }

        /// <summary>
        /// Handle switching the song.
        /// </summary>
        /// <returns>True when the action was finished.</returns>
        private bool HandleSwitchSong() {
            target.GetComponent<StereoBehaviour>().PlayNextSong();
            return true;
        }

        protected override void OnFinish() {
            if (target != null) {
                target.users.Remove(pony);
            }
        }
    }
}

}