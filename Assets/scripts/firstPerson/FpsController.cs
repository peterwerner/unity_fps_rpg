using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
//using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof (CharacterController))]
[RequireComponent(typeof (AudioSource))]
public class FpsController : MonoBehaviour 
{
	
	public enum MoveState { IDLE, WALKING, RUNNING, CROUCHING, SLIDING};

	public bool walkEnabled = true, runEnabled = true, crouchEnabled = true,
				slideEnabled = true, jumpEnabled = true;
	[SerializeField] private float walkSpeed = 4, runSpeed = 8, jumpSpeed = 5, crouchWalkSpeed = 2;
	[SerializeField] private float slideSpeed = 8, slideDuration = 0.5f;
	[SerializeField] private float gravityMultiplier = 5, stickToGroundForce = 5;

	[SerializeField] private Camera cam;
	//[SerializeField] private MouseLook mouseLook;
	private CharacterController characterController;

	private MoveState moveState = MoveState.IDLE;
	private bool isCrouching = false;
	private float timeSlideStart = 0;


	// Use this for initialization
	void Start () 
	{
		characterController = GetComponent<CharacterController>();
	}


	// Update is called once per frame
	void Update () 
	{
		Vector3 moveVec = Vector3.zero;

		Vector2 input2d = new Vector2(
				CrossPlatformInputManager.GetAxis("Horizontal"),
				CrossPlatformInputManager.GetAxis("Vertical")
			);
		UpdateMoveState(input2d);
		
		// Rotate move direction to move along camera forward
		Vector3 desiredMove = transform.forward * input2d.y + transform.right * input2d.x;
		// Get a normal for the surface that is being touched to move along it
		RaycastHit hitInfo;
		Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
							characterController.height/2f);
		desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
		// Determine movement speed based on movement state and crouch state
		float moveSpeed = walkSpeed;
		if (isCrouching)
			moveSpeed = crouchWalkSpeed;
		if (moveState == MoveState.SLIDING)
			moveSpeed = slideSpeed;
		else if (moveState == MoveState.RUNNING)
			moveSpeed = runSpeed;
		moveVec.x = desiredMove.x * moveSpeed;
		moveVec.z = desiredMove.z * moveSpeed;

		// If grounded, apply stick-to-ground force or jump
		if (characterController.isGrounded) {
			moveVec.y = -stickToGroundForce;
			if (jumpEnabled && CrossPlatformInputManager.GetButtonDown("Jump")) {
				moveVec.y = jumpSpeed;
			}
		}
		// If not grounded, apply gravity
		else {
			moveVec += Physics.gravity * gravityMultiplier * Time.deltaTime;
		}

		characterController.Move(moveVec * Time.deltaTime);
	}


	// Update movement state machine + crouch state
	void UpdateMoveState(Vector2 input2d)
	{
		switch (moveState)
		{
		case MoveState.IDLE:
			if (crouchEnabled && CrossPlatformInputManager.GetButtonDown("Crouch"))
				isCrouching = crouchEnabled && !isCrouching;
			if (!Mathf.Approximately(input2d.magnitude, 0) && characterController.isGrounded) {
				if (walkEnabled)
					moveState = MoveState.WALKING;
				if (runEnabled && CrossPlatformInputManager.GetButtonDown("Sprint")) {
					moveState = MoveState.RUNNING;
					isCrouching = false;
				}
			}
			break;

		case MoveState.WALKING:
			if (crouchEnabled && CrossPlatformInputManager.GetButtonDown("Crouch"))
				isCrouching = crouchEnabled && !isCrouching;
			if (Mathf.Approximately(input2d.magnitude, 0)) {
				if (runEnabled && CrossPlatformInputManager.GetButtonDown("Sprint")) {
					moveState = MoveState.RUNNING;
					isCrouching = false;
				}
			}
			if (walkEnabled || Mathf.Approximately(input2d.magnitude, 0) || !characterController.isGrounded)
				moveState = MoveState.IDLE;
			break;

		case MoveState.RUNNING:
			if (!runEnabled || CrossPlatformInputManager.GetButtonDown("Sprint"))
				moveState = MoveState.WALKING;
			if (walkEnabled || Mathf.Approximately(input2d.magnitude, 0) || !characterController.isGrounded)
				moveState = MoveState.IDLE;
			if (crouchEnabled && slideEnabled && CrossPlatformInputManager.GetButtonDown("Crouch")) {
				timeSlideStart = Time.time;
				moveState = MoveState.SLIDING;
			}
			break;

		case MoveState.SLIDING:
			isCrouching = true;
			if (!crouchEnabled || !slideEnabled || Time.time - timeSlideStart > slideDuration 
				|| !characterController.isGrounded) 
			{
				moveState = MoveState.WALKING;
			}
			break;
		}

		if (!crouchEnabled)
			isCrouching = false;
	}

}
