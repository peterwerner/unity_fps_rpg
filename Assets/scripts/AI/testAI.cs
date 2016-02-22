using UnityEngine;
using System.Collections;

public class testAI : MonoBehaviour {

	[SerializeField] private Seeker seeker;
	[SerializeField] private SoundListener listener;


	void Start () {
	
	}

	
	void FixedUpdate () 
	{
		if (listener.soundLocations.Count > 0) {
			seeker.Seek(listener.soundLocations[0]);
			listener.soundLocations.Clear();
		}
	}

}
