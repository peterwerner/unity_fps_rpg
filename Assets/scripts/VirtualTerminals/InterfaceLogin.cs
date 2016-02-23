using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

[Serializable]
public class InterfaceLogin {

	private enum State { USERNAME, PASSWORD };

	[SerializeField] private UserAccount account;
	[SerializeField] private string emptyUsername = "username: ", emptyPassword = "password: ";
	[SerializeField] private string cursorCharacter = "_";
	[SerializeField] private float cursorFlashDuration = 0.5f;
	[SerializeField] private int maxLen = 25;
	[SerializeField] private string legalChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-=\\[];',./`~!@#$%^&*()_+|{}:\"<>?";
	private Text usernameField, passwordField; 
	private float cursorTimeLastFlashed = 0f;
	private bool cursorOn = false;
	private string inputUsername = "", inputPassword = "";
	private State state = State.USERNAME;
	private float timeBackspaceDown = 0f, timeBackspaceDelete = 0f;
	private float delayBackspaceStart = 0.5f, delayBackspaceDelete = 0.025f;
	private bool backspacing = false;


	public void Init (Text usernameField, Text passwordField)
	{
		this.usernameField = usernameField;
		this.passwordField = passwordField;
	}


	public void Update () 
	{
		// Take user input
		if (!Input.GetKey(KeyCode.Backspace) && !Input.GetKey(KeyCode.Delete))
			backspacing = false;
		if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete)) {
			timeBackspaceDown = Time.time;
			Backspace();
		}
		else if (Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Delete)) {
			if (Time.time - timeBackspaceDown > delayBackspaceStart)
				backspacing = true;
			if (backspacing && Time.time - timeBackspaceDelete > delayBackspaceDelete) {
				timeBackspaceDelete = Time.time;
				Backspace();
			}	
		}
		else if (Input.GetKeyDown(KeyCode.Tab)) {
			SwitchField();
		}
		else if (Input.inputString != null && Input.inputString.Length == 1 && legalChars.Contains(Input.inputString)) {
			if (state == State.USERNAME && inputUsername.Length < maxLen)
				inputUsername += Input.inputString;
			else if (state == State.PASSWORD && inputPassword.Length < maxLen)
				inputPassword += Input.inputString;
		}
		// Update canvas text
		usernameField.text = emptyUsername + inputUsername;
		passwordField.text = emptyPassword + inputPassword;
		// Update flashing cursor
		if (Time.time - cursorTimeLastFlashed	 > cursorFlashDuration) {
			cursorOn = !cursorOn;
			cursorTimeLastFlashed = Time.time;
		}
		// Append flashing cursor
		if (cursorOn) {
			if (state == State.USERNAME)
				usernameField.text = usernameField.text + cursorCharacter;
			else if (state == State.PASSWORD)
				passwordField.text = passwordField.text + cursorCharacter;
		}
	}


	public bool Authenticate()
	{
		return account.Authenticate(inputUsername, inputPassword);
	}


	void Backspace()
	{
		if (state == State.USERNAME && inputUsername.Length > 0)
			inputUsername = inputUsername.Substring(0, inputUsername.Length - 1);
		else if (state == State.PASSWORD && inputPassword.Length > 0)
			inputPassword = inputPassword.Substring(0, inputPassword.Length - 1);
	}


	void SwitchField()
	{
		if (state == State.USERNAME)
			state = State.PASSWORD;
		else
			state = State.USERNAME;
	}

}
