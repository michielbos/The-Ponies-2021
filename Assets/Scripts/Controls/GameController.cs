﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class for helping control the general state of the game.
/// It will be kept between scene transitions.
/// </summary>
public class GameController : MonoBehaviour {
	public static GameController instance;
	private int enteringLot = -1;

	void Awake () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
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
		GameObject propertyObject = GameObject.FindGameObjectWithTag("PropertyController");
		PropertyController propertyController = propertyObject.GetComponent<PropertyController>();
		propertyController.Initialize(lotId);
	}
}