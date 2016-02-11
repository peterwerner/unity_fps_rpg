using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 	Maintains a collection of Equipments
 * 		Responsible for removing the corresponding Equipable when adding an Equipment to the inventory
 * 			and for placing the corresponding Equipable back into the world when removing from inventory
 * 		Passes inputs off to the active equipment
 * 	Requires an aimTarget to determine what its Equipments should aim at
 */
public class Inventory : MonoBehaviour {

	// Inputs that can be passed to the inventory / items in the inventory
	public enum InputType { 
		DROP_CURRENT_ITEM, 
		WEAPON_FIRE1_DOWN,
		WEAPON_FIRE1
	}

	public AimTarget aimTarget;			// Passed to equipment (ie: so guns know where to aim)
	public Camera viewModelCamera;		/* If this is set, viewmodels will be used */
	public Transform worldModelParent;	/* If this is set, worldmodels will be used */
	public float dropItemForce;			// Additional velocity to add to items when dropping them
	protected Equipment currentItem;		// ie: what is currently in the character's hands
	private List<Equipment> items = new List<Equipment>();		
	private Vector3 position_previous;	// Position in last update - used to calculate instantaneous velocity
	private Vector3 velocity;	

	// Use this for initialization
	public void Start() { BaseStart(); }
	public void BaseStart() 
	{
	}

	// Update is called once per frame
	public void Update() { BaseUpdate(); }
	public void BaseUpdate()
	{
		// Instantaneous velocity
		velocity = (transform.position - position_previous) / Time.deltaTime;
		position_previous = transform.position;
	}
		

	/**
	 * 	Act on inputs (or pass them on to the active equipment)
	 */
	public void TakeInput(InputType input)
	{
		if (input == InputType.DROP_CURRENT_ITEM) 
		{
			if (currentItem != null)
				DropCurrentItem(velocity);
		}
		else if (currentItem != null)
		{
			currentItem.TakeInput(input);
		}
	}


	/**
	 * 	Attempt to add equipment to this inventory
	 *  And remove the world item
	 */
	public virtual bool Equip(Equipment equipment)
	{
		if (equipment == null || items.Contains(equipment))
			return false;
		items.Add(equipment);
		if (currentItem == null)
			SetCurrentItem(equipment);
		equipment.RemoveFromWorld();
		equipment.SetAimTarget(aimTarget);
		return true;
	}


	/**
	 * 	Set the current item
	 */
	private void SetCurrentItem(Equipment equipment) 
	{
		// Deactivate the previous active model
		if (currentItem != null)
			currentItem.Deactivate();
		// Activate the new active item's viewmodel / worldmodel
		currentItem = equipment;
		if (currentItem != null) {
			if (worldModelParent != null)
				currentItem.ActivateWorldModel(worldModelParent);
			if (viewModelCamera != null)
				currentItem.ActivateViewModel(viewModelCamera);
		}
	}


	/**
	 * 	Drop the current item
	 */
	public virtual void DropCurrentItem(Vector3 velocity) 
	{
		DropItem(currentItem, velocity);
	}
	/**
	 * Drop an item
	 */
	public virtual void DropItem(Equipment item, Vector3 velocity) 
	{
		Vector3 position = this.transform.position;
		Quaternion rotation = this.transform.rotation;
		// Adjust position so the item is dropped in a more natural position
		Vector3 position_offset = position + (rotation * item.dropOffset);
		// Spawn the world item
		Vector3 dropDir = transform.forward + (rotation * new Vector3(0, 0.25f, 0));
		Vector3 dropVelocity = velocity + dropItemForce * dropDir.normalized;
		item.SpawnInWorld(position_offset, rotation, dropVelocity);

		// Deactivate and Remove the item from the inventory
		item.Deactivate();
		items.Remove(item);

		// Update the current item
		if (item == currentItem) {
			if (items.Count <= 0)
				SetCurrentItem(null);
			else
				SetCurrentItem(items[items.Count - 1]);
		}
	}


	public List<Equipment> GetItems()
	{
		return items;
	}

}
