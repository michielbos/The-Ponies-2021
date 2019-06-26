using UnityEngine;
using UnityEngine.UI;

namespace UI {

[RequireComponent(typeof(Button))]
public class ModeButton : MonoBehaviour {
    public GameObject[] assignedObjects;
    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
    }

    public bool Locked {
        get { return !button.interactable; }
        set { button.interactable = !value; }
    }

    public void SetModeActive(bool active) {
        if (active) {
            OnModeActivated();
        } else {
            OnModeDeactivated();
        }
    }

    private void OnModeActivated() {
        foreach (GameObject o in assignedObjects) {
            o.SetActive(true);
        }
    }

    private void OnModeDeactivated() {
        foreach (GameObject o in assignedObjects) {
            o.SetActive(false);
        }
    }
}

}