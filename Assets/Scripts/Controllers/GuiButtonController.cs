using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers {

public class GuiButtonController : MonoBehaviour {
    public GameObject[] assignedObjects;
    public Button modeButton;

    public bool Locked {
        get { return !modeButton.interactable; }
        set { modeButton.interactable = !value; }
    }

    public virtual void OnEnable() {
        foreach (GameObject o in assignedObjects) {
            o.SetActive(true);
        }
        var cb = modeButton.colors;
        cb.normalColor = cb.pressedColor;
        modeButton.colors = cb;
    }

    public virtual void OnDisable() {
        foreach (GameObject o in assignedObjects) {
            o.SetActive(false);
        }
        var cb = modeButton.colors;
        cb.normalColor = Color.white;
        modeButton.colors = cb;
    }
}

}