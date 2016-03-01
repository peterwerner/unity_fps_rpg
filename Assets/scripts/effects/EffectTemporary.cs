using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class EffectTemporary : MonoBehaviour {

	public float decaySpeed = 25;
	public SpriteRenderer sprite;
	private float alpha;

	// Use this for initialization
	void Start () {
		alpha = UnityEngine.Random.Range(0.4f, 0.7f);
	}
	
	// Update is called once per frame
	void Update () {
		alpha -= decaySpeed * Time.deltaTime;
		if (alpha <= 0)
			Destroy(this.gameObject);
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
	}
}
