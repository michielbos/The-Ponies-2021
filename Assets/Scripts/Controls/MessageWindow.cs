using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : MonoBehaviour {
	public AudioClip clickSound;
	public Text titleText;
	public Text contentText;
	public Button firstButton;
	private Messages messages;
	private AudioSource audioSource;

	public void Initialize (Messages messages, AudioSource audioSource, string title, string content, string[] buttonStrings) {
		this.messages = messages;
		this.audioSource = audioSource;
		titleText.text = title;
		contentText.text = content;
		SetupButtons(buttonStrings);
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
		}
	}

	public void CloseWindow () {
		messages.OnMessageClosed(0);
		Destroy(gameObject);
		audioSource.PlayOneShot(clickSound);
	}
}
