using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveWindowController : MonoBehaviour {
	public PropertyController propertyController;
	public GameObject savePanel;
	public InputField saveNameField;
	public InputField saveDescriptionField;
	public Button saveButton;
	public Button dontSaveButton;
	public Button cancelButton;
	private SaveType saveType;

	enum SaveType {SAVE, SAVE_AND_RETURN, SAVE_AND_QUIT};

	public void OpenNormalSaveDialog () {
		OpenSaveDialog();
		saveButton.GetComponentInChildren<Text>().text = "Save";
		saveType = SaveType.SAVE;
	}

	public void OpenSaveAndReturnDialog () {
		OpenSaveDialog();
		saveButton.GetComponentInChildren<Text>().text = "Save & Return";
		saveType = SaveType.SAVE_AND_RETURN;
	}

	public void OpenSaveAndQuitDialog () {
		OpenSaveDialog();
		saveButton.GetComponentInChildren<Text>().text = "Save & Quit";
		saveType = SaveType.SAVE_AND_QUIT;
	}

	private void OpenSaveDialog () {
		savePanel.transform.localScale = new Vector3(1, 1, 1);
		saveNameField.text = propertyController.property.name;
		saveDescriptionField.text = propertyController.property.description;
	}

	public void ConfirmSaveButton () {
		CloseSaveDialog();
		SaveProperty();
		if (saveType == SaveType.SAVE_AND_RETURN) {
			GameController.instance.EnterNeighbourhood();
		} else if (saveType == SaveType.SAVE_AND_QUIT) {
			Application.Quit();
		}
	}

	private void SaveProperty () {
		propertyController.property.name = saveNameField.text;
		propertyController.property.description = saveDescriptionField.text;
		propertyController.SaveProperty();
	}

	public void DontSaveButton () {
		CloseSaveDialog();
		if (saveType == SaveType.SAVE_AND_RETURN) {
			GameController.instance.EnterNeighbourhood();
		} else if (saveType == SaveType.SAVE_AND_QUIT) {
			Application.Quit();
		}
	}

	public void CloseSaveDialog () {
		savePanel.transform.localScale = new Vector3(0, 0, 0);
	}
}
