using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;

public class CharacterStats : Damageable {

	public enum State { ALIVE, DEAD, UNCONSCIOUS };

	public float healthMax = 100f;
	public float health = 100f;
	public PuppetMaster puppetMaster;
	[Tooltip("Settings for killing and freezing the puppet.")]
	public PuppetMaster.StateSettings stateSettings = PuppetMaster.StateSettings.Default;
	public float puppetUnpin = 5, puppetForceMultiplier = 100;
	[Tooltip("When killed / KO'd these behaviours will be disabled")]
	[SerializeField] private MonoBehaviour[] lifeDependentBehaviours;

	protected CharacterStats.State state = State.ALIVE;


	// Use this for initialization
	void Start () 
	{
	}


	// Update is called once per frame
	void Update ()
	{
		if (health <= 0)
			Kill();
	}


	public void Damage(float damage)
	{
		health -= damage;
	}
	public override void Damage(float damage, float force, RaycastHit hit, Vector3 direction)
	{
		float damageMultiplier = 1, unpinMultiplier = 1;
		// Take into account hitboxes
		HitBoxStats hitbox = hit.collider.GetComponent<HitBoxStats>();
		if (hitbox) {
			damageMultiplier = hitbox.damageMultiplier;
			unpinMultiplier = hitbox.unpinMultiplier;
		}
		// Affect puppet master ragdolls
		var broadcaster = hit.collider.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();
		if (broadcaster != null)
			broadcaster.Hit(puppetUnpin * unpinMultiplier, direction * force * puppetForceMultiplier, hit.point);
		// Do damage
		Damage(damage * damageMultiplier);
	}


	public virtual void Knockout()
	{
		state = State.UNCONSCIOUS;
		if (puppetMaster)
			puppetMaster.Kill(stateSettings);
		disableBehaviours();
	}


	public virtual void Kill() 
	{
		state = State.DEAD;
		if (puppetMaster)
			puppetMaster.Kill(stateSettings);
		disableBehaviours();
	}


	public void Resurrect()
	{
		Resurrect(health);
	}
	public virtual void Resurrect(float newHealth)
	{
		if (newHealth <= 0)
			return;
		state = State.ALIVE;
		health = Mathf.Max(newHealth, healthMax);
		if (puppetMaster)
			puppetMaster.Resurrect();
		enableBehaviours();
	}


	private void disableBehaviours()
	{
		foreach (MonoBehaviour script in lifeDependentBehaviours)
			script.enabled = false;
	}
	private void enableBehaviours()
	{
		foreach (MonoBehaviour script in lifeDependentBehaviours)
			script.enabled = true;
	}
	
}
