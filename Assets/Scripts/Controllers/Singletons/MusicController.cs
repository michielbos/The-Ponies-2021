﻿using System;
using System.Linq;
using Assets.Scripts.Util;
using Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// Enum with all the music types.
/// </summary>
public enum MusicType {
    NoMusic,
    Neighbourhood,
    CreateAPony,
    BuyMode,
    BuildMode,
    CommunityBuildMode
}

public class MusicController : SingletonMonoBehaviour<MusicController> {
    private AudioClip[] neighbourhoodSongs = new AudioClip[0];
    private AudioClip[] capSongs = new AudioClip[0];
    private AudioClip[] buyModeSongs = new AudioClip[0];
    private AudioClip[] buildModeSongs = new AudioClip[0];
    private AudioClip[] communityBuildSongs = new AudioClip[0];
    private MusicType currentMusicType = MusicType.NoMusic;

    private AudioClip[][] playingClips;
    private int[] playingIndex;
    private AudioSource audioSource;

    public void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        audioSource = Camera.main.gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// Load all music from the ContentController.
    /// Must be called after the ContentController has finished loading all audio.
    /// </summary>
    public void LoadAllMusic() {
        int numberOfTypes = Enum.GetNames(typeof(MusicType)).Length;
        playingClips = new AudioClip[numberOfTypes][];
        playingIndex = new int[numberOfTypes];

        ContentController contentController = ContentController.Instance;
        buyModeSongs = contentController.GetAudioClips("Music/Buy/").ToArray();
        buildModeSongs = contentController.GetAudioClips("Music/Build/").ToArray();
        capSongs = contentController.GetAudioClips("Music/CAP/").ToArray();
        communityBuildSongs = contentController.GetAudioClips("Music/Community/").ToArray();
        neighbourhoodSongs = contentController.GetAudioClips("Music/Neighbourhood/").ToArray();
        
        foreach (MusicType musicType in Enum.GetValues(typeof(MusicType))) {
            ShufflePlayingList(musicType);
        }
    }

    private void Update() {
        if (audioSource.clip != null && !audioSource.isPlaying) {
            PlayNextSong();
        }
    }

    public void SwitchMusic(MusicType musicType) {
        currentMusicType = musicType;
        PlayNextSong();
    }

    public void PlayNextSong() {
        AudioClip[] songs = GetSongsForType(currentMusicType);
        if (songs.Length <= 0) {
            audioSource.Stop();
            audioSource.clip = null;
            return;
        }
        int musicType = (int) currentMusicType;
        if (playingIndex[musicType] >= songs.Length) {
            ShufflePlayingList(currentMusicType);
        }
        AudioClip[] playing = playingClips[musicType];
        audioSource.clip = playing[playingIndex[musicType]++];
        audioSource.PlayDelayed(2);
    }

    private void ShufflePlayingList(MusicType musicType) {
        AudioClip[] songs = GetSongsForType(musicType);
        int musicTypeIndex = (int) musicType;
        AudioClip[] playing = playingClips[musicTypeIndex];
        AudioClip lastSong = null;

        if (playing != null && playing.Length > 1) {
            lastSong = playing[playing.Length - 1];
        }
        playing = ((AudioClip[]) songs.Clone()).OrderBy(x => Random.value).ToArray();
        if (playing.Length > 1 && playing[0] == lastSong) {
            int switchIndex = Random.Range(1, playing.Length);
            playing[0] = playing[switchIndex];
            playing[switchIndex] = lastSong;
        }
        playingClips[musicTypeIndex] = playing;
        playingIndex[musicTypeIndex] = 0;
    }

    private AudioClip[] GetSongsForType(MusicType musicType) {
        switch (musicType) {
            case MusicType.NoMusic:
                return new AudioClip[0];
            case MusicType.Neighbourhood:
                return neighbourhoodSongs;
            case MusicType.CreateAPony:
                return capSongs;
            case MusicType.BuyMode:
                return buyModeSongs;
            case MusicType.BuildMode:
                return buildModeSongs;
            case MusicType.CommunityBuildMode:
                return communityBuildSongs;
            default:
                return new AudioClip[0];
        }
    }
}