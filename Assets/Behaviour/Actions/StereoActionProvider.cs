using System.Collections.Generic;
using Controllers;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using ThePoniesBehaviour.Extensions;

namespace ThePoniesBehaviour.Actions {

public class StereoActionProvider : IObjectActionProvider {
    private const string TurnOnIdentifier = "stereoTurnOn";
    private const string TurnOffIdentifier = "stereoTurnOff";
    private const string SwitchChannelIdentifier = "stereoSwitchChannel";
    private const string StereoType = "stereo";

    private const string TurnedOn = "turnedOn";
    private const string RadioChannel = "radioChannel";
    private const string StartupChannel = "Dubstep";

    public IEnumerable<ObjectAction> GetActions(Pony pony, PropertyObject target) {
        if (target.Type == StereoType) {
            if (target.data.GetBool(TurnedOn, false)) {
                List<ObjectAction> actions = new List<ObjectAction>();
                actions.Add(new TurnOnOffAction(pony, target, false));
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
        return null;
    }

    private class TurnOnOffAction : ObjectAction {
        private readonly bool isTurnOn;

        public TurnOnOffAction(Pony pony, PropertyObject target, bool turnOn) :
            base(turnOn ? TurnOnIdentifier : TurnOffIdentifier, pony, target, turnOn ? "Turn On" : "Turn Off") {
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
                target.data.Put(TurnedOn, true);
                if (!target.data.ContainsKey(RadioChannel)) {
                    target.data.Put(RadioChannel, StartupChannel);
                }
            } else {
                target.data.Put(TurnedOn, false);
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
            target.data.Put(RadioChannel, data.GetString(RadioChannel, ""));
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