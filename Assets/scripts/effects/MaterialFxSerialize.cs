using UnityEngine;
using System.Collections;
using System;

/**
 * Exists only to help serialize data from the editor
 * Data from the serialized instances of this are dumped into a hashmap
 * And then this is deleted
 */
[Serializable]
public class MaterialFxSerialize {

	[SerializeField] public PhysicMaterial[] materials;
	[SerializeField] public AudioClip[] impactSounds;
	[SerializeField] public EffectTemporary impactEffect;

}
