using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

	[SerializeField] private GUISkin GUIskin;
	[SerializeField] private PlayerStats playerStats;
	[SerializeField] private Inventory inventory;
	[SerializeField] private float cornerStatsWidth;
	[SerializeField] private Texture2D leftStatsTex, rightStatsTex;

	private GUIStyle leftStatsStyle, rightStatsStyle;
	private float cornerStatsHeightToWidth = 0.5f;
	private Rect leftStatsRect, rightStatsRect;
	private int screenWidthPrev, screenHeightPrev;
	private string leftStatsString, rightStatsString;


	// Use this for initialization
	void Start () 
	{
		leftStatsStyle = new GUIStyle();					rightStatsStyle = new GUIStyle();
		leftStatsStyle.alignment = TextAnchor.LowerLeft;	rightStatsStyle.alignment = TextAnchor.LowerRight;
		screenWidthPrev = Screen.width;
		screenHeightPrev = Screen.height;
		UpdateDimensions();
	}


	// Update is called once per frame
	void Update () 
	{
		// Respond to screen size changing
		if (Screen.width != screenWidthPrev || Screen.height != screenHeightPrev) 
			UpdateDimensions();
		screenWidthPrev = Screen.width;
		screenHeightPrev = Screen.height;

		leftStatsString = playerStats.health + "|" + playerStats.energy;

		GunHitscan gun = null;
		if (inventory.GetCurrentItem())
			gun = inventory.GetCurrentItem().GetComponent<GunHitscan>();
		if (gun) {
			rightStatsString = gun.GetNumBulletsLoaded() + "|" + gun.GetNumClips();
		}
		else
			rightStatsString = null;
	}

	void UpdateDimensions()
	{
		float w = Screen.width * cornerStatsWidth;
		float h = w * cornerStatsHeightToWidth;
		leftStatsRect = new Rect(0, Screen.height - h, w, h);
		rightStatsRect = new Rect(Screen.width - w, Screen.height - h, w, h);
	}


	void OnGUI ()
	{
		GUI.skin = GUIskin;

		if (leftStatsTex)
			GUI.DrawTexture(leftStatsRect, leftStatsTex);
		if (rightStatsTex)
			GUI.DrawTexture(rightStatsRect, rightStatsTex);

		if (leftStatsString != null)
			GUI.Label(leftStatsRect, leftStatsString, leftStatsStyle);
		if (rightStatsString != null)
			GUI.Label(rightStatsRect, rightStatsString, rightStatsStyle);
	}

}
