using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class UserAccount {

	[SerializeField] private string username;
	[SerializeField] private string password;

	public bool Authenticate(string username, string password)
	{
		if (this.username.Equals(username) && this.password.Equals(password))
			return true;
		return false;
	}

}
