using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatsController : MonoBehaviour {
	public InputField cheatField;
	public RectTransform consolePanel;
	public Text consoleText;
	public HUDController hudController;
	private bool visible = false;
	private bool expanded = false;
	private bool hadFocus = false;
	private string consoleContent = "";
	private float initialCheatFieldX, initialCheatFieldY, initialCheatFieldWidth;

	void Start () {
		RectTransform rectTransform = cheatField.GetComponent<RectTransform>();
		initialCheatFieldX = rectTransform.position.x;
		initialCheatFieldY = rectTransform.position.y;
		initialCheatFieldWidth = rectTransform.rect.width;
	}

	void Update () {
		//It seems Unity doesn't accept the C key while ctrl is pressed...
		if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C)) {
			setCheatFieldVisible(!visible);
		}
		if (visible && hadFocus && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))) {
			bool wasExpanded = expanded;
			if (cheatField.text.Length > 0) {
				enterCheat(cheatField.text);
			}
			cheatField.text = "";
			if (expanded || wasExpanded) {
				cheatField.ActivateInputField();
			} else {
				setCheatFieldVisible(false);
			}
		}
		hadFocus = cheatField.isFocused;
	}
		
	void setCheatFieldVisible (bool visible) {
		this.visible = visible;
		cheatField.interactable = visible;
		//The active/enabled flag won't hide it, so we're just shrinking it to zero for now...
		if (visible) {
			cheatField.transform.localScale = new Vector3(1, 1, 1);
			cheatField.ActivateInputField();
			if (expanded) {
				consolePanel.localScale = new Vector3(1, 1, 1);
			}
		} else {
			cheatField.text = "";
			cheatField.transform.localScale = new Vector3(0, 0, 0);
			consolePanel.localScale = new Vector3(0, 0, 0);
		}
	}

	void setExpanded (bool expanded) {
		this.expanded = expanded;
		if (visible) {
			consolePanel.localScale = new Vector3(1, 1, 1);
		}
		RectTransform cheatFieldTransform = cheatField.GetComponent<RectTransform>();
		if (expanded) {
			cheatFieldTransform.anchoredPosition = new Vector2(0, -consolePanel.rect.height);
			cheatFieldTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, consolePanel.rect.width);
		} else {
			cheatFieldTransform.position = new Vector2(initialCheatFieldX, initialCheatFieldY);
			cheatFieldTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, initialCheatFieldWidth);
		}
	}

	void addConsoleLine (string text) {
		consoleText.text += "\n" + text;
		LayoutRebuilder.ForceRebuildLayoutImmediate(consolePanel);
		consolePanel.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
	}

	bool enterCheat (string cheat) {
		addConsoleLine(cheat);
		string[] split = cheat.Split(new char[]{' '}, 2);
		string[] parameters;
		if (split.Length > 1) {
			parameters = split[1].Split(new char[]{' '});
		} else {
			parameters = new string[0];
		}
		return tryCheat(split[0].ToLower(), parameters);
	}

	bool tryCheat (string command, string[] parameters) {
		//Money cheats
		if (command == "rosebud")
			hudController.ChangeFunds(1000);
		else if (command == "motherload")
			hudController.ChangeFunds(50000);
		else if (command == "filthyrich")
			hudController.ChangeFunds(1000000);
		else if (command == "adjustfunds" && parameters.Length == 1)
			return changeFundsCheat(parameters[0]);
		else if (command == "addfunds" && parameters.Length == 1)
			return addFundsCheat(parameters[0]);
		//Misc cheats
		else if (command == "expand")
			setExpanded(!expanded);
		else if (command == "help")
			showHelp();
		else if (command == "forcequit")
			Application.Quit();
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

	void showHelp () {
		setExpanded(true);
		TextAsset helpText = Resources.Load<TextAsset>("cheats_help");
		addConsoleLine(helpText.text);
	}
}
