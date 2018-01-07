using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatsController : MonoBehaviour {
	public InputField cheatField;
	private bool hadFocus = false;

	void Start () {
		
	}

	void Update () {
		//It seems Unity doesn't accept the C key while ctrl is pressed...
		if (/*Input.GetKey(KeyCode.LeftControl) &&*/ Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C)) {
			setCheatFieldVisible(!cheatField.enabled);
		}
		if (cheatField.enabled && hadFocus && Input.GetKey(KeyCode.Return)) {
			Debug.Log("Entered cheat: " + cheatField.text);
			setCheatFieldVisible(false);
		}
		hadFocus = cheatField.isFocused;
	}
		
	void setCheatFieldVisible (bool visible) {
		//The active/enabled flag won't hide it, so we're just shrinking it to zero for now...
		cheatField.interactable = visible;
		cheatField.enabled = visible;
		if (visible) {
			cheatField.transform.localScale = new Vector3 (1, 1, 1);
			cheatField.ActivateInputField();
		} else {
			cheatField.text = "";
			cheatField.transform.localScale = new Vector3 (0, 0, 0);
		}
	}
}
