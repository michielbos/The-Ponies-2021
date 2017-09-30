using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {

	/*public enum Mode
	{
		Live = 0,
		Buy = 1,
		Build = 2,
		Cam = 3,
		Opt = 4
	}*/

	public AudioSource audioSource;

	public List<AudioClip> audioClips;
	
	public List<GameObject> panels;
	public List<GameObject> toothpicks;
	public List<GameObject> speedButtons;
	public List<GameObject> roofButtons;

	int selectedSpeed = 1;
	int selectedRoof = 1;

	public Text fundsText;
	int funds = 1337;

	void Start()
	{
		UpdateSpeed();
		UpdateRoof();
		UpdateFunds();
	}

	public void ActivatePanel(int m)
	{
		if (panels[m].activeSelf)
		{
			panels[m].SetActive(false);
			toothpicks[m].SetActive(false);
		}
		else
		{
			foreach (GameObject g in panels)
			{
				g.SetActive(m == panels.IndexOf(g));
			}
			foreach (GameObject g in toothpicks)
			{
				g.SetActive(m == toothpicks.IndexOf(g));
			}
		}
	}

	public void SetSpeed(int s)
	{
		PlaySpeedSound(selectedSpeed, s);
		selectedSpeed = s;
		UpdateSpeed();
	}

	public void SetRoof(int r)
	{
		selectedRoof = r;
		UpdateRoof();
	}
	
	public void SetFunds(int a)
	{
		funds = a;
		UpdateFunds();
	}

	/**
	 * <summary>
	 * Negative amounts reduces funds. Debug command
	 * for visually changing amount of funds.
	 * </summary>
	 */
	public void ChangeFunds(int a)
	{
		funds += a;
		UpdateFunds();
	}

	private void UpdateSpeed()
	{
		foreach (GameObject g in speedButtons)
		{
			bool active = speedButtons.IndexOf(g) == selectedSpeed;
			Button b = g.GetComponent<Button>();
			b.interactable = !active;
		}
	}

	private void UpdateRoof()
	{
		foreach (GameObject g in roofButtons)
		{
			bool active = roofButtons.IndexOf(g) == selectedRoof;
			Button b = g.GetComponent<Button>();
			b.interactable = !active;
		}
	}

	private void PlaySpeedSound(int f, int t)
	{
		string ff = f.ToString();
		string tf = t.ToString();
		if (ff == "0") ff = "P";
		if (tf == "0") tf = "P";
		string clipName = "UI_SPEED_" + ff + "TO" + tf;

		foreach (AudioClip c in audioClips)
		{
			if (c.name == clipName)
			{
				audioSource.PlayOneShot(c);
			}
		}
	}

	private void UpdateFunds()
	{
		fundsText.text = "$" + funds.ToString();
	}
}
