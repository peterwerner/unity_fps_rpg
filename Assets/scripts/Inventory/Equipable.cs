using UnityEngine;
using System.Collections;

/**
 *	An item in the world that is equipable 
 */
[RequireComponent (typeof(Rigidbody))]
public class Equipable : MonoBehaviour {

	// Corresponding inventory item
	private Equipment equipment;


	public void SetEquipment(Equipment e) { equipment = e; }
	public Equipment GetEquipment() { return equipment; }
}
