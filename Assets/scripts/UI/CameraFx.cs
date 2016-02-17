using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(Camera))]
public class CameraFx : MonoBehaviour {

	private Camera cam;
	private PostEffectsBase[] effects;
	private float farClipPlane;

	// Use this for initialization
	void Start () 
	{
		cam = GetComponent<Camera>();
		effects = GetComponents<PostEffectsBase>();
		foreach (PostEffectsBase effect in effects)
			effect.enabled = false;
		farClipPlane = cam.farClipPlane;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}


	public void ActivateMenuEffects()
	{
		foreach (PostEffectsBase effect in effects)
			effect.enabled = true;
		farClipPlane = cam.farClipPlane;
		cam.farClipPlane = 100f;
	}
	public void DeactivateMenuEffects()
	{
		foreach (PostEffectsBase effect in effects)
			effect.enabled = false;
		cam.farClipPlane = farClipPlane;
	}
}
