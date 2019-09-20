using System;
using System.Collections;
using Assets.Scripts.Util;
using Controllers.Playmode;
using Model.Ponies;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.Singletons {

public class CheatsController : SingletonMonoBehaviour<CheatsController> {
    // The max number of characters on the console panel text.
    // We use 10000 for now to not get too close to the geometry limit.
    private const int CharacterLimit = 10000;
    
    public InputField cheatField;
    public RectTransform consolePanel;
    public Text consoleText;
    public Text fpsText;
    public bool moveObjectsMode;
    private bool visible;
    private bool expanded;
    private bool hadFocus;
    private bool showFps;
    private float initialCheatFieldX, initialCheatFieldY, initialCheatFieldWidth;

    private void Start() {
        RectTransform rectTransform = cheatField.GetComponent<RectTransform>();
        initialCheatFieldX = rectTransform.position.x;
        initialCheatFieldY = rectTransform.position.y;
        initialCheatFieldWidth = rectTransform.rect.width;
        Application.logMessageReceived += OnLogMessage;
    }

    private void OnDestroy() {
        Application.logMessageReceived -= OnLogMessage;
    }

    private void OnLogMessage(string logString, string stackTrace, LogType logType) {
        AddConsoleLine(logString, GetLogTypeColor(logType));
    }

    private void Update() {
        if ((Application.isEditor || Input.GetKey(KeyCode.LeftControl)) && Input.GetKey(KeyCode.LeftShift) &&
            Input.GetKeyDown(KeyCode.C)) {
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
            fpsText.text = "FPS: " + Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
        }
    }

    public void SetCheatFieldVisible(bool visible) {
        this.visible = visible;
        cheatField.interactable = visible;
        if (visible) {
            cheatField.transform.gameObject.SetActive(true);
            cheatField.ActivateInputField();
            if (expanded) {
                consolePanel.gameObject.SetActive(true);
            }
        } else {
            cheatField.text = "";
            cheatField.gameObject.SetActive(false);
            consolePanel.gameObject.SetActive(false);
        }
    }

    public void SetExpanded(bool expanded) {
        this.expanded = expanded;
        if (visible) {
            consolePanel.gameObject.SetActive(true);;
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

    public void AddConsoleLine(string text, string color = "white") {
        string lineText = "<color=" + color + ">" + text + "</color>";
        string panelText = consoleText.text;
        
        if (panelText.Length > 0)
            lineText = "\n" + lineText;
        panelText += lineText;
        if (panelText.Length > CharacterLimit) {
            int newStart = panelText.IndexOf("</color>\n", panelText.Length - CharacterLimit, StringComparison.Ordinal) + 9;
            if (newStart == 0)
                panelText = "";
            panelText = panelText.Substring(newStart);
        }
        consoleText.text = panelText;
        
        // Make sure the scroll view scrolls automatically if it's at the bottom.
        ScrollRect consoleScrollRect = consolePanel.GetComponent<ScrollRect>();
        if (Math.Abs(consoleScrollRect.verticalNormalizedPosition) < 0.0001) {
            StartCoroutine(ScrollConsoleToBottom());
        }
    }

    private string GetLogTypeColor(LogType logType) {
        switch (logType) {
            case LogType.Error:
                return "orange";
            case LogType.Assert:
                return "red";
            case LogType.Warning:
                return "yellow";
            case LogType.Exception:
                return "magenta";
            default:
                return "white";
        }
    }

    private IEnumerator ScrollConsoleToBottom() {
        // For some reason, LateUpdate doesn't work, but WaitForEndOfFrame does.
        yield return new WaitForEndOfFrame();
        ScrollRect consoleScrollRect = consolePanel.GetComponent<ScrollRect>();
        consoleScrollRect.verticalNormalizedPosition = 0;
    }

    public bool EnterCheat(string cheat) {
        AddConsoleLine(cheat);
        string[] split = cheat.Split(new char[] {' '}, 2);
        string[] parameters;
        if (split.Length > 1) {
            parameters = split[1].Split(' ');
        } else {
            parameters = new string[0];
        }

        return TryCheat(split[0].ToLower(), parameters, cheat);
    }

    private bool TryCheat(string command, string[] parameters, string wholeCommand) {
        // Money cheats
        if (command == "rosebud")
            MoneyController.Instance.ChangeFunds(1000);
        else if (command == "motherlode")
            MoneyController.Instance.ChangeFunds(50000);
        else if (command == "filthyrich")
            MoneyController.Instance.ChangeFunds(1000000);
        else if (command == "adjustfunds" && parameters.Length == 1)
            return ChangeFundsCheat(parameters[0]);
        else if (command == "addfunds" && parameters.Length == 1)
            return AddFundsCheat(parameters[0]);
        // Buying/building cheats
        else if (command == "moveobjects" && parameters.Length == 1)
            return MoveObjectsCheat(parameters[0]);
        // Live cheats
        else if (command == "maxmuffins") {
            MaxNeeds();
        }
        // Misc cheats
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

    private bool ChangeFundsCheat(string amountString) {
        int amount;
        if (int.TryParse(amountString, out amount)) {
            MoneyController.Instance.SetFunds(amount);
            return true;
        } else
            return false;
    }

    private bool AddFundsCheat(string amountString) {
        int amount;
        if (int.TryParse(amountString, out amount)) {
            MoneyController.Instance.ChangeFunds(amount);
            return true;
        } else
            return false;
    }

    private bool MoveObjectsCheat(string parameter) {
        if (parameter.ToLower() == "on") {
            moveObjectsMode = true;
        } else if (parameter.ToLower() == "off") {
            moveObjectsMode = false;
        } else
            return false;

        return true;
    }

    private void MaxNeeds() {
        Household household = HouseholdController.Instance.Household;
        if (household == null)
            return;
        foreach (Pony pony in household.ponies.Values) {
            pony.needs.SetAll(1);
        }
    }

    private void ShowHelp() {
        SetExpanded(true);
        TextAsset helpText = Resources.Load<TextAsset>("cheats_help");
        AddConsoleLine(helpText.text);
    }

    private void SetShowFps(bool showFps) {
        this.showFps = showFps;
        fpsText.rectTransform.gameObject.SetActive(showFps);
    }

    private void ClearConsole() {
        consoleText.text = "";
    }
}

}