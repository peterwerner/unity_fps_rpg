using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class Terminal : Useable {

	private enum State { OFF, LOGIN, EMAIL };

	[SerializeField] private InterfaceLogin login;
	[SerializeField] private Text usernameField, passwordField; 
	private State state = State.OFF;


	void Start () 
	{
		login.Init(usernameField, passwordField);
	}


	void Update () 
	{
		if (state == State.LOGIN) {
			login.Update();
			if (CrossPlatformInputManager.GetButtonDown("Submit") && login.Authenticate())
				Deactivate();
		}
	}


	public override void Use() 
	{
		Activate();
	}


	private void Activate()
	{
		ControlManager.Instance.ActivateGuiMode(false);
		ControlManager.Instance.DeactivateFpsInput();
		state = State.LOGIN;
	}

	private void Deactivate()
	{
		ControlManager.Instance.DeactivateGuiMode();
		ControlManager.Instance.ActivateFpsInput();
		state = State.OFF;
	}

}
