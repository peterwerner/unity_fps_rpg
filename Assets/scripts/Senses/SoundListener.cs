using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class SoundRecord {
	public Sound.Type soundType;
	public float effectiveSoundLevel;
	public Vector3 location;
	public float timeHeard;
	public SoundRecord(Sound.Type soundType, float effectiveSoundLevel, Vector3 location) {
		this.soundType = soundType;
		this.effectiveSoundLevel = effectiveSoundLevel;
		this.location = location;
		this.timeHeard = Time.time;
	}
}


public class SoundListener : MonoBehaviour {

	[SerializeField] private float listenStrengthFactor = 1;
	[SerializeField] private LayerMask layerMask;
	[HideInInspector] public List<SoundRecord> soundMemory;


	// Use this for initialization
	void Start () {
		soundMemory = new List<SoundRecord>();
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		if (listenStrengthFactor <= 0)
			return;
		foreach (Sound sound in Sound.InstanceList) {
			// Calculate the effective sound level, taking into account obstruction and listener strength
			float dist = Vector3.Distance(this.transform.position, sound.transform.position);
			float effectiveSoundLevel = sound.soundLevel * listenStrengthFactor;
			if (dist < effectiveSoundLevel) {
				// If possibly in range, fire a ray and dampen if there is an obstruction
				RaycastHit hit;
				Ray ray = new Ray(this.transform.position, sound.transform.position - this.transform.position);
				if (Physics.Raycast(ray, out hit, dist, layerMask))
				if (hit.distance - effectiveSoundLevel < 0.1f && !ArrayUtility.Contains(hit.collider.gameObject.GetComponentsInChildren<Sound>(), sound))
					effectiveSoundLevel *= Sound.wallDampenFactor;
			}
			if (dist < effectiveSoundLevel)
				soundMemory.Add(new SoundRecord(sound.soundType, effectiveSoundLevel, sound.transform.position));
		}
	}

}
