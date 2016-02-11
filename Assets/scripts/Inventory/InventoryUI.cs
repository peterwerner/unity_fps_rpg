using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(InventoryGrid))]
public class InventoryUI : MonoBehaviour {

	public GUISkin GUIskin;
	public Texture2D tileTexture, tileTextureActive;
	private bool isEnabled = false;
	[SerializeField] private float padding_y = 0.2f;
	[SerializeField] private float padding_x = 0.2f;
	[SerializeField] private float tilePadding = 0.05f;
	private int numTilesX, numTilesY;
	private int numHotkeys;
	private float invWidthToHeightRatio;
	private float invWidth, invHeight;
	private float invX, invY;
	private Rect[,] tileRects;	
	private Rect[] hotkeyTileRects;	
	private InventoryGrid inventoryGrid;
	private int screenWidthPrev, screenHeightPrev;

	// State information
	private Equipment itemSelected;				// Item currently selected
	private Equipment itemMouseHover;			// Itenabled mouse currently hoveenableder it
	private Equipment itemMoving;				// Item currently being moved to aenabledent position
	private InventoryGridItem itemMovingDummy;	// Dummy item to illustrate the item actively moving around on the grid
	private Vector3 dummyOffsetPixels;			// Offset in pixels from the cornder of  an item when click-and-dragged to move


	void Start ()
	{
		inventoryGrid = GetComponent<InventoryGrid>();
		numTilesX = inventoryGrid.getNumTilesX();
		numTilesY = inventoryGrid.getNumTilesY();
		numHotkeys = inventoryGrid.getNumHotkeys();
		screenWidthPrev = Screen.width;
		screenHeightPrev = Screen.height;
		UpdateTiles();
	}


	void OnGUI ()
	{
		if (isEnabled) 
		{
			GUI.skin = GUIskin;

			DrawTiles();
			DrawItems();
		}
	}


	void Update()
	{
		// Respond to screen size changing
		if (Screen.width != screenWidthPrev || Screen.height != screenHeightPrev)
			UpdateTiles();
		screenWidthPrev = Screen.width;
		screenHeightPrev = Screen.height;

		if (isEnabled) 
		{	
			/* Determine which item the mouse is hovering over */
			int[] mouseCoords = MouseToTileCoords();
			itemMouseHover = null;
			if (mouseCoords[0] >= 0 && mouseCoords[1] >= 0) {
				Equipment item = inventoryGrid.ItemAtGridPoint(mouseCoords[0], mouseCoords[1]);
				if (item)
					itemMouseHover = item;
			}

			/* Select item */
			if (CrossPlatformInputManager.GetButtonDown("Fire1")) {
				if (itemMouseHover)
					itemSelected = itemMouseHover;
				else
					itemSelected = inventoryGrid.GetHotkeyItem(MousePosToHotkey());
			}

			/* Remove item from hotkey */
			if (CrossPlatformInputManager.GetButtonDown("Fire2"))
				inventoryGrid.SetHotkeyItem(MousePosToHotkey(), null);

			/* Click-and-drag to move items around */
			if (CrossPlatformInputManager.GetButtonDown("Fire1") && itemMouseHover) {
				itemMoving = itemMouseHover;
				itemMovingDummy = new InventoryGridItem(itemMoving.inventoryGridItem);
				// Set pixel coords offset (mouse pos - corner of item)
				Rect corner = tileRects[itemMoving.inventoryGridItem.x, numTilesY - 1 - itemMoving.inventoryGridItem.y];
				dummyOffsetPixels.x = Input.mousePosition.x - corner.center.x;
				dummyOffsetPixels.y = Input.mousePosition.y - corner.center.y;
			}
			// Update dummy item's tile coords when user is moving an item around the grid 
			// If not on the grid, set to (-1, -1)
			if (itemMoving) {
				int[] tileCoords = MouseToTileCoords_Unconstrained(Input.mousePosition - dummyOffsetPixels);
				if (tileCoords[0] < 0 || tileCoords[0] > numTilesX - itemMovingDummy.width 
					|| tileCoords[1] < 0 || tileCoords[1] > numTilesY - itemMovingDummy.height) 
				{
					itemMovingDummy.x = -1;
					itemMovingDummy.y = -1;
				}
				else {
					itemMovingDummy.x = tileCoords[0];
					itemMovingDummy.y = tileCoords[1];
				}
			}
			// Attempt to drop an item off when user let's go after click-and-drag
			// (either into a new spot in the inventory grid or into a hotkey slot)
			if (CrossPlatformInputManager.GetButtonUp("Fire1")) {
				if (itemMoving) {
					bool moved = inventoryGrid.MoveItem(itemMoving, itemMovingDummy.x, itemMovingDummy.y);
					if (!moved)
						inventoryGrid.SetHotkeyItem(MousePosToHotkey(), itemMoving);
				}
				itemMoving = null;
			}

		}
	}


	/**
	 * Calculate inventory dimensions based on current screen size
	 * Create a 2d array of Rects representing each square in the grid and in the hotkey list
	 */
	void UpdateTiles()
	{
		invWidthToHeightRatio = (float)numTilesX / numTilesY;

		// Calculate inventory dimensions based on screen size
		invHeight = Screen.height - 2 * padding_y * Screen.height;
		invWidth = invWidthToHeightRatio * invHeight;
		if (invWidth > Screen.width - 2 * padding_x * Screen.width) {
			invWidth = Screen.width - 2 * padding_x * Screen.width;
			invHeight = invWidth / invWidthToHeightRatio;
		}
		invX = (Screen.width - invWidth) / 2;
		invY = (Screen.height - invHeight) / 2;

		float tileWidth = (invWidth - ((numTilesX - 1) * tilePadding)) / numTilesX;
		float tileHeight = (invHeight - ((numTilesY - 1) * tilePadding)) / numTilesY;

		// Create tile rects in the main inventory grid
		tileRects = new Rect[numTilesX, numTilesY];
		for (int i = 0; i < numTilesX; i++) {
			for (int j = 0; j < numTilesY; j++) 
			{
				Rect rect = new Rect(
					invX + i * (tileWidth + tilePadding),
					invY + j * (tileHeight + tilePadding),
					tileWidth,
					tileHeight
				);
				tileRects[i,j] = rect;
			}
		}

		// Create tile rects for the hotkeys
		hotkeyTileRects = new Rect[numHotkeys];
		float hotkeysX = (Screen.width - (tileWidth * numHotkeys) - (tilePadding * (numHotkeys - 1))) / 2;
		float hotkeysY = Screen.height - invY + (tileHeight / 2);
		for (int i = 0; i < numHotkeys; i++) {
			Rect rect = new Rect(
				hotkeysX + i * (tileWidth + tilePadding),
				hotkeysY,
				tileWidth,
				tileHeight
			);
			hotkeyTileRects[i] = rect;
		}
	}


	// Draw the background tiles
	private void DrawTiles()
	{
		for (int i = 0; i < numTilesX; i++) {
			for (int j = 0; j < numTilesY; j++) 
			{
				if (itemSelected && itemSelected.inventoryGridItem.IsOnCoords(i, j) && !itemMoving)
					GUI.DrawTexture(tileRects[i,j], tileTextureActive);
				else
					GUI.DrawTexture(tileRects[i,j], tileTexture);
			}
		}
		for (int i = 0; i < numHotkeys; i++) {
			if (itemSelected && itemSelected == inventoryGrid.GetHotkeyItem(i))
				GUI.DrawTexture(hotkeyTileRects[i], tileTextureActive);
			else
				GUI.DrawTexture(hotkeyTileRects[i], tileTexture);
		}
	}


	// Draw the item textures, fitted to the background tiles
	private void DrawItems()
	{
		// Draw all items in inventory
		List<Equipment> equipments = inventoryGrid.GetItems();
		foreach (Equipment equipment in equipments) 
		{
			// If item is currently being moved, draw it at partial opacity
			float alpha = GUI.color.a;
			if (itemMoving && equipment == itemMoving)
				SetAlpha(0.4f);
			GUI.DrawTexture(RectFromGridItem(equipment.inventoryGridItem), equipment.inventoryGridItem.texture);
			SetAlpha(alpha);
		}

		// Draw all hotkey items
		// Exception: in the special case where an item is being moved onto the hotkey in question
		for (int i = 0; i < numHotkeys; i++) {
			Equipment item = inventoryGrid.GetHotkeyItem(i);
			if (item && !(itemMoving && MousePosToHotkey() == i))
				GUI.DrawTexture(hotkeyTileRects[i], item.inventoryGridItem.textureHotkey);
		}

		// Draw dummy item if an item is being moved
		if (itemMoving) 
		{
			// If the item is dragged onto a hotkey tile, draw it on the hotkey tile
			int hotKeyIndex = MousePosToHotkey();
			if (hotKeyIndex >= 0) {
				GUI.DrawTexture(hotkeyTileRects[hotKeyIndex], itemMoving.inventoryGridItem.textureHotkey);
			}
			// Otherwise, draw the item being moved around
			else
			{
				// If the item is in outside bounds of the grid, let it follow the mouse freely
				Rect posRect;
				if (itemMovingDummy.x < 0 || itemMovingDummy.y < 0 || itemMovingDummy.x > numTilesX - itemMovingDummy.width || itemMovingDummy.y > numTilesY - itemMovingDummy.height) {
					float tileWidth = tileRects[0,0].width;
					float tileHeight = tileRects[0,0].height;
					posRect = new Rect(
						(Input.mousePosition - dummyOffsetPixels).x - (tileRects[0,0].width / 2),
						Screen.height - (Input.mousePosition - dummyOffsetPixels).y - (tileRects[0,0].height / 2),
						(tileWidth * itemMovingDummy.width) + (tilePadding * (itemMovingDummy.width - 1)),
						(tileHeight * itemMovingDummy.height) + (tilePadding * (itemMovingDummy.height - 1))
					);
				}
				// Otherwise, if the item is within the grid, snap it to the grid tiles
				else
					posRect = RectFromGridItem(itemMovingDummy);
				
				// Tint red if it is in a spot where it doesn't fit, and will not be successfully moved
				Color color = GUI.color;
				if (!inventoryGrid.CanItemFit(itemMovingDummy, itemMovingDummy.x, itemMovingDummy.y))
					GUI.color = Color.red;
				SetAlpha(0.75f);
				GUI.DrawTexture(posRect, itemMovingDummy.texture);
				GUI.color = color;
			}
		}
	}



	private Rect RectFromGridItem(InventoryGridItem item)
	{
		int x = item.x;
		int y = item.y;
		Rect rectStart = tileRects[x, y];
		Rect rectEnd = tileRects[ x + item.width - 1, y + item.height - 1 ];
		Rect rect = new Rect(
			rectStart.x,
			rectStart.y,
			rectEnd.x - rectStart.x + rectEnd.width,
			rectEnd.y - rectStart.y + rectEnd.height
		);
		return rect;
	}


	/**
	 * Return the tile coordinates of the mouse's current position
	 * Return negative coordinates if the mouse is not over a tile
	 */
	private int[] MouseToTileCoords()
	{
		// Mouse is out of tile bounds, return (-1,-1)
		if (Input.mousePosition.x < invX || Input.mousePosition.y < invY || Input.mousePosition.x > invX + invWidth || Input.mousePosition.y > invY + invHeight) {
			int[] coords = new int[2];
			coords[0] = -1;
			coords[1] = -1;
			return coords;
		}
		// Mouse is within tile bounds
		return MouseToTileCoords_Unconstrained();
	}
	/**
	 *	Return the effective tile coordinates of the mouse's current location, even if the coords are out of range
	 */
	private int[] MouseToTileCoords_Unconstrained() { return MouseToTileCoords_Unconstrained(Input.mousePosition); }
	private int[] MouseToTileCoords_Unconstrained(Vector3 mousePos)
	{
		int[] coords = new int[2];
		coords[0] = (int)((mousePos.x - invX) * numTilesX / invWidth);
		// (0 px, 0 px) is the bottom left of screen, so we have to flip the y tile coordinate
		coords[1] = (numTilesY - 1) - (int)((mousePos.y - invY) * numTilesY / invHeight); 
		return coords;
	}


	/**
	 * Return the index of the hotkey rect that the mouse is hovering over.
	 * Return -1 if the pixel coordinate pair is not on a hotkey rect.
	 */
	private int MousePosToHotkey()
	{
		Vector3 mousePos = new Vector3(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 0);
		for (int i = 0; i < numHotkeys; i++) {
			if (hotkeyTileRects[i].Contains(mousePos))
				return i;
		}
		return -1;
	}


	// Change GUI opacity. Affects anything drawn after the change.
	private void SetAlpha(float alpha)
	{
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
	}


	public bool IsEnabled()
	{
		return isEnabled;
	}
	public void Enable()
	{
		isEnabled = true;
	}
	public void Disable()
	{
		isEnabled = false;
	}

}
