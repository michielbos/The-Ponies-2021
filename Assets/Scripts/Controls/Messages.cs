using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Messages : MonoBehaviour {
	public MessageWindow messageWindowPrefab;
	public AudioSource audioSource;
	private List<MessageWindow> messageWindows;

	public Messages () {
		messageWindows = new List<MessageWindow>();
	}

	public MessageWindow SendMessage (string title, string content) {
		return SendQuestion(title, content, "OK");
	}

	public MessageWindow sendConfirm (string title, string content) {
		return SendQuestion(title, content, "No", "Yes");
	}

	public MessageWindow SendQuestion (string title, string content, params string[] buttons) {
		MessageWindow window = Instantiate(messageWindowPrefab, transform);
		window.Initialize(this, audioSource, title, content, buttons);
		if (messageWindows.Count > 0) {
			window.gameObject.SetActive(false);
		}
		messageWindows.Add(window);
		return window;
	}

	public void OnMessageClosed () {
		messageWindows.RemoveAt(0);
		if (messageWindows.Count > 0)
			messageWindows[0].gameObject.SetActive(true);
	}
}
