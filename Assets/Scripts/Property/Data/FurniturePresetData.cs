﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FurniturePresetData {
	public int id;
	public string name;
	public string description;
	public int price;
	public int category;

	public FurniturePresetData (int id, string name, string description, int price, int category) {
		this.id = id;
		this.name = name;
		this.description = description;
		this.price = price;
		this.category = category;
	}
}
