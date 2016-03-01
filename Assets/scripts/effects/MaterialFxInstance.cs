using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class MaterialFxInstance {

	[SerializeField] private AudioClip[] impactSounds;
	[SerializeField] private EffectTemporary impactEffect;


	public MaterialFxInstance(MaterialFxSerialize serialized) 
	{
		this.impactSounds = serialized.impactSounds;
		this.impactEffect = serialized.impactEffect;
	}


	public AudioClip GetImpactSound() 
	{ 
		int i = UnityEngine.Random.Range(0, impactSounds.Length);
		return impactSounds[i]; 
	}

	public EffectTemporary GetImpactEffect()
	{
		return impactEffect;
	}

}
