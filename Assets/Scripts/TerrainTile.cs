using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TerrainTile : MonoBehaviour, IPointerDownHandler
{
	[HideInInspector]
	public int x;
	[HideInInspector]
	public int y;

	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("Wasdf");
		GetComponent<MeshRenderer>().material = null;
	}
}
