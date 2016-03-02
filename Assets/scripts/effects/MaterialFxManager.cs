using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(AudioSource))]
public class MaterialFxManager : SingletonComponent<MaterialFxManager> {

	[SerializeField] private MaterialFxInstance defaultEffects;
	[SerializeField] private MaterialFxSerialize[] materialSpecificEffects;
	[SerializeField] private float impactSoundLevel = 7;
	private Dictionary<PhysicMaterial, MaterialFxInstance> map;
	private AudioSource audioSrc;


	void Awake() 
	{
		Setup();
		audioSrc = GetComponent<AudioSource>();
		map = new Dictionary<PhysicMaterial, MaterialFxInstance>();
	}


	void Start() 
	{
		// Move all the serialized data into the hashmap, then get rid of the serialized list
		foreach (MaterialFxSerialize serialized in materialSpecificEffects) {
			foreach (PhysicMaterial mat in serialized.materials)
				map.Add(mat, new MaterialFxInstance(serialized));
		}
		materialSpecificEffects = null;
	}


	public void DoBulletImpactFx(RaycastHit hit) 
	{
		PhysicMaterial mat = hit.collider.sharedMaterial;
		audioSrc.transform.position = hit.point;
		audioSrc.PlayOneShot(BulletImpactSound(mat));
		Sound.MakeSound(audioSrc.transform.position, Sound.Type.GUNSHOT, impactSoundLevel, 1);
		EffectTemporary effect = (EffectTemporary)Instantiate(BulletImpactEffect(mat), hit.point, Quaternion.identity);
		effect.transform.forward = hit.normal;
		effect.transform.parent = hit.collider.transform;
	}


	public AudioClip BulletImpactSound(PhysicMaterial material)
	{
		if (material == null)
			return defaultEffects.GetImpactSound();
		MaterialFxInstance effects;
		bool success = map.TryGetValue(material, out effects);
		if (success)
			return effects.GetImpactSound();
		return defaultEffects.GetImpactSound();
	}


	public EffectTemporary BulletImpactEffect(PhysicMaterial material)
	{
		if (material == null)
			return defaultEffects.GetImpactEffect();
		MaterialFxInstance effects;
		bool success = map.TryGetValue(material, out effects);
		if (success)
			return effects.GetImpactEffect();
		return defaultEffects.GetImpactEffect();
	}
}
