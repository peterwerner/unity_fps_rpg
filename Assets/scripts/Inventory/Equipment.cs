using UnityEngine;
using System.Collections;

/**
 * 	An item in a character's inventory
 * 	Can maintain reference to an equipable in the world 
 * 		When the equipable is picked up, this is added to the character's inventory
 * 	Maintains a viewmodel (attached to the first person player's camera)
 * 		and a worldmodel (attached to third person characters like NPCs)
 * 	It is up to the inventory to activate the world model / view model
 * 		When activating the viewmodel, a camera must be provided accurate targetting
 * 		When activating the worldmodel, a <TODO> must be provided for accurate targetting
 */
public class Equipment : MonoBehaviour {

	public Equipable equipablePrefab;	// Corresponding world item prefab
	public Equipable equipable;
	public Vector3 dropOffset;			// Offset from the camera position when the equipable is dropped
	public InventoryGridItem inventoryGridItem;		// Corresponding UI grid item
	protected AimTarget aimTarget;		// Tracks what this equipment is aiming at
	[HideInInspector] public Inventory inventory;	// Inventory this currently belongs to
	/* Viewmodel */
	public ViewModel viewModelPrefab;	// view model prefab
	protected ViewModel viewModel;		// view model - this is only utilized for the player character
	/* Worldmodel */
	public WorldModel worldModelPrefab;	// world model prefab
	protected WorldModel worldModel;	// world model - this is typically utilized for an NPC


	// Use this for initialization
	public void Start() { BaseStart(); }
	public void BaseStart() 
	{
		if (equipable != null)
			equipable.SetEquipment(this);
	}

	// Update is called once per frame
	public void Update() { BaseUpdate(); }
	public void BaseUpdate()
	{
	}


	/**
	 * 	Act on inputs
	 */
	public virtual void TakeInput(Inventory.InputType input)
	{
	}


	/**
	 *  Instantiate the viewmodel (if a prefab for it exists)
	 * 	and parent it to the given camera
	 */
	public virtual bool ActivateViewModel(Camera viewModelCamera)
	{
		if (viewModelPrefab == null)
			return false;
		viewModel = (ViewModel)Instantiate(viewModelPrefab, viewModelCamera.transform.position, viewModelCamera.transform.rotation);
		viewModel.transform.parent = viewModelCamera.transform;
		return true;
	}


	/**
	 *  Instantiate the world model (if a prefab for it exists)
	 * 	and parent it to the given transform
	 */
	public virtual bool ActivateWorldModel(Transform worldModelParent)
	{
		if (worldModelPrefab == null)
			return false;
		worldModel = (WorldModel)Instantiate(worldModelPrefab, worldModelParent.position, worldModelParent.rotation);
		worldModel.transform.parent = worldModelParent;
		return true;
	}


	/**
	 * Deactivate this - deactivate viewmodel / worldmodel
	 */
	public virtual void Deactivate()
	{
		if (viewModel != null) {
			viewModel.transform.parent = null;
			Destroy(viewModel.gameObject);
			viewModel = null;
		}
		if (worldModel != null) {
			worldModel.transform.parent = null;
			Destroy(worldModel.gameObject);
			worldModel = null;
		}
	}


	/**
	 * 	Spawn the equipable in the world at some position, with some rotation and velocity
	 * 	Return false if the item is already in the world
	 */
	public bool SpawnInWorld()
	{
		return SpawnInWorld(transform.position, transform.rotation, new Vector3(0,0,0));
	}
	public bool SpawnInWorld(Vector3 position, Quaternion rotation, Vector3 velocity)
	{
		Deactivate();
		RemoveFromWorld();
		equipable = (Equipable)Instantiate(equipablePrefab, position, rotation);
		equipable.SetEquipment(this);
		Rigidbody rigidbody = equipable.GetComponent<Rigidbody>();
		if (rigidbody != null)
			rigidbody.velocity = velocity;
		return true;
	}

	/**
	 * 	Remove the equipable world item
	 */
	public bool RemoveFromWorld()
	{
		if (equipable == null)
			return false;
		Destroy (equipable.gameObject);
		return true;
	}


	// Update the aim target
	public void SetAimTarget(AimTarget newTarget)
	{
		aimTarget = newTarget;
	}

}
