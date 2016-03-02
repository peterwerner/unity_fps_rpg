using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

/**
 * Controls the inventory UI
 */
public class PauseUI : MonoBehaviour {

	[SerializeField] private GUISkin GuiSkin;
	[SerializeField] BaseUI[] subInterfaces;
	[SerializeField] [Range(0f, 1f)] private float heightTopBanner = 0.1f, paddingBannerSide = 0.1f, alphaTopBanner = 0.5f;
	[SerializeField] CameraFx cameraFx;
	private bool isActive = false;
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
		

	public void Toggle()
	{
		if (!isActive) {
			ControlManager.Instance.ActivateGuiMode(false);
			subInterfaces[subIfaceCurrent].Show();
			if(cameraFx)
				cameraFx.ActivateMenuEffects();
		}
		else {
			ControlManager.Instance.DeactivateGuiMode();
			subInterfaces[subIfaceCurrent].Hide();
			if(cameraFx)
				cameraFx.DeactivateMenuEffects();
		}
		isActive = !isActive;
	}


	public bool IsActive() { return isActive; }
}
