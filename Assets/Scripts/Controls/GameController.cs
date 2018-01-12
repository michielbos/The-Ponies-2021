using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class for helping control the general state of the game.
/// It will be kept between scene transitions.
/// </summary>
public class GameController : MonoBehaviour {
	int enteringLot = -1;

	void Awake () {
		DontDestroyOnLoad(gameObject);
	}

	public void EnterLot (int id) {
		enteringLot = id;
		SceneManager.LoadScene("GameSceneTest");
	}

	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
		
	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		if (scene.name == "GameSceneTest" && enteringLot >= 0) {
			OnEnteredLot(enteringLot);
			enteringLot = -1;
		}
	}

	void OnEnteredLot (int lotId) {
		PropertyController propertyController = GameObject.FindGameObjectWithTag("PropertyController").GetComponent<PropertyController>();
		propertyController.Initialize(lotId);
	}
}
