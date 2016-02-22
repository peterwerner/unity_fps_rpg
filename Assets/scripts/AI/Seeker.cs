using UnityEngine;
using System.Collections;

public class Seeker : MonoBehaviour {

	[SerializeField] private CharacterController controller;
	[SerializeField] private float moveSpeed = 1f;
	[SerializeField] private float gravityMultiplier = 2f;
	[SerializeField] private float tolerance = 1f;
	private bool seek = false;
	private Vector3 seekPoint;


	void Start () {
	
	}
	

	void FixedUpdate () 
	{
		if (Vector3.Distance(seekPoint, this.transform.position) <= tolerance)
			seek = false;

		Vector3 moveDir = Vector3.zero;
		if (seek) {
			Vector3 lookDir = new Vector3((seekPoint - transform.position).x, 0, (seekPoint - transform.position).z);
			moveDir += lookDir * moveSpeed * Time.fixedDeltaTime;
		}
		moveDir += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;

		controller.Move(moveDir);
	}


	public void Seek(Vector3 point)
	{
		seek = true;
		seekPoint = point;
	}

}
