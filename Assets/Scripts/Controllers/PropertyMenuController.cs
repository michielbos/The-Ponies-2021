using Controllers.Global;
using Model.Property;
using UnityEngine;
using UnityEngine.UI;

public class PropertyMenuController : MonoBehaviour {
	public Button[] lotButtons;

	void Start () {
		PropertyLoader propertyLoader = new PropertyLoader();
		for (int i = 0; i < lotButtons.Length; i++) {
			if (propertyLoader.PropertyExists(i)) {
				PropertyData propertyData = propertyLoader.LoadProperty(i);
				lotButtons[i].GetComponentInChildren<Text>().text = i + ". " + propertyData.name + "\n(" + propertyData.streetName + ")";
			} else {
				lotButtons[i].GetComponentInChildren<Text>().text = "Empty lot (" + i + ")";
			}
		}
		DiscordController.Instance.UpdateActivity(DiscordController.DiscordState.Neighbourhood);
	}

	public void enterLot (int id) {
		GameController.Instance.EnterLot(id);
	}
}
