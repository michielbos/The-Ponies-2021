﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatsController : MonoBehaviour {
	public InputField cheatField;
	public RectTransform consolePanel;
	public Text consoleText;
	public HUDController hudController;
	public Text fpsText;
	private bool visible = false;
	private bool expanded = false;
	private bool hadFocus = false;
	private bool showFps = false;
	private float initialCheatFieldX, initialCheatFieldY, initialCheatFieldWidth;
	private float lastConsoleHeight = 0;

	void Start () {
		RectTransform rectTransform = cheatField.GetComponent<RectTransform>();
		initialCheatFieldX = rectTransform.position.x;
		initialCheatFieldY = rectTransform.position.y;
		initialCheatFieldWidth = rectTransform.rect.width;
	}

	void Update () {
		if ((Application.isEditor || Input.GetKey(KeyCode.LeftControl)) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C)) {
			SetCheatFieldVisible(!visible);
		} else if (visible) {
			if (hadFocus && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))) {
				bool wasExpanded = expanded;
				if (cheatField.text.Length > 0) {
					if (!EnterCheat(cheatField.text)) {
					AddConsoleLine("No such cheat.");
					}
				}
				cheatField.text = "";
				if (expanded || wasExpanded) {
					cheatField.ActivateInputField();
				} else {
				SetCheatFieldVisible(false);
				}
			} else if (Input.GetKeyDown(KeyCode.Escape)) {
				SetCheatFieldVisible(false);
			}
		}
		hadFocus = cheatField.isFocused;
		if (showFps) {
			fpsText.text = "FPS: " + Mathf.RoundToInt(1f / Time.deltaTime);
		}
	}

	void LateUpdate () {
		ScrollRect consoleScrollRect = consolePanel.GetComponent<ScrollRect>();
		if (consoleScrollRect.verticalNormalizedPosition != lastConsoleHeight) {
			consoleScrollRect.verticalNormalizedPosition = 0;
		}
	}
		
	void SetCheatFieldVisible (bool visible) {
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

	void SetExpanded (bool expanded) {
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

	void AddConsoleLine (string text) {
		consoleText.text += "\n" + text;
	}

	bool EnterCheat (string cheat) {
		AddConsoleLine(cheat);
		string[] split = cheat.Split(new char[]{' '}, 2);
		string[] parameters;
		if (split.Length > 1) {
			parameters = split[1].Split(new char[]{' '});
		} else {
			parameters = new string[0];
		}
		return TryCheat(split[0].ToLower(), parameters);
	}

	bool TryCheat (string command, string[] parameters) {
		//Money cheats
		if (command == "rosebud")
			hudController.ChangeFunds(1000);
		else if (command == "motherlode")
			hudController.ChangeFunds(50000);
		else if (command == "filthyrich")
			hudController.ChangeFunds(1000000);
		else if (command == "adjustfunds" && parameters.Length == 1)
			return ChangeFundsCheat(parameters[0]);
		else if (command == "addfunds" && parameters.Length == 1)
			return AddFundsCheat(parameters[0]);
		//Misc cheats
		else if (command == "expand")
			SetExpanded(!expanded);
		else if (command == "help")
			ShowHelp();
		else if (command == "showfps")
			SetShowFps(!showFps);
		else if (command == "clear")
			ClearConsole();
		else if (command == "close")
			SetCheatFieldVisible(false);
		else if (command == "forcequit")
			Application.Quit();
		else
			return false;
		return true;
	}

	bool ChangeFundsCheat (string amountString) {
		int amount;
		if (int.TryParse(amountString, out amount)) {
			hudController.SetFunds(amount);
			return true;
		} else
			return false;
	}

	bool AddFundsCheat (string amountString) {
		int amount;
		if (int.TryParse(amountString, out amount)) {
			hudController.ChangeFunds(amount);
			return true;
		} else
			return false;
	}

	void ShowHelp () {
		SetExpanded(true);
		TextAsset helpText = Resources.Load<TextAsset>("cheats_help");
		AddConsoleLine(helpText.text);
	}

	void SetShowFps (bool showFps) {
		this.showFps = showFps;
		if (showFps) {
			fpsText.rectTransform.localScale = new Vector3(1, 1, 1);
		} else {
			fpsText.rectTransform.localScale = new Vector3(0, 0, 0);
		}
	}

	void ClearConsole () {
		consoleText.text = "";
	}
}
