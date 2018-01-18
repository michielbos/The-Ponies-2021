using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
	private const int PANEL_LIVE = 0;
	private const int PANEL_BUY = 1;
	private const int PANEL_BUILD = 2;
	private const int PANEL_CAMERA = 3;
	private const int PANEL_OPTIONS = 4;

	/*public enum Mode
	{
		Live = 0,
		Buy = 1,
		Build = 2,
		Cam = 3,
		Opt = 4
	}*/

	public Camera gameCamera;

	public AudioSource audioSource;

	public List<AudioClip> audioClips;

	public List<GameObject> panels;
	public List<GameObject> toothpicks;
	public List<GameObject> speedButtons;
	public List<GameObject> roofButtons;

	public Sprite[] pauseSprites;

	public BuyController buyController;

	bool paused = false;
	bool forcePaused = false;
	int selectedSpeed = 1;
	int selectedRoof = 1;
	int selectedPanel = -1;

	public Text fundsText;
	int funds = 1337;

	void Start()
	{
		UpdateSpeed();
		UpdateRoof();
		UpdateFunds();
	}

	float pauseTimer = 0;
	void Update()
	{
		if (forcePaused || selectedPanel > 0)
		{
			pauseTimer += Time.deltaTime;
			speedButtons[0].GetComponent<Image>().overrideSprite = pauseSprites[(int)Mathf.Floor(pauseTimer % 1f * 2) * 2];
		}
		else if (paused)
		{
			pauseTimer += Time.deltaTime;
			speedButtons[0].GetComponent<Image>().overrideSprite = pauseSprites[(int)Mathf.Floor(pauseTimer % 1f * 2)];
		}
	}

	public void ActivatePanel(int m)
	{
		if (panels[m].activeSelf)
		{
			panels[m].SetActive(false);
			toothpicks[m].SetActive(false);
			selectedPanel = -1;
			speedButtons[0].GetComponent<Image>().overrideSprite = speedButtons[0].GetComponent<Image>().sprite;
			pauseTimer = 0f;
			speedButtons[0].GetComponent<Button>().interactable = true;
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
			selectedPanel = m;
			if (m > 0) speedButtons[0].GetComponent<Button>().interactable = false;
			else { speedButtons[0].GetComponent<Image>().overrideSprite = speedButtons[0].GetComponent<Image>().sprite; pauseTimer = 0f; speedButtons[0].GetComponent<Button>().interactable = true; }
		}
		buyController.enabled = selectedPanel == PANEL_BUY || selectedPanel == PANEL_BUILD;
	}

	public void SetSpeed(int s)
	{
		PlaySpeedSound(paused ? 0 : selectedSpeed, paused ? s == 0 ? selectedSpeed : s : s);
		if (s > 0)
		{
			paused = false;
			selectedSpeed = s;
			UpdateSpeed();
		}
		else { paused = !paused; pauseTimer = 0f; }
	}

	public void SetRoof(int r)
	{
		selectedRoof = r;
		UpdateRoof();
	}

	public void Zoom(int dir)
	{
		gameCamera.orthographicSize *= dir == -1 ? .5f : dir == 1 ? 2 : 1;
	}

	public void Rotate(bool cc)
	{
		gameCamera.GetComponent<CameraControls>().Rotate(cc);
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
		speedButtons[0].GetComponent<Image>().overrideSprite = speedButtons[0].GetComponent<Image>().sprite;
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
