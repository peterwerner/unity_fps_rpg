using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 *	Back-end for grid style inventory
 */
public class InventoryGrid : Inventory {

	[SerializeField] private int numTilesX = 16, numTilesY = 8;
	[SerializeField] private int numHotkeys;
	private bool[,] gridFills;			// True if an item is taking up this spot in the grid
	private Equipment[] hotkeyItems;	// Items assigned to the 1-9 hotkeys


	new void Start() {
		base.BaseStart();

		gridFills = new bool[numTilesX, numTilesY];
		hotkeyItems = new Equipment[numHotkeys];
	}


	new void Update () {
		base.BaseUpdate();
	}


	/**
	 * Attempt to add an item to this grid in any spot that it will fit
	 * Return false if no space
	 */
	public override bool AddItem(Equipment item)
	{
		InventoryGridItem gridItem = item.inventoryGridItem;
		for (int i = 0; i + gridItem.width <= numTilesX; i++) {
			for (int j = 0; j + gridItem.height <= numTilesY; j++) {
				// If the item fits, add it to the inventory
				if (CanItemFit(item.inventoryGridItem, i, j)) {
					bool inserted = base.AddItem(item);
					// If it was successfully added, add it to the grid
					if (inserted) {
						gridItem.x = i;
						gridItem.y = j;
						for (int k = 0; k < gridItem.width; k++) 
							for (int q = 0; q < gridItem.height; q++) 
								gridFills[gridItem.x + k, gridItem.y + q] = true;
						// Auto add it to an available hotkey slot if the item is flagged to do so
						if (item.inventoryGridItem.autoAddToHotkey) {
							for (int q = 0; q < numHotkeys; q++) {
								if (hotkeyItems[q] == null) {
									hotkeyItems[q] = item;
									break;
								}
							}
						}
					}
					return inserted;
				}
			}
		}
		return false;
	}


	/**
	 *	Delete item from the inventory 
	 */
	public override void DeleteItem(Equipment item)
	{
		InventoryGridItem gridItem = item.inventoryGridItem;
		for (int k = 0; k < gridItem.width; k++) 
			for (int q = 0; q < gridItem.height; q++) 
				gridFills[gridItem.x + k, gridItem.y + q] = false;

		for (int i = 0; i < numHotkeys; i++)
			if (hotkeyItems[i] == item)
				hotkeyItems[i] = null;

		if (currentItem == item)
			currentItem = null;

		base.DeleteItem(item);
	}


	/**
	 * 	Attempt to move an item to a new point on the grid
	 * 	Return true / false for success (ie: false if it doesn't fit into the new position)
	 */
	public bool MoveItem(Equipment item, int i, int j)
	{
		if (!CanItemFit(item.inventoryGridItem, i, j))
			return false;
		
		for (int k = 0; k < item.inventoryGridItem.width; k++) 
			for (int q = 0; q < item.inventoryGridItem.height; q++) 
				gridFills[item.inventoryGridItem.x + k, item.inventoryGridItem.y + q] = false;
		
		item.inventoryGridItem.x = i;
		item.inventoryGridItem.y = j;

		for (int k = 0; k < item.inventoryGridItem.width; k++) 
			for (int q = 0; q < item.inventoryGridItem.height; q++) 
				gridFills[item.inventoryGridItem.x + k, item.inventoryGridItem.y + q] = true;
		
		return true;
	}


	/**
	 * Given an item and a tile coordinate pair, can it fit in that position?
	 */
	public bool CanItemFit(InventoryGridItem item, int i, int j)
	{
		for (int k = 0; k < item.width; k++) {
			for (int q = 0; q < item.height; q++) {
				if (i < 0 || j < 0 || i + k >= numTilesX || j + q >= numTilesY)
					return false;
				Equipment targetItem = ItemAtGridPoint(i + k, j + q);
				if ((gridFills[i + k, j + q] && targetItem != item.GetEquipment()))
					return false;
			}
		}
		return true;
	}


	/**
	 * Given a tile coordinate pair, return the item sitting at that point
	 * Return null if no item at that point
	 */
	public Equipment ItemAtGridPoint(int i, int j)
	{
		if (i < 0 || j < 0 || i >= numTilesX || j >= numTilesY || !gridFills[i,j])
			return null;
		foreach (Equipment equipment in GetItems()) {
			if (equipment.inventoryGridItem.IsOnCoords(i, j))
				return equipment;
		}
		return null;
	}


	public Equipment GetHotkeyItem(int index)
	{
		if (index < 0 || index >= numHotkeys)
			return null;
		return hotkeyItems[index];
	}
	public bool SetHotkeyItem(int index, Equipment item)
	{
		if (index < 0 || index >= numHotkeys)
			return false;
		hotkeyItems[index] = item;
		return true;
	}

	public int getNumHotkeys() { return numHotkeys; }


	public int getNumTilesY() { return numTilesY; }
	public int getNumTilesX() { return numTilesX; }
}
