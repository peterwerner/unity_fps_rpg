using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

/**
 * Singleton for removing and giving control to the fps controller
 */
public class ControlManager : SingletonComponent<ControlManager> {

	[SerializeField] private Crosshair crosshair;
	[SerializeField] private Camera cam;
	[SerializeField] private FirstPersonController2 controller;
	[SerializeField] private FpsInput fpsInput;
	private float timeScalePrev;
	private bool controllerEnabledPrev;
	private bool didPause = false;


	void Start() {
		Screen.lockCursor = true;
	}


	public void ActivateGuiMode() {
		ActivateGuiMode(true);
	}
	public void ActivateGuiMode(bool pauseGame)
	{
		if (crosshair)
			crosshair.enabled = false;
		Screen.lockCursor = false;
		timeScalePrev = Time.timeScale;
		controllerEnabledPrev = controller.enabled;
		if (pauseGame) {
			Time.timeScale = 0;
			controller.enabled = false; 
			didPause = true;
		}
	}

	public void DeactivateGuiMode()
	{
		if (crosshair)
			crosshair.enabled = true;
		Screen.lockCursor = true;
		if (didPause) {
			Time.timeScale = timeScalePrev;
			controller.enabled = controllerEnabledPrev;
			didPause = false;
		}
	}


	/**
	 *	Enable fps player input (use key, reload, shoot, open inventory, etc)
	 */

	public void ActivateFpsInput()
	{
		fpsInput.enabled = true;
	}

	public void DeactivateFpsInput()
	{
		fpsInput.enabled = false;
	}

}
