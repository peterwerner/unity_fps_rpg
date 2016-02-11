using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

/**
 *  Handle user input from the player
 */
[RequireComponent (typeof(Inventory))]
public class InputPlayer : MonoBehaviour {
	
	new public Camera camera;
	public float maxUseDist = 1;
	public LayerMask layerMask;			// Layermask for interacting with the world
	public InventoryUI inventoryUI;
	public FirstPersonController controller;
	public Crosshair crosshair;
	public AimTarget aimTarget;

	private Inventory inventory;	// Inventory to interact with
	private float timeScalePrev;
	private CameraFx cameraFx;

		
	void Start () {
		this.inventory = GetComponent<Inventory>();
		cameraFx = camera.GetComponent<CameraFx>();
	}


	// Update is called once per frame
	void Update () {
		if (CrossPlatformInputManager.GetButtonDown("OpenInventory")) {
			if (!inventoryUI.IsEnabled()) {
				ActivateUI();
				inventoryUI.Enable();
			}
			else {
				DeactivateUI();
				inventoryUI.Disable();
			}
		}

		if (!inventoryUI.IsEnabled())
		{
			/* USE KEY - interact with the world */
			if (CrossPlatformInputManager.GetButtonDown("Use"))
			{
				// Cast a ray from the camera
				RaycastHit hit;
				Ray ray = new Ray(camera.transform.position, aimTarget.target - camera.transform.position);
				if (Physics.Raycast(ray, out hit, maxUseDist, layerMask)) {
					Transform objHit = hit.transform;

					// Attempt to equip item to inventory
					Equipable equipable = objHit.GetComponent<Equipable>();
					if (equipable != null) {
						inventory.Equip(equipable.GetEquipment());
					}

					// Attempt to use useable objects in the world
					Useable useable = objHit.GetComponent<Useable>();
					if (useable != null) {
						useable.Use();
					}
			
				}
			}

			if (CrossPlatformInputManager.GetButtonDown("Fire1")) 
				inventory.TakeInput(Inventory.InputType.WEAPON_FIRE1_DOWN);

			if (CrossPlatformInputManager.GetButton("Fire1"))
				inventory.TakeInput(Inventory.InputType.WEAPON_FIRE1);

			if (CrossPlatformInputManager.GetButtonDown("Fire3")) 
				inventory.TakeInput(Inventory.InputType.DROP_CURRENT_ITEM);
		
		}
	}


	/**
	 * 	Activate / Deactivate UI mode
	 * 	Disable character controller input
	 * 	Hide crosshair
	 */
	private void ActivateUI()
	{
		if (crosshair)
			crosshair.enabled = false;
		controller.ActivateGuiMode();
		timeScalePrev = Time.timeScale;
		Time.timeScale = 0;
		if(cameraFx)
			cameraFx.ActivateMenuEffects();
	}
	private void DeactivateUI()
	{
		if (crosshair)
			crosshair.enabled = true;
		controller.DeactivateGuiMode();
		Time.timeScale = timeScalePrev;
		if(cameraFx)
			cameraFx.DeactivateMenuEffects();
	}
}
