using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class UserAccount {

	public string username { get{return username;} private set{username = value;} }
	public string password { get{return password;} private set{password = value;} }

	public bool Authenticate(string username, string password)
	{
		if (this.username.Equals(username) && this.password.Equals(password))
			return true;
		return false;
	}

}
