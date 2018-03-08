using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : MonoBehaviour {
	public AudioClip clickSound;
	public Text titleText;
	public Text contentText;
	public Button firstButton;
	public Image image;
	public Sprite imageSprite;
	private Messages messages;
	private AudioSource audioSource;

	public delegate void WindowClosedListener(int button);
	public WindowClosedListener windowClosedListener;

	public void Initialize (Messages messages, AudioSource audioSource, string title, string content, string[] buttonStrings) {
		this.messages = messages;
		this.audioSource = audioSource;
		titleText.text = title;
		contentText.text = content;
		SetupButtons(buttonStrings);
		if (imageSprite != null) {
			SetupImage();
		}
	}

	void SetupImage () {
		image.gameObject.SetActive(true);
		image.sprite = imageSprite;
		RectTransform imageTransform = image.GetComponent<RectTransform>();
		imageTransform.sizeDelta = new Vector2(imageSprite.texture.width, imageSprite.texture.height);
		RectTransform contentTransform = contentText.GetComponent<RectTransform>();
		Vector2 contentOffset = contentTransform.offsetMin;
		contentOffset.x += imageTransform.anchoredPosition.x + imageSprite.texture.width;
		contentTransform.offsetMin = contentOffset;
	}

	void SetupButtons (string[] buttonStrings) {
		Button button = firstButton;
		float width = GetComponent<RectTransform>().sizeDelta.x - firstButton.GetComponent<RectTransform>().sizeDelta.x / 4;
		float widthPerButton = width / buttonStrings.Length;
		for (int i = 0; i < buttonStrings.Length; i++) {
			if (i > 0) {
				button = Instantiate(button, transform);
			}
			button.GetComponentInChildren<Text>().text = buttonStrings[i];
			RectTransform buttonTransform = button.GetComponent<RectTransform>();
			Vector3 pos = buttonTransform.anchoredPosition;
			pos.x = widthPerButton * (i + 0.5f) - width / 2; 
			buttonTransform.anchoredPosition = pos;
			int buttonId = i;
			button.onClick.AddListener(delegate{CloseWindow(buttonId);});
		}
	}

	public void CloseWindow (int button) {
		messages.OnMessageClosed();
		Destroy(gameObject);
		audioSource.PlayOneShot(clickSound);
		if (windowClosedListener != null) {
			windowClosedListener(button);
		}
	}
}
