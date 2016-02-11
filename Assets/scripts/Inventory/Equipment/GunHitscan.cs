using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;


[RequireComponent (typeof(AudioSource))]
public class GunHitscan : Equipment {


	public float impactDamage = 10f;
	public float impactForce = 10f;
	public float shotDelay = 0.05f; 	// Min delay between shots (controls rate of fire)
	public float maxRange = 500f;		// Max range a bullet can travel
	public bool automatic = false;		// Automatic (click and hold) vs Semi-automatic (click per shot)
	public AudioClip audioShoot;
	public LayerMask layers;
	public ParticleSystem particleImpact;

	protected Transform aimOriginTransform;
	private float timeSinceLastShot = 0f;
	private AudioSource audioSrc;
	private Camera aimCamera;			// Optional camera to use for aiming directly at the center of the screen


	// Use this for initialization
	new void Start () {
		base.BaseStart();

		aimOriginTransform = transform;
		audioSrc = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	new void Update () {
		base.BaseUpdate();

		if (timeSinceLastShot < shotDelay)
			timeSinceLastShot += Time.deltaTime;
	}


	/**
	 * 	Act on inputs
	 */
	public override void TakeInput(Inventory.InputType input)
	{
		if (input == Inventory.InputType.WEAPON_FIRE1_DOWN)
			Shoot();
		else if (input == Inventory.InputType.WEAPON_FIRE1 && automatic)
			Shoot();
	}


	/**
	 * Deactivate this - unparent the audio source BEFORE destroying viewmodel / worldmodel
	 */
	public override void Deactivate() 
	{
		audioSrc.transform.parent = null;

		base.Deactivate();
	}


	/**
	 * 	Shoot the gun
	 */ 
	private bool Shoot()
	{
		if (timeSinceLastShot >= shotDelay) 
		{
			// Fire the bullet ray
			RaycastHit hit = new RaycastHit();	
			Ray ray = new Ray(aimOriginTransform.position, aimTarget.target - aimOriginTransform.position);
			if (Physics.Raycast(ray, out hit, maxRange, layers)) 
			{
				// Spawn impact particle
				if (particleImpact) {
					particleImpact.transform.position = hit.point;
					particleImpact.transform.rotation = Quaternion.LookRotation(hit.normal);
					particleImpact.Emit(5);
				}

				// Damage object
				Damageable damageable = hit.collider.GetComponentInParent<Damageable>();
				if (damageable)
					damageable.Damage(impactDamage, impactForce, hit, ray.direction);
			}

			// Debug - draw the bullet ray
			if (hit.distance <= 0.01f)	hit.distance = 100f;
			Debug.DrawRay(ray.origin, ray.direction.normalized * hit.distance, Color.white, 0.5f);	

			PlayShootSound();

			timeSinceLastShot = 0;
			return true;
		}
		return false;
	}

	// Play the gunshot sound
	private void PlayShootSound()
	{
		if (audioSrc != null && audioShoot != null) {
			audioSrc.PlayOneShot(audioShoot);
		}
	}


	/**
	 *  Set the aim origin
	 * 	Parent the sound to the viewmodel / worlmodel
	 */
	public override bool ActivateViewModel(Camera viewModelCamera) {
		if (!base.ActivateViewModel(viewModelCamera))
			return false;

		audioSrc.transform.parent = viewModel.points[0];
		audioSrc.transform.localPosition = Vector3.zero;

		//aimOriginTransform = viewModel.points[0];

		// Alternatively, simply use the camera as the origin
		aimOriginTransform = viewModelCamera.transform;

		return true;
	}
	public override bool ActivateWorldModel(Transform worldModelParent) {
		if (!base.ActivateWorldModel(worldModelParent))
			return false;

		audioSrc.transform.parent = worldModel.points[0];
		audioSrc.transform.localPosition = Vector3.zero;

		aimOriginTransform = worldModel.points[0];

		return true;
	}

}
