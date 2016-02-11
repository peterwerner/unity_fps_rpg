using UnityEngine;
using System.Collections;

/**
 * 	Uses the camera to determine what point in space is being targetted
 */
[RequireComponent (typeof(Camera))]
public class AimTargetCam : AimTarget {

	private Camera aimCamera;

	// Use this for initialization
	void Start () {
		aimCamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		// Set the aim target to whatever the camera is pointing at
		RaycastHit hit;
		Ray ray = new Ray(aimCamera.transform.position, aimCamera.transform.forward);
		if (Physics.Raycast(ray, out hit))
			target = hit.point;
		// If the ray didn't hit anything, just fake it by setting the target very far from the camera
		else
			target = ray.origin + 1000f * ray.direction;
	}
}
