using UnityEngine;
using System.Collections;

/**
 * Tracks user mouse position and maps the cursor to the appropriate point in the virtual terminal's space
 */
public class MouseTracker : MonoBehaviour {

	[SerializeField] private Collider terminalCollider;
	[SerializeField] private float width, height;
	[SerializeField] private Camera playerCam;
	[SerializeField] private Camera terminalCam;
	[SerializeField] private float maxDist = 10f;
	private Vector2 cursorPos;

	
	// Update is called once per frame
	void Update () 
	{
		Vector3 outward = -transform.forward, up = transform.up;
		Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (terminalCollider.Raycast(ray, out hit, maxDist)) {
			Vector3 diff = hit.point - transform.position;
			Vector3 rotated = Quaternion.LookRotation(outward, up) * diff;
			float camHalfHeight = terminalCam.orthographicSize;
			float camHalfWidth = camHalfHeight * terminalCam.aspect;
			cursorPos = new Vector2(
				rotated.x * 2 * camHalfWidth / width,
				rotated.y * 2 * camHalfHeight / height
			);
		}
	}
		
	public Vector2 GetCursorPos() { return cursorPos; }

}
