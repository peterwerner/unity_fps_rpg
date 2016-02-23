using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

/**
 * Controls the inventory UI
 */
public class PauseUI : MonoBehaviour {

	public enum State { INVENTORY, OBJECTIVES };

	[SerializeField] private GUISkin GuiSkin;
	[SerializeField] private bool pauseGame = true;
	[SerializeField] private Crosshair crosshair;
	[SerializeField] private Camera cam;
	[SerializeField] private FirstPersonController controller;
	[SerializeField] BaseUI[] subInterfaces;
	[SerializeField] [Range(0f, 1f)] private float heightTopBanner = 0.1f, paddingBannerSide = 0.1f, alphaTopBanner = 0.5f;
	private bool isActive = false;
	private float timeScalePrev;
	private CameraFx cameraFx;
	private Texture2D topBannerTexture;
	private GUIStyle bannerStyle;
	private Rect topBannerRect;
	private int screenWidthPrev, screenHeightPrev;
	private Rect[] subIfaceRects;
	private int subIfaceCurrent, subIfaceHover;
	private Color colorSelected = Color.white, colorUnselected = Color.gray;


	// Use this for initialization
	void Start () 
	{
		subIfaceRects = new Rect[subInterfaces.Length];
		screenWidthPrev = Screen.width;
		screenHeightPrev = Screen.height;
		cameraFx = cam.GetComponent<CameraFx>();
		topBannerTexture = new Texture2D(1, 1);
		topBannerTexture.SetPixel(0, 0, Color.black);
		topBannerTexture.wrapMode = TextureWrapMode.Repeat;
		topBannerTexture.Apply();
		bannerStyle = new GUIStyle();
		bannerStyle.alignment = TextAnchor.MiddleCenter;
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

		subIfaceHover = -1;
		for (int i = 0; i < subIfaceRects.Length; i++) {
			Vector3 mousePos = new Vector3(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 0);
			if (subIfaceRects[i].Contains(mousePos))
				subIfaceHover = i;
		}

		int ifacePrev = subIfaceCurrent;
		if (CrossPlatformInputManager.GetButtonDown("Fire1")) {
			if (subIfaceHover >= 0)
				subIfaceCurrent = subIfaceHover;
		}

		if (subIfaceCurrent != ifacePrev) {
			subInterfaces[ifacePrev].Hide();
			subInterfaces[subIfaceCurrent].Show();
		}
	}

	void UpdateDimensions()
	{
		topBannerRect = new Rect(0, 0, Screen.width, Mathf.Min(Screen.height, Screen.width * 0.5f) * heightTopBanner);
		for (int i = 0; i < subIfaceRects.Length; i++) {
			float rectWidth = Screen.width * (1 - 2*paddingBannerSide) / subIfaceRects.Length - 2;
			subIfaceRects[i] = new Rect((Screen.width*paddingBannerSide) + i * rectWidth, 0, rectWidth, topBannerRect.height);
		}
		foreach (BaseUI subIface in subInterfaces)
			subIface.UpdateDimensions();
		bannerStyle.fontSize = (int)(0.75f * topBannerRect.height);
	}


	void OnGUI ()
	{
		if (!isActive)
			return;

		GUI.skin = GuiSkin;

		float alphaPrev = GUI.color.a;
		Color color = GUI.color;
		color.a = alphaTopBanner;
		GUI.color = color;
		GUI.DrawTexture(topBannerRect, topBannerTexture);
		color.a = alphaPrev;
		GUI.color = color;

		for (int i = 0; i < subIfaceRects.Length; i++) {
			if (i == subIfaceCurrent || i == subIfaceHover)
				bannerStyle.normal.textColor = colorSelected;
			else
				bannerStyle.normal.textColor = colorUnselected;
			GUI.Label(subIfaceRects[i], subInterfaces[i].displayName, bannerStyle);
		}
	}


	/**
	 * 	Activate / Deactivate UI mode
	 * 	Disable character controller input
	 * 	Hide crosshair
	 */

	public void Activate()
	{
		isActive = true;
		if (crosshair)
			crosshair.enabled = false;
		controller.ActivateGuiMode();
		timeScalePrev = Time.timeScale;
		if (pauseGame)
			Time.timeScale = 0;
		if(cameraFx)
			cameraFx.ActivateMenuEffects();

		subInterfaces[subIfaceCurrent].Show();
	}

	public void Deactivate()
	{
		isActive = false;
		if (crosshair)
			crosshair.enabled = true;
		controller.DeactivateGuiMode();
		Time.timeScale = timeScalePrev;
		if(cameraFx)
			cameraFx.DeactivateMenuEffects();
		
		subInterfaces[subIfaceCurrent].Hide();
	}

	public void Toggle()
	{
		if (isActive)
			Deactivate();
		else
			Activate();
	}


	public bool IsActive() { return isActive; }
}
