﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Controllers {
    public enum SoundType {
        Undefined,
        Sell,
        Buy,
        Deny,
        Place,
        Rotate,
        Woosh,
        Click
    }

    [Serializable]
    public class SoundValuePair {
        public SoundType type;
        public AudioClip clip;
    }

    public class SoundController : SingletonMonoBehaviour<SoundController> {
        [SerializeField] protected int chanelCount = 5;
        [SerializeField] protected List<SoundValuePair> clips;

        private AudioSource[] sources;
        private int currentSource = 0;

        public void Start() {
            if (chanelCount < 2)
                chanelCount = 2;

            sources = new AudioSource[chanelCount];
            for (int i = 0; i < chanelCount; i++) {
                sources[i] = Camera.main.gameObject.AddComponent<AudioSource>();
            }
        }

        public void PlaySound(SoundType type) {
            if (clips.Count == 0 || type == SoundType.Undefined)
                return;
            SoundValuePair[] pairs = clips.Where(c => c.type == type).ToArray();
            if (pairs.Length == 0)
                return;
            SoundValuePair pair = pairs[UnityEngine.Random.Range(0, pairs.Length)];
            PlaySound(pair.clip);
        }

        public void PlaySound(string name) {
            if (clips.Count == 0)
                return;
            SoundValuePair[] pairs = clips.Where(c => c.clip != null && c.clip.name == name).ToArray();
            if (pairs.Length == 0)
                return;
            SoundValuePair pair = pairs[UnityEngine.Random.Range(0, pairs.Length)];
            PlaySound(pair.clip);
        }

        public void PlaySound(AudioClip clip) {
            if (clip == null)
                return;
            sources[currentSource].PlayOneShot(clip);
            currentSource = (currentSource + 1) % chanelCount;
        }
    }
}