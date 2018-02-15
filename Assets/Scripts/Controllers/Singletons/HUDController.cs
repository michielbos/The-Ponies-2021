using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HUDController : SingletonMonoBehaviour<HUDController>, IPointerEnterHandler, IPointerExitHandler
{
	public List<GameObject> speedButtons;
	public List<GameObject> roofButtons;
	public Sprite[] pauseSprites;
	
	private int TEMP_selectedRoof = 1;

	public Text fundsText;
	private bool touchingGui;

	void Start()
	{
		UpdateSpeed();
		UpdateRoof();
		UpdateFunds();
	}

	// Called from Unity GUI Button
	public void ActivatePanel(int index)
	{
		SoundController.Instance.PlaySound(SoundType.Click);
		SoundController.Instance.PlaySound(SoundType.Woosh);
		ModeController.Instance.SwitchMode((HudPanel)index);
	}

	// Called from Unity GUI Button
	public void SetRoofButton(int index)
	{
		// TODO: Implement a proper controller for this
		SoundController.Instance.PlaySound(SoundType.Click);
		TEMP_selectedRoof = index;
		UpdateRoof();
	}

	// Called from Unity GUI Button
	public void SetSpeed(int index)
	{
		SpeedController.Instance.SetSpeed((Speed)index);
	}

	// Called from Unity GUI Button
	public void Zoom(int direction)
	{
		SoundController.Instance.PlaySound(SoundType.Click);
		CameraController.Instance.Zoom(direction);
	}

	// Called from Unity GUI Button
	public void Rotate(bool counterClockwise)
	{
		SoundController.Instance.PlaySound(SoundType.Click);
		CameraController.Instance.Rotate(counterClockwise);
	}

	public void UpdateSpeed()
	{
		speedButtons[0].GetComponent<Image>().overrideSprite = speedButtons[0].GetComponent<Image>().sprite;
		foreach (GameObject g in speedButtons)
		{
			bool active = speedButtons.IndexOf(g) == (int)SpeedController.Instance.CurrentSpeed;
			g.GetComponent<Button>().interactable = !active;
		}
	}

	public void UpdatePauseBlink(float timer)
	{
		speedButtons[0].GetComponent<Image>().overrideSprite = pauseSprites[(int)Mathf.Floor(timer % 2)];
	}

	private void UpdateRoof()
	{
		foreach (GameObject g in roofButtons)
		{
			bool active = roofButtons.IndexOf(g) == TEMP_selectedRoof;
			g.GetComponent<Button>().interactable = !active;
		}
	}

	public void UpdateFunds()
	{
		fundsText.text = "$" + MoneyController.Instance.Funds;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		touchingGui = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		touchingGui = false;
	}

	public bool IsMouseOverGui()
	{
		//I doubt how reliable this is, but it's not like we have anything better at the moment.
		return touchingGui;
	}
}

public enum HudPanel
{
	None = -1,
	Live = 0,
	Buy = 1,
	Build = 2,
	Camera = 3,
	Options = 4
}