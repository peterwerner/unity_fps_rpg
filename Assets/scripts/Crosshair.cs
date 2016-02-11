using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {

	public AimTarget target;
	public Camera cam;
	public Texture2D crosshairTexture;
	public float crosshairScale = 1;

	void OnGUI()
	{
		//if not paused
		if(Time.timeScale != 0)
		{
			if(crosshairTexture!=null) 
			{
				Vector3 screenPos = cam.WorldToScreenPoint(target.target);

				GUI.DrawTexture(
					new Rect(
						screenPos.x-(crosshairTexture.width*crosshairScale/2),
						screenPos.y-(crosshairTexture.height*crosshairScale/2),
						crosshairTexture.width*crosshairScale,
						crosshairTexture.height*crosshairScale
					),	
					crosshairTexture
				);
			}
			else
				Debug.Log("No crosshair texture set in the Inspector");
		}
	}
		
}
