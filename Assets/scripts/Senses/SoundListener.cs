using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SoundListener : MonoBehaviour {

	[SerializeField] private float listenStrengthFactor = 1;
	[SerializeField] private LayerMask layerMask;
	[HideInInspector] public List<Vector3> soundLocations;


	// Use this for initialization
	void Start () {
		soundLocations = new List<Vector3>();
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
				Hear(sound);
		}
	}


	private void Hear(Sound sound) 
	{
		soundLocations.Add(sound.transform.position);
		//print("Heard a " + sound.soundType.ToString());
	}

}
