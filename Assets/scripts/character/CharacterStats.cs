using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using RootMotion.Dynamics;

public class CharacterStats : Damageable {

	public enum State { ALIVE, DEAD, UNCONSCIOUS };

	public float healthMax = 100f;
	public float health = 100f;
	//public PuppetMaster puppetMaster;
	//[Tooltip("Settings for killing and freezing the puppet.")]
	//public PuppetMaster.StateSettings stateSettings = PuppetMaster.StateSettings.Default;
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
		Damage(damage, 0, new RaycastHit(), Vector3.zero);
	}
	public override void Damage(float damage, float force, RaycastHit hit, Vector3 direction)
	{
		// Update health to reflect damage
		health -= damage;
	}


	public void Knockout()
	{
		state = State.UNCONSCIOUS;
		//if (puppetMaster != null)
			//puppetMaster.Kill(stateSettings);
		disableBehaviours();
	}


	public void Kill() 
	{
		state = State.DEAD;
		//if (puppetMaster != null)
			//puppetMaster.Kill(stateSettings);
		disableBehaviours();
	}


	public void Resurrect()
	{
		Resurrect(health);
	}
	public void Resurrect(float newHealth)
	{
		if (newHealth <= 0)
			return;
		state = State.ALIVE;
		health = Mathf.Max(newHealth, healthMax);
		//if (puppetMaster != null)
			//puppetMaster.Resurrect();
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
