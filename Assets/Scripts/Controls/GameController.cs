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
		Debug.Log("OnEnable called");
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
		
	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		Debug.Log("OnSceneLoaded: " + scene.name);
		Debug.Log("Mode: " + mode);
		if (scene.name == "GameSceneTest" && enteringLot >= 0) {
			OnEnteredLot(enteringLot);
			enteringLot = 0;
		}
	}

	void OnEnteredLot (int lotId) {
		Debug.Log("ENTERED " + lotId);
		GameObject gameObject = GameObject.FindGameObjectWithTag("PropertyController");
		PropertyController propertyController = GameObject.FindGameObjectWithTag("PropertyController").GetComponent<PropertyController>();
		propertyController.Initialize(lotId);
	}

	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
}
