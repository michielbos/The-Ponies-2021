using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAPController : MonoBehaviour {

	public Button[] genderButtons;
	public Button[] raceButtons;
	public Button[] stageButtons;

	public const int FEMALE=0, MALE=1, EARTH=0, PEGASUS=1, UNICORN=2, MARYSUE=3, ADULT=0, CHILD=1;

	int selectedGender = MALE;
	int selectedRace = EARTH;
	int selectedStage = ADULT;

	// Use this for initialization
	void Start () {
		UpdateButtons();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SetGender(int g)
	{
		selectedGender = g;
		UpdateButtons();
	}

	public void SetRace(int r)
	{
		selectedRace = r;
		UpdateButtons();
	}

	public void SetStage(int s)
	{
		selectedStage = s;
		UpdateButtons();
	}

	void UpdateButtons()
	{
		int bi = -1;
		foreach (Button b in genderButtons)
		{
			bi++;
			b.interactable = bi != selectedGender;
		}

		bi = -1;
		foreach (Button b in raceButtons)
		{
			bi++;
			b.interactable = bi != selectedRace;
		}

		bi = -1;
		foreach (Button b in stageButtons)
		{
			bi++;
			b.interactable = bi != selectedStage;
		}
	}
}
