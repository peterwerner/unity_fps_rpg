using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IfaceLogin : MonoBehaviour {

	private enum State { USERNAME, PASSWORD };

	[SerializeField] private UserAccount account;
	[SerializeField] private Text usernameField, passwordField; 
	[SerializeField] private string emptyUsername = "username: ", emptyPassword = "password: ";
	[SerializeField] private string cursorCharacter = "_";
	[SerializeField] private float cursorFlashDuration = 0.5f;
	[SerializeField] private int maxLen = 25;
	private float cursorTimeLastFlashed = 0f;
	private bool cursorOn = false;
	private string inputUsername = "", inputPassword = "";
	private State state = State.USERNAME;
	private float timeBackspaceDown = 0f, timeBackspaceDelete = 0f;
	private float delayBackspaceStart = 0.5f, delayBackspaceDelete = 0.025f;
	private bool backspacing = false;


	// Update is called once per frame
	void Update () 
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
		else if (Input.inputString != null && Input.inputString.Length == 1) {
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


	void Backspace()
	{
		if (state == State.USERNAME && inputUsername.Length > 0)
			inputUsername = inputUsername.Substring(0, inputUsername.Length - 1);
		else if (state == State.PASSWORD && inputPassword.Length > 0)
			inputPassword = inputPassword.Substring(0, inputPassword.Length - 1);
	}

}
