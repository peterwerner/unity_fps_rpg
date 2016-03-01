using UnityEngine;
using System.Collections;
//using RootMotion.Dynamics;
using System.Collections.Generic;


[RequireComponent (typeof(AudioSource))]
public class GunHitscan : Equipment {

	public float impactDamage = 10f;
	public float impactForce = 10f;
	public float shotDelay = 0.05f; 	// Min delay between shots (controls rate of fire)
	public float maxRange = 500f;		// Max range a bullet can travel
	public bool automatic = false;		// Automatic (click and hold) vs Semi-automatic (click per shot)
	public int clipSize = 10;
	public Ammo.Type ammoType;
	public AudioClip audioShoot;
	public float soundLevel = 30f;
	public LayerMask layers;
	public EffectTemporary muzzleFlashEffect;

	protected Transform aimOriginTransform;
	private int numBulletsLoaded;
	private float timeSinceLastShot = 0f;
	private AudioSource audioSrc;
	private Camera aimCamera;			// Optional camera to use for aiming directly at the center of the screen


	// Use this for initialization
	new void Start () {
		base.BaseStart();

		aimOriginTransform = transform;
		audioSrc = GetComponent<AudioSource>();
		numBulletsLoaded = clipSize;
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
		else if (input == Inventory.InputType.WEAPON_RELOAD)
			Reload();
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
		if (timeSinceLastShot < shotDelay) 
			return false;

		if (numBulletsLoaded <= 0)
			return false;

		// Fire the bullet ray
		RaycastHit hit = new RaycastHit();	
		Ray ray = new Ray(aimOriginTransform.position, aimTarget.target - aimOriginTransform.position);
		if (Physics.Raycast(ray, out hit, maxRange, layers)) 
		{
			// Spawn muzzle flash particle
			if (muzzleFlashEffect) {
				Transform barrel = null;
				if (viewModel)
					barrel = viewModel.points[0];
				else if (worldModel)
					barrel = viewModel.points[0];
				if (barrel) {
					EffectTemporary flash = (EffectTemporary)Instantiate(muzzleFlashEffect, barrel.position, barrel.rotation);
					flash.transform.parent = aimOriginTransform;
					flash.transform.forward = aimOriginTransform.forward;
				}
			}

			// Spawn impact particle
			MaterialFxManager.Instance.DoBulletImpactFx(hit);

			// Damage object
			Damageable damageable = hit.collider.GetComponentInChildren<Damageable>();
			if (!damageable)
				damageable = hit.collider.GetComponentInParent<Damageable>();
			if (damageable)
				damageable.Damage(impactDamage, impactForce, hit, ray.direction);
		}

		// Debug - draw the bullet ray
		if (hit.distance <= 0.01f)	hit.distance = 100f;
		Debug.DrawRay(ray.origin, ray.direction.normalized * hit.distance, Color.white, 0.5f);	

		PlayShootSound();

		timeSinceLastShot = 0;

		numBulletsLoaded--;

		return true;
	}

	// Play the gunshot sound
	private void PlayShootSound()
	{
		if (audioSrc != null && audioShoot != null) {
			audioSrc.PlayOneShot(audioShoot);
			Sound.MakeSound(this.transform, Sound.Type.GUNSHOT, soundLevel, 1);
		}
	}


	/**
	 * Attempt to reload the weapon
	 */
	private bool Reload()
	{
		if (!ConsumeClip())
			return false;

		numBulletsLoaded = clipSize;

		return true;
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


	/**
	 * Look in the inventory and return the number of clips available
	 */
	public int GetNumClips()
	{
		if (inventory == null)
			return 0;
		int count = 0;
		List<Equipment> invItems = inventory.GetItems();
		foreach (Equipment item in invItems) {
			Ammo ammo = item.GetComponent<Ammo>();
			if (ammo && ammo.type == this.ammoType)
				count++;
		}
		return count;
	}
	/**
	 * Remove 1 ammo clip from the inventory
	 */
	private bool ConsumeClip()
	{
		if (inventory == null)
			return false;
		List<Equipment> invItems = inventory.GetItems();
		foreach (Equipment item in invItems) {
			Ammo ammo = item.GetComponent<Ammo>();
			if (ammo && ammo.type == this.ammoType) {
				inventory.DeleteItem(item);
				return true;
			}
		}
		return false;
	}



	public int GetNumBulletsLoaded() { return numBulletsLoaded; }

}
