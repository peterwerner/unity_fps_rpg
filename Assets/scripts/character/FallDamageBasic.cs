using UnityEngine;
using System.Collections;

public class FallDamageBasic : MonoBehaviour {

	public CharacterController controller;
	public CharacterStats stats;
	public float deltaSpeedThreshold;
	public float deltaSpeedInstakill;
	[Tooltip("If true, any rapid change in velocity will cause damage. " + 
		"If false, only positive y acceleration (ie: falling to the ground) will cause damage.")]
	public bool multiDirectional = true;

	private Vector3 velocityPrev = Vector3.zero;

	
	// Update is called once per frame
	void Update () 
	{
		Vector3 deltaVelocity = controller.velocity - velocityPrev;
		float deltaSpeed;
		if (multiDirectional)
			deltaSpeed = deltaVelocity.magnitude;
		else
			deltaSpeed = Mathf.Max(0, deltaVelocity.y);

		float damage;
		if (deltaSpeed >= deltaSpeedInstakill)
			damage = stats.healthMax;
		else if (deltaSpeed <= deltaSpeedThreshold)
			damage = 0;
		else
			damage = stats.healthMax * (deltaSpeed - deltaSpeedThreshold) / (deltaSpeedInstakill - deltaSpeedThreshold);

		if (damage > 0)
			stats.Damage(damage);

		velocityPrev = controller.velocity;
	}

}
