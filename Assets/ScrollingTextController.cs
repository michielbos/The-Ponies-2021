using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingTextController : MonoBehaviour {

	public GameObject text;

	private string[] lines;

	void Start()
	{
		TextAsset data = Resources.Load<TextAsset>("scrolling_lines");
		Debug.Log(data);
		lines = data.text.Split('\n');
		Debug.Log(data.text);
		Debug.Log("-------------");
		foreach (string l in lines)
		{
			Debug.Log(l);
		}
	}

	void Update()
	{
		text.transform.Translate(-200 * Time.deltaTime, 0, 0);
		if (text.transform.localPosition.x < -500)
		{
			Randomize();
			text.transform.Translate(1000, 0, 0);
		}
	}

	public void Randomize()
	{
		text.GetComponent<Text>().text = lines[(int)Mathf.Floor(Random.Range(0, lines.Length))];
	}
}
