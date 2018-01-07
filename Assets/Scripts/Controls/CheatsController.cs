using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatsController : MonoBehaviour {
	public InputField cheatField;
	public HUDController hudController;
	private bool hadFocus = false;

	void Start () {
		
	}

	void Update () {
		//It seems Unity doesn't accept the C key while ctrl is pressed...
		if (/*Input.GetKey(KeyCode.LeftControl) &&*/ Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C)) {
			setCheatFieldVisible(!cheatField.enabled);
		}
		if (cheatField.enabled && hadFocus && Input.GetKey(KeyCode.Return)) {
			Debug.Log(enterCheat(cheatField.text));
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

	bool enterCheat (string cheat) {
		string[] split = cheat.Split(new char[]{' '}, 2);
		string[] parameters;
		if (split.Length > 1) {
			parameters = split[1].Split(new char[]{ ' ' });
		} else {
			parameters = new string[0];
		}
		return tryCheat(split[0].ToLower(), parameters);
	}

	bool tryCheat (string command, string[] parameters) {
		if (command == "rosebud")
			hudController.ChangeFunds(1000);
		else if (command == "motherload")
			hudController.ChangeFunds(50000);
		else if (command == "filthyrich")
			hudController.ChangeFunds(1000000);
		else if (command == "forcequit")
			Application.Quit();
		else if (command == "adjustfunds" && parameters.Length == 1)
			return changeFundsCheat(parameters[0]);
		else if (command == "addfunds" && parameters.Length == 1)
			return addFundsCheat(parameters[0]);
		else
			return false;
		return true;
	}

	bool changeFundsCheat (string amountString) {
		int amount;
		if (int.TryParse(amountString, out amount)) {
			hudController.SetFunds(amount);
			return true;
		} else
			return false;
	}

	bool addFundsCheat (string amountString) {
		int amount;
		if (int.TryParse(amountString, out amount)) {
			hudController.ChangeFunds(amount);
			return true;
		} else
			return false;
	}
}
