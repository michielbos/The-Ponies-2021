using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HUDController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	/* Leaving this here in case it was needed after all... 
	 * Who the hell uses const ints instead of a proper 
	 * enum anyways? I don't want to see anything like that in 
	 * this codebase ever akain, understood? :P
	 *   - Ebunix 

	private const int PANEL_LIVE = 0;
	private const int PANEL_BUY = 1;
	private const int PANEL_BUILD = 2;
	private const int PANEL_CAMERA = 3;
	private const int PANEL_OPTIONS = 4;
	*/

	public enum HudPanel
	{
		None = -1,
		Live = 0,
		Buy = 1,
		Build = 2,
		Camera = 3,
		Options = 4
	}

	public Camera gameCamera;

	public AudioSource audioSource;

	public List<AudioClip> audioClips;

	public List<GameObject> panels;
	public List<GameObject> modeButtons;
	public List<GameObject> toothpicks;
	public List<GameObject> speedButtons;
	public List<GameObject> roofButtons;

	public Sprite[] pauseSprites;

	public CatalogController catalogController;
	public BuyController buyController;
	public MusicController musicController;

	private bool paused = false;
	private bool forcePaused = false;
	private int selectedSpeed = 1;
	private int selectedRoof = 1;
	private HudPanel selectedPanel = HudPanel.None;

	public Text fundsText;
	private int funds = 20000;
	private float pauseTimer = 0;
	private bool touchingGui = false;

	void Start()
	{
		UpdateSpeed();
		UpdateRoof();
		UpdateFunds();
	}

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

	public void ActivatePanel(int id)
	{
		ActivatePanel((HudPanel)id);
	}

	public void ActivatePanel(HudPanel panel)
	{
		int panelId = (int)panel;

		// Keep the game paused when building/buying
		forcePaused = panelId > 0;

		if (panels[panelId].activeSelf)
		{
			panels[panelId].SetActive(false);
			toothpicks[panelId].SetActive(false);
			selectedPanel = HudPanel.None;

			var button = modeButtons[panelId].GetComponent<Button>();
			ColorBlock block = button.colors;
			block.normalColor = Color.white;
			button.colors = block;

			speedButtons[0].GetComponent<Image>().overrideSprite = speedButtons[0].GetComponent<Image>().sprite;
			speedButtons[0].GetComponent<Button>().interactable = true;
			pauseTimer = 0f;
		}
		else
		{
			foreach (GameObject g in panels)
			{
				g.SetActive(panelId == panels.IndexOf(g));
			}
			foreach (GameObject g in modeButtons)
			{
				var button = g.GetComponent<Button>();
				ColorBlock block = button.colors;
				block.normalColor = panelId == modeButtons.IndexOf(g) ? block.highlightedColor : Color.white;
				button.colors = block;
			}
			foreach (GameObject g in toothpicks)
			{
				g.SetActive(panelId == toothpicks.IndexOf(g));
			}
			selectedPanel = panel;
			if (panel > 0) speedButtons[0].GetComponent<Button>().interactable = false;
			else
			{
				speedButtons[0].GetComponent<Image>().overrideSprite = speedButtons[0].GetComponent<Image>().sprite;
				speedButtons[0].GetComponent<Button>().interactable = true;
				pauseTimer = 0f;
			}
		}

		if (selectedPanel == HudPanel.Buy)
		{
			musicController.SwitchMusic(MusicType.BuyMode);
			buyController.enabled = true;
		}
		else
		{
			musicController.SwitchMusic(selectedPanel == HudPanel.Build ? MusicType.BuildMode : MusicType.NoMusic);
			catalogController.CloseCatalog();
			buyController.enabled = false;
		}
	}

	public void SetSpeed(int s)
	{
		if (forcePaused)
			return;

		PlaySpeedSound(paused ? 0 : selectedSpeed, paused ? s == 0 ? selectedSpeed : s : s);
		if (s > 0)
		{
			paused = false;
			selectedSpeed = s;
			UpdateSpeed();
		}
		else
		{
			paused = !paused;
			pauseTimer = 0f;
		}
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

	/// <summary>
	/// Get the current amount of funds.
	/// If this is an unoccuppied lot, this will return a large amount. (to be implemented)
	/// </summary>
	/// <returns>The current amount of funds.</returns>
	public int GetFunds()
	{
		return funds;
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

	public void OnPointerEnter (PointerEventData eventData) {
		touchingGui = true;
	}

	public void OnPointerExit (PointerEventData eventData) {
		touchingGui = false;
	}

	public bool IsMouseOverGui () {
		//I doubt how reliable this is, but it's not like we have anything better at the moment.
		return touchingGui;
	}
}
