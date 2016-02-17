using UnityEngine;
using System.Collections;

public class PickUpPhysics : MonoBehaviour {

	public float maxMass = 3; 
	new public Camera camera;
	public float massReduced = 0.1f;
	public float distFromCam = 2;

	private Rigidbody obj = null;
	private float objMass, objDrag;
	private float distTarget;


	// Update is called once per frame
	void Update () 
	{
		if (obj) {
			Vector3 targetPos = camera.transform.position + distTarget * camera.transform.forward;
			obj.AddForce((targetPos - obj.transform.position) * Mathf.Pow((targetPos - obj.transform.position).magnitude, 2));
			obj.drag = 0.01f / Mathf.Pow((targetPos - obj.transform.position).magnitude, 6);
		}
	}


	public bool Grab(Rigidbody rb, RaycastHit hit) 
	{
		if (obj)
			return false;
		if (rb && !rb.isKinematic && rb.mass <= maxMass) {
			obj = rb;
			objMass = obj.mass;
			objDrag = obj.drag;
			obj.mass = massReduced;
			obj.useGravity = false;
			distTarget = (obj.transform.position - camera.transform.position).magnitude - hit.distance + distFromCam;
			return true;
		}
		return false;
	}


	public bool Drop() 
	{
		if (obj == null)
			return false;
		obj.mass = objMass;
		obj.drag = objDrag;
		obj.useGravity = true;
		obj = null;
		return true;
	}


	public bool IsHolding() { return (obj != null); }

}
