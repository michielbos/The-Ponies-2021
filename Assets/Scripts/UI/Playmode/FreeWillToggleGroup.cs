using System;
using Controllers.Global.Settings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Playmode {
public class FreeWillToggleGroup : MonoBehaviour {
    public ToggleGroup group;
    public Toggle fullToggle;
    public Toggle minimalToggle;
    public Toggle offToggle;

    public UnityEvent<FreeWillOption> onSelectionChanged;

    private void Start() {
        RegisterToggle(fullToggle, FreeWillOption.Full);
        RegisterToggle(minimalToggle, FreeWillOption.Minimal);
        RegisterToggle(offToggle, FreeWillOption.Off);
    }

    private void RegisterToggle(Toggle toggle, FreeWillOption option) {
        toggle.onValueChanged.AddListener(value => {
            if (value) {
                onSelectionChanged.Invoke(option);
            }
        });
    }

    public void SetSelection(FreeWillOption option) {
        fullToggle.isOn = option == FreeWillOption.Full;
        minimalToggle.isOn = option == FreeWillOption.Minimal;
        offToggle.isOn = option == FreeWillOption.Off;
    }
}
}