using UnityEngine;
using UnityEditor;
using System.Collections;

public class SoundListener : MonoBehaviour {

	public float listenStrengthFactor = 1;
	public LayerMask layerMask;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
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
		print("Heard a " + sound.soundType.ToString());
	}

}
