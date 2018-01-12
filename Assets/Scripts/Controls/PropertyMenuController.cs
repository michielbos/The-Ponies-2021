using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PropertyMenuController : MonoBehaviour {
	public Button[] lotButtons;

	void Start () {
		PropertyLoader propertyLoader = new PropertyLoader();
		for (int i = 0; i < lotButtons.Length; i++) {
			if (propertyLoader.PropertyExists(i)) {
				Property property = propertyLoader.LoadProperty(i);
				lotButtons[i].GetComponentInChildren<Text>().text = i + ". " + property.name + "\n(" + property.streetName + ")";
			} else {
				lotButtons[i].GetComponentInChildren<Text>().text = "Empty lot (" + i + ")";
			}
		}
	}

	public void enterLot (int id) {
		GameController.instance.EnterLot(id);
	}
}
