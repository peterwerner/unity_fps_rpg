using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Email {

	public string senderName { get{return senderName;} private set{senderName = value;} }
	[TextArea(3,10)] public string message;


	public string GetMessage() {
		return message;
	}

}
