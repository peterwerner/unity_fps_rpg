using UnityEngine;
using System.Collections;
using RootMotion.Demos;

public class testAI : MonoBehaviour {

	[SerializeField] private UserControlAI control;
	[SerializeField] private SoundListener listener;


	void Start () {
	
	}


	void FixedUpdate () 
	{
		if (listener.soundMemory.Count > 0) {
			SoundRecord maxSound = null;
			foreach (SoundRecord sound in listener.soundMemory) {
				if (maxSound == null || sound.soundType.CompareTo(maxSound.soundType) > 0)
					maxSound = sound; 
				if (sound.soundType.CompareTo(maxSound.soundType) == 0 && sound.effectiveSoundLevel > maxSound.effectiveSoundLevel)
					maxSound = sound; 
			}
			if (maxSound != null) {
				control.moveTargetPoint = maxSound.location;
				listener.soundMemory.Clear();
			}
		}
	}

}
