using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

/**
 * Singleton for removing and giving control to the fps controller
 */
public class ControlManager : SingletonComponent<ControlManager> {

	[SerializeField] private Crosshair crosshair;
	[SerializeField] private Camera cam;
	[SerializeField] private FirstPersonController controller;
	[SerializeField] private FpsInput fpsInput;
	private float timeScalePrev;


	/**
	 * 	Disable mouse lock
	 * 	Hide crosshair
	 */
	public void ActivateGuiMode(bool pauseGame)
	{
		if (crosshair)
			crosshair.enabled = false;
		controller.ActivateGuiMode();
		timeScalePrev = Time.timeScale;
		if (pauseGame)
			Time.timeScale = 0;
	}

	public void DeactivateGuiMode()
	{
		if (crosshair)
			crosshair.enabled = true;
		controller.DeactivateGuiMode();
		Time.timeScale = timeScalePrev;
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
