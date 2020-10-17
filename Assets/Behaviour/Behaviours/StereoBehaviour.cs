using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Model.Property;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ThePoniesBehaviour.Behaviours {

public class StereoBehaviour : MonoBehaviour {
    private const string TurnedOn = "turnedOn";
    private const string RadioChannel = "radioChannel";
    private const string StartupChannel = "Dubstep";

    private PropertyObject propertyObject;
    private AudioClip[] playlist = new AudioClip[0];
    private int playIndex;
    private bool turnedOn;

    private void Awake() {
        propertyObject = GetComponent<PropertyObject>();
    }

    private void Start() {
        turnedOn = propertyObject.data.GetBool(TurnedOn, false);
        if (turnedOn) {
            PlayNextSong();
            propertyObject.PlayAnimation("Playing");
        }
    }

    private void Update() {
        if (turnedOn && !propertyObject.IsPlayingSound()) {
            PlayNextSong();
        }
    }

    public void PlayNextSong() {
        playIndex++;
        if (playIndex >= playlist.Length) {
            ShufflePlaylist();
        }
        if (playlist.Length > 0) {
            propertyObject.PlaySound(playlist[playIndex]);
        }
    }

    private void ShufflePlaylist() {
        string channel = propertyObject.data.GetString(RadioChannel, StartupChannel);
        IEnumerable<AudioClip> clips = ContentController.Instance.GetAudioClips($"Music/Radio/{channel}");

        playlist = clips.OrderBy(clip => Random.value).ToArray();
        playIndex = 0;
    }

    public void TurnOn() {
        propertyObject.data.Put(TurnedOn, true);
        turnedOn = true;
        playIndex = playlist.Length;
        PlayNextSong();
        propertyObject.PlayAnimation("Playing");
    }

    public void TurnOff() {
        propertyObject.data.Put(TurnedOn, false);
        turnedOn = false;
        propertyObject.StopSound();
        propertyObject.StopAnimation();
    }

    public void SwitchChannel(string channel) {
        propertyObject.data.Put(RadioChannel, channel);
        playIndex = playlist.Length;
        PlayNextSong();
    }
}

}