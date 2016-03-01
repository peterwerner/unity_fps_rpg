using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class Terminal : Useable {

	private enum State { OFF, LOGIN, EMAIL };

	[SerializeField] private Camera virtualTerminalCam;
	[SerializeField] private MouseTracker mouseTracker;
	[SerializeField] private InterfaceLogin login;
	[SerializeField] private Text usernameText, passwordText; 
	private State state = State.OFF;


	void Start () 
	{
		login.Init(usernameText, passwordText);
		mouseTracker.enabled = false;
		DeactivateVirtualCam();
	}


	void Update () 
	{
		if (state == State.LOGIN) {
			login.Update();
			if (CrossPlatformInputManager.GetButtonDown("Submit") && login.Authenticate())
				SetState(State.OFF);;
			if (CrossPlatformInputManager.GetButtonDown("Cancel"))
				SetState(State.OFF);
		}
	}


	public override void Use() 
	{
		if (state == State.OFF)
			SetState(State.LOGIN);
	}
		


	private void SetState(State newState) {
		if (state == newState)
			return;
		if (newState == State.OFF) {
			ControlManager.Instance.DeactivateGuiMode();
			ControlManager.Instance.ActivateFpsInput();
			mouseTracker.enabled = false;
			DeactivateVirtualCam();
		}
		else {
			ControlManager.Instance.ActivateGuiMode(false);
			ControlManager.Instance.DeactivateFpsInput();
			mouseTracker.enabled = true;
			ActivateVirtualCam();
		}
		state = newState;
	}


	private void ActivateVirtualCam() {
		virtualTerminalCam.enabled = true;
	}

	private void DeactivateVirtualCam() {
		virtualTerminalCam.enabled = false;
	}


}
