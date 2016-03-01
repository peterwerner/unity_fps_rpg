using UnityEngine;
using System.Collections;

public class PhysMaterialAssigner : MonoBehaviour {

	[SerializeField] private PhysicMaterial material;

	// Use this for initialization
	void Start () 
	{
		AssignMaterialToChildren();
	}

	private void AssignMaterialToChildren() 
	{
		Collider col = this.GetComponent<Collider>();
		if (col)
			col.sharedMaterial = material;
		foreach (Collider collider in this.GetComponentsInChildren<Collider>())
			collider.sharedMaterial = material;
	}

}
