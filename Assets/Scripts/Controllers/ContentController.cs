using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;
using Util;

namespace Controllers {

public class ContentController : SingletonMonoBehaviour<ContentController> {
    private Dictionary<string, AudioClip> loadedAudioClips = new Dictionary<string, AudioClip>();
    private bool audioLoaded;
    private readonly List<Action> audioLoadedListeners = new List<Action>();

    private void Start() {
        ContentLoader contentLoader = new ContentLoader();
        StartCoroutine(contentLoader.LoadAudioClips(clips => {
            loadedAudioClips = clips;
            audioLoaded = true;
            audioLoadedListeners.ForEach(action => action());
            audioLoadedListeners.Clear();
            Debug.Log("Loaded audio.");
        }));
    }

    [CanBeNull]
    public AudioClip GetAudioClip(string name) {
        return loadedAudioClips.Get(name);
    }

    public IEnumerable<AudioClip> GetAudioClips(string prefix) => loadedAudioClips
        .Where(pair => pair.Key.StartsWith(prefix))
        .Select(pair => pair.Value);

    /// <summary>
    /// Execute the given action when the audio content has been loaded.
    /// If the content was already loaded, it will be executed immediately.
    /// </summary>
    public void OnAudioLoaded(Action action) {
        if (audioLoaded)
            action();
        else
            audioLoadedListeners.Add(action);
    }
}

}