using UnityEngine;
using System.Collections;

public class DoorRotating : Useable {

	public Transform parentAxis;
	public float speed = 10f;
	public float degreesOfRotation = 90f;

	private Quaternion rotationInitial;
	private Quaternion target;
	private Quaternion targetOpen;
	private Quaternion targetClosed;

	// Use this for initialization
	void Start () 
	{
		rotationInitial = parentAxis.transform.rotation;
		targetClosed = rotationInitial;
		targetOpen = rotationInitial * Quaternion.Euler(parentAxis.transform.up * degreesOfRotation);
		target = targetClosed;
	}


	public override void Use() 
	{ 
		if (target == targetClosed) {
			target = targetOpen;
			speed = Mathf.Abs(speed);
		}
		else {
			target = targetClosed;
			speed = -1 * Mathf.Abs(speed);
		}
	}


	// FixedUpdate is called once per physics update
	void FixedUpdate () 
	{
		if (transform.rotation.Equals(target))
			return;

		Quaternion delta = Quaternion.Euler(parentAxis.up * speed * Time.fixedDeltaTime); 

		// If sufficiently close to target, snap into exact rotation
		float angle = Mathf.Abs(Quaternion.Angle(parentAxis.transform.rotation, target));
		if (angle > 180f)
			angle = 360f - angle;
		if (angle < Mathf.Abs(speed) * Time.fixedDeltaTime)
			parentAxis.transform.rotation = target;
		// Otherwise, interpolate to target
		else
			parentAxis.transform.rotation *= delta;
	}
		
}
