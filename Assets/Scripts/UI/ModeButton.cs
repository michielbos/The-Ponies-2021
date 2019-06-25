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

    public virtual void OnEnable() {
        foreach (GameObject o in assignedObjects) {
            o.SetActive(true);
        }
    }

    public virtual void OnDisable() {
        foreach (GameObject o in assignedObjects) {
            o.SetActive(false);
        }
    }
}

}