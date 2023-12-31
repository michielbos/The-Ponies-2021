﻿using Assets.Scripts.Util;
using Controllers;
using Controllers.Global;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class for helping control the general state of the game.
/// It will be kept between scene transitions.
/// </summary>
public class GameController : SingletonMonoBehaviour<GameController> {
	private int enteringLot = -1;

	private new void Awake() {
		if (!HasInstance) {
			base.Awake();
			DontDestroyOnLoad(gameObject);
			// Instantiate other global controllers that should be available cross-scene.
			gameObject.AddComponent<ContentController>();
			gameObject.AddComponent<MusicController>();
			gameObject.AddComponent<SettingsController>();
			gameObject.AddComponent<DiscordController>();
		} else {
			Destroy(gameObject);
		}
	}

	public void EnterLot (int id) {
		enteringLot = id;
		SceneManager.LoadScene("GameSceneTest");
	}

	public void EnterNeighbourhood () {
		SceneManager.LoadScene("PropertyScene");
	}

	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
		
	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		if (scene.name == "GameSceneTest") {
			if (enteringLot < 0) {
				Debug.Log("Started directly from scene, loading lot 0.");
				enteringLot = 0;
			}
			OnEnteredLot(enteringLot);
			enteringLot = -1;
		}
	}

	void OnEnteredLot (int lotId) {
		PropertyController.Instance.Initialize(lotId);
	}

    private void Update()
    {
#if UNITY_EDITOR
        // Allow breaking (pausing) the game in edit mode.
        // Usefull when wanting to check something in the 
        // hierarchy that is not persistent. Press Ctrl + Esc
        if (Input.GetKeyDown(KeyCode.Escape) && Input.GetKey(KeyCode.LeftControl))
        {
            Debug.Break();
        }
#endif
    }
}
