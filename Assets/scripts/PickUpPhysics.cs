using UnityEngine;
using System.Collections;

public class PickUpPhysics : MonoBehaviour {

	public float maxMass = 3; 
	public float maxDistFromTarget = 1.5f;
	new public Camera camera;
	public float massReduced = 0.1f;
	public float distFromCam = 2;

	private string playerTag = "Player";
	private Rigidbody obj = null;
	private float objMass, objDrag, objDragAngular;
	private float distTarget;
	private bool isTouchingPlayer;


	void FixedUpdate () 
	{
		Vector3 targetPos = camera.transform.position + distTarget * camera.transform.forward;
		if (obj && Vector3.Distance(obj.transform.position, targetPos) > maxDistFromTarget)
			Drop();
		if (obj) {
			MoveTowards(targetPos);
			TorqueTowards(camera.transform.forward);
		}
	}


	public bool Grab(Rigidbody rb, RaycastHit hit) 
	{
		if (IsHolding() || isTouchingPlayer)
			return false;
		if (rb && !rb.isKinematic && rb.mass <= maxMass) {
			obj = rb;
			objMass = obj.mass;
			objDrag = obj.drag;
			objDragAngular = obj.angularDrag;
			obj.mass = massReduced;
			obj.useGravity = false;
			distTarget = (obj.transform.position - camera.transform.position).magnitude - hit.distance + distFromCam;
			return true;
		}
		return false;
	}


	public bool Drop() 
	{
		if (!IsHolding())
			return false;
		obj.mass = objMass;
		obj.drag = objDrag;
		obj.angularDrag = objDragAngular;
		obj.useGravity = true;
		obj = null;
		return true;
	}


	// Apply torque to reach a target rotation (adapted from: answers.unity3d.com/questions/48836)
	// Apply angular drag to slow down near the target rotation
	private void TorqueTowards(Vector3 targetDir)
	{
		if (obj == null)
			return;
		Vector3 x = Vector3.Cross(obj.transform.forward.normalized, targetDir.normalized);
		float theta = Mathf.Asin(x.magnitude);
		Vector3 w = x.normalized * theta / Time.fixedDeltaTime;
		Quaternion q = transform.rotation * obj.inertiaTensorRotation;
		Vector3 T = q * Vector3.Scale(obj.inertiaTensor, (Quaternion.Inverse(q) * w));
		if (float.IsNaN(T.x) || float.IsNaN(T.y) || float.IsNaN(T.z))
			return;
		obj.AddTorque(T * 4);
		obj.angularDrag = 0.01f / Mathf.Pow(theta, 4);
	}


	// Apply force to reach a target position
	// Apply drag to slow down near the target position
	private void MoveTowards(Vector3 targetPos)
	{
		if (obj == null)
			return;
		obj.AddForce(100 * (targetPos - obj.transform.position) * (targetPos - obj.transform.position).magnitude);
		obj.AddForce(-2 * obj.velocity);
		obj.drag = 0.01f / Mathf.Pow((targetPos - obj.transform.position).magnitude, 4);
	}


	public bool IsHolding() { return (obj != null); }

}
