using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Util {

public class ContentLoader {
    public static readonly string ContentFolder = Application.dataPath + "/Content/";
    public static readonly string ModsFolder = Application.dataPath + "/../Mods/";

    /// <summary>
    /// Coroutine to load OGG AudioClips from the content folder.
    /// The onFinish action is executed when the loading is complete.
    /// If one or more items failed to load, those will not be included in the list and instead logged to the console.
    /// </summary>
    public IEnumerator LoadAudioClips(Action<Dictionary<string, AudioClip>> onFinish) {
        string path = ContentFolder;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
        foreach (string file in files) {
            if (!file.EndsWith(".ogg"))
                continue;
            string name = file.Substring(path.Length);
            yield return LoadAudioClip(file, clip => clips[name] = clip, Debug.LogException);
        }
        onFinish(clips);
    }

    /// <summary>
    /// Coroutine to load OGG AudioClip from the given file.
    /// onSuccess action is executed when the load has completed succesfully.
    /// If the load failed, onFail will be executed instead.
    /// </summary>
    public IEnumerator LoadAudioClip(string file, Action<AudioClip> onSuccess, Action<ContentLoaderException> onFail) {
        string filePath = "file://" + file;
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.OGGVORBIS)) {
            yield return www.SendWebRequest();
            try {
                if (www.isNetworkError || www.isHttpError) {
                    throw new ContentLoaderException("Couldn't load file: " + filePath + " (" + www.error + ")");
                }

                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                if (clip.samples == 0)
                    throw new ContentLoaderException("Failed to load file: " + filePath);
                onSuccess(clip);
            } catch (ContentLoaderException e) {
                onFail(e);
            }
        }
    }
}

}