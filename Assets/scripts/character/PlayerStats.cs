using UnityEngine;
using System.Collections;

public class PlayerStats : CharacterStats {

	public float energyMax = 100f;
	public float energy = 100f;


	private void Die()
	{
	}

	public override void Kill() 
	{
		base.Kill();
		Die();
	}

	public override void Knockout() 
	{
		base.Knockout();
		Die();
	}

	public override void Resurrect(float newHealth)
	{
		// Do nothing, cannot revive the player.
	}

}
