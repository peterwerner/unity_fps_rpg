using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class EmailInbox {

	public string ownerName { get{return ownerName;} private set{ownerName = value;} }
	public string ownerAddress { get{return ownerAddress;} private set{ownerAddress = value;} }
	[SerializeField] private List<Email> emails;
	private int currentEmailIndex;

	public int Count() { return emails.Count; }

	public int GetCurrentEmailIndex() { return currentEmailIndex; }

	public Email GetCurrent() { return Get(currentEmailIndex); }

	public Email Get(int index)
	{
		if (index < 0 || index >= Count())
			return null;
		return emails[index];
	}

	public void Remove(int index)
	{
		if (index >= 0 && index < Count())
			emails.RemoveAt(index);
	}
	public void Remove(Email email)
	{
		emails.Remove(email);
	}
		
}
