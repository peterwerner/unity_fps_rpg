using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class InventoryGridItem {

	public Texture2D texture, textureHotkey;
	[Tooltip("Width and height in terms of number tiles")]
	[Range(1,10)] public int width = 1, height = 1;
	public bool autoAddToHotkey = false;
	[SerializeField] private Equipment equipment;

	[HideInInspector] public int x, y;	// x,y coords of the top left corner in terms of tiles

	public InventoryGridItem(InventoryGridItem copy)
	{
		this.texture = copy.texture;
		this.textureHotkey = copy.textureHotkey;
		this.width = copy.width;
		this.height = copy.height;
		this.x = copy.x;
		this.y = copy.y;
		this.equipment = copy.GetEquipment();
	}


	public bool IsOnCoords(int i, int j)
	{
		return (i >= x && j >= y && i < x + width && j < y + height);
	}

	public Equipment GetEquipment()
	{
		return equipment;
	}
		
}
