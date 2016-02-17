using UnityEngine;
using System.Collections;

// TEST NPC - equip and fire an automatic gun

[RequireComponent (typeof(Inventory))]
[RequireComponent (typeof(AimTarget))]
public class TEST_NPC : MonoBehaviour {

	public Equipment gunToEquip;

	Inventory inv;
	AimTarget aimTarget;

	void Start()
	{		
		if (gunToEquip == null)
			print ("NULL GUN");
		aimTarget = GetComponent<AimTarget>();
		inv = GetComponent<Inventory>();
		inv.aimTarget = aimTarget;
	}
	
	// Update is called once per frame
	void Update () {
		if (gunToEquip != null) {
			inv.AddItem(gunToEquip);
			gunToEquip = null;
		}
		aimTarget.target = this.transform.position + 100f * this.transform.forward;
		inv.TakeInput(Inventory.InputType.WEAPON_FIRE1);
	}
}
