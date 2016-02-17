using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

/**
 *  Handle user input from the player
 */
[RequireComponent (typeof(Inventory))]
public class InputPlayer : MonoBehaviour {
	
	new public Camera camera;
	public float maxUseDist = 1;
	public LayerMask layerMask;			// Layermask for interacting with the world
	public AimTarget aimTarget;
	public PauseUI pauseUi;
	public PickUpPhysics pickUp;

	private Inventory inventory;	// Inventory to interact with

		
	void Start () 
	{
		this.inventory = GetComponent<Inventory>();
	}


	// Update is called once per frame
	void Update () 
	{
		if (CrossPlatformInputManager.GetButtonDown("OpenInventory")) {
			pauseUi.Toggle();
		}

		if (!pauseUi.IsActive())
		{
			/* USE KEY - interact with the world */
			if (CrossPlatformInputManager.GetButtonDown("Use"))
			{
				if (pickUp.IsHolding())
					pickUp.Drop();
				else
				{
					// Cast a ray from the camera
					RaycastHit hit;
					Ray ray = new Ray(camera.transform.position, aimTarget.target - camera.transform.position);
					if (Physics.Raycast(ray, out hit, maxUseDist, layerMask)) {
						Transform objHit = hit.transform;

						// Attempt to equip item to inventory
						Equipable equipable = objHit.GetComponent<Equipable>();
						if (equipable != null) {
							inventory.AddItem(equipable.GetEquipment());
						}

						// Attempt to pickup physics objects
						else if (!pickUp.IsHolding()) {
							Rigidbody rb = objHit.GetComponent<Rigidbody>();
							if (rb)
								pickUp.Grab(rb, hit);
						}

						// Attempt to use useable objects in the world
						Useable useable = objHit.GetComponent<Useable>();
						if (useable != null) {
							useable.Use();
						}
					}
				}
			}

			if (CrossPlatformInputManager.GetButtonDown("Fire1")) 
				inventory.TakeInput(Inventory.InputType.WEAPON_FIRE1_DOWN);

			if (CrossPlatformInputManager.GetButton("Fire1"))
				inventory.TakeInput(Inventory.InputType.WEAPON_FIRE1);

			if (CrossPlatformInputManager.GetButtonDown("Fire3")) 
				inventory.TakeInput(Inventory.InputType.DROP_CURRENT_ITEM);

			if (CrossPlatformInputManager.GetButtonDown("Reload")) 
				inventory.TakeInput(Inventory.InputType.WEAPON_RELOAD);
		
		}
	}
		
}
