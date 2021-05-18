using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;
using Util;

namespace Controllers {

public class ContentController : SingletonMonoBehaviour<ContentController> {
    private Dictionary<string, AudioClip> loadedAudioClips = new Dictionary<string, AudioClip>();

    /// <summary>
    /// Coroutine that loads the content into this controller (currently just the audio).
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadContent() {
        ContentLoader contentLoader = new ContentLoader();
        yield return contentLoader.LoadAudioClips(clips => {
            loadedAudioClips = clips;
        });
    }

    [CanBeNull]
    public AudioClip GetAudioClip(string name) {
        return loadedAudioClips.Get(name);
    }

    public IEnumerable<AudioClip> GetAudioClips(string prefix) => loadedAudioClips
        .Where(pair => pair.Key.StartsWith(prefix))
        .Select(pair => pair.Value);

    public IEnumerable<string> GetAudioNames(string prefix) => loadedAudioClips.Keys
        .Where(key => key.StartsWith(prefix));

    /// <summary>
    /// Get all top level audio folders with the given prefix.
    /// For example "Music/Radio/" returns the names of all radio channels..
    /// </summary>
    public IEnumerable<string> GetTopLevelAudioFolders(string prefix) => loadedAudioClips.Keys
        .Where(path => path.StartsWith(prefix))
        .Select(path => path.Substring(prefix.Length))
        .Select(path => path.Substring(0, path.IndexOf('/')))
        .Distinct();
}

}