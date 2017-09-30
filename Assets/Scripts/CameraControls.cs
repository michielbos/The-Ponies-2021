﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour {

	Vector3 panStart;
	Vector3 panStartPos;

	Camera camera;

	const float minSize = 1;
	const float maxSize = 32;

	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire3"))
		{
			panStart = Input.mousePosition;
			panStartPos = transform.position;
		}
		if (Input.GetButton("Fire3"))
		{
			transform.position = panStartPos + LevelVector(transform.forward) * (Input.mousePosition - panStart).y * 6 * camera.orthographicSize / Screen.height
											 + LevelVector(transform.right) * (Input.mousePosition - panStart).x * 4 * camera.orthographicSize / Screen.width;
		}

		camera.orthographicSize -= camera.orthographicSize * Input.mouseScrollDelta.y/10;
		camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minSize, maxSize);
	}

	Vector3 LevelVector (Vector3 v)
	{
		Vector3 v2 = v;
		v2.y = 0;
		return v2;
	}
}
