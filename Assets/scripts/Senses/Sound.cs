using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sound : ListComponent<Sound> {

	public enum Type { DEFAULT, GUNSHOT, EXPLOSION, DOOR, FOOTSTEP, SPEECH };

	[Tooltip("If something is in the way of a sound and a listener, by what factor should the soundLevel be multiplied?")]
	public static float wallDampenFactor = 0.5f;

	[Tooltip("Effectively equal to the max distance away that a standard listener can hear this")]
	public float soundLevel;
	public Type soundType;

	private uint lifetimeFrames = 0;	// 0 = do not use frames to determine lifecycle
	private float lifetimeSec = 0;		// 0 = do not use time to determine lifecycle
	private uint framesAlive = 0;
	private float timeAlive = 0;


	// Update is called once per frame
	void Update () 
	{
		if ((lifetimeFrames > 0 && framesAlive >= lifetimeFrames) || lifetimeSec > 0 && timeAlive >= lifetimeSec) {
			this.transform.parent = null;
			Destroy(this.gameObject);
		}
		framesAlive++;
		timeAlive += Time.deltaTime;
	}


	// Various options for instantiating a sound
	// Combinations of: transform parent vs vector position, frames lifetime vs seconds lifetime
	public static Sound MakeSound(Transform parent, Sound.Type soundType, float soundLevel, uint lifetimeFrames) 
	{
		Sound newSound = MakeSound(parent, soundType, soundLevel);
		newSound.lifetimeFrames = lifetimeFrames;
		return newSound;
	}
	public static Sound MakeSound(Transform parent, Sound.Type soundType, float soundLevel, float lifetime) 
	{
		Sound newSound = MakeSound(parent, soundType, soundLevel);
		newSound.lifetimeSec = lifetime;
		return newSound;
	}
	public static Sound MakeSound(Vector3 position, Sound.Type soundType, float soundLevel, uint lifetimeFrames) 
	{
		Sound newSound = MakeSound(position, soundType, soundLevel);
		newSound.lifetimeFrames = lifetimeFrames;
		return newSound;
	}
	public static Sound MakeSound(Vector3 position, Sound.Type soundType, float soundLevel, float lifetime) 
	{
		Sound newSound = MakeSound(position, soundType, soundLevel);
		newSound.lifetimeSec = lifetime;
		return newSound;
	}
	public static Sound MakeSound(Transform parent, Sound.Type soundType, float soundLevel) 
	{
		GameObject obj = new GameObject();
		Sound newSound = obj.AddComponent<Sound>();
		newSound.transform.position = parent.position;
		newSound.transform.parent = parent;
		newSound.soundType = soundType;
		newSound.soundLevel = soundLevel;
		return newSound;
	}
	public static Sound MakeSound(Vector3 position, Sound.Type soundType, float soundLevel) 
	{
		GameObject obj = new GameObject();
		Sound newSound = obj.AddComponent<Sound>();
		newSound.transform.position = position;
		newSound.soundType = soundType;
		newSound.soundLevel = soundLevel;
		return newSound;
	}


	private float GetSoundLevel() { return soundLevel; }
}
