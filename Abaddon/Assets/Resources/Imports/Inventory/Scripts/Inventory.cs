///----------------------------\\\
//  Ultimate Inventory Engine   \\
// Copyright (c) N-Studios. All Rights Reserved. \\
//      https://nikichatv.com/N-Studios.html	  \\
///-----------------------------------------------\\\



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

[RequireComponent(typeof(SaveAndLoad))]
public class Inventory : MonoBehaviour
{
	[Header("Inventory Settings")]
	[Tooltip("Put your whole inventory object inside here.")]
	public GameObject inventoryObject;
	[Tooltip("Assign your inventory object inside here.")]
	public GameObject equipmentObject;
	[Tooltip("equiptment object.")]
	public GameObject statObject;
	[Tooltip("stat object.")]
	public GameObject craftObject;
	[Tooltip("crafting :scream:")]
	public GameObject levelUpObject;
	[Tooltip("level up object.")]

	public GameObject InventoryTabButton;
	public GameObject LevelUpTabButton;
	public GameObject shadow;
	[Tooltip("Assign the game object thas is a parent for each of your hotbar slots. NOTE: It has to have the \"HotbarParent\" script attached to it.")]
	public HotbarParent hotbarParent;
	[Tooltip("Select a font for the amount displays.")]
	public TMP_FontAsset font;

	[Header("Forces")]
	[Tooltip("Assign an empty game object it's position will be used as the point from where items will be dropped when thrown out of the inventory.")]
	public Transform throwPosition;
	[Tooltip("This is distance from which the player can pick items. The red wire sphere displays the maximum distance in the editor window.")]
	public float pickupRange;
	[Tooltip("This is the upward force that will be applied to the item when throwing it.")]
	public float throwForceUp;
	[Tooltip("This is the forward force that will be applied to the item when throwing it.")]
	public float throwForceForward;
	[Tooltip("Select your movement script of choice. It could be our built-in FPS controller or any other.")]
	public Controller playerMovement;

	[Header("Slots")]
	[Tooltip("Drag all of your slots in here. NOTE: They HAVE to be in ascending order or else the items will not go to the first empty slot but in a random one!")]
	public Slot[] slots;
	[Tooltip("Drag all of your equipment slots in here. NOTE: They HAVE to be in ascending order or else the items will not go to the first empty slot but in a random one!")]
	public Slot[] equipSlots;
	[Tooltip("no tooltip for you")]
	public StatInfoUpdater[] statValues;

	[HideInInspector]
	public List<Slot> emptySlots;
	[HideInInspector]
	public bool readyToAdd = true;

	private UISfx invSfxPlayer;

	private enum Tab
	{
		Inventory,
		LevelUp,
	}

	private Tab current_tab = Tab.Inventory;
	public bool isInventoryOpen = false;

	private void Awake()
	{
		invSfxPlayer = inventoryObject.transform.parent.GetComponent<UISfx>();
	}

	private void Start()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (!slots[i].slotsItem && !slots[i].gameObject.GetComponent<EquipmentSlot>()) emptySlots.Add(slots[i]);
		}

		foreach (Slot slot in slots)
		{
			if (slot) slot.CustomStart();
		}

		foreach (Slot slot in equipSlots)
		{
			if (slot) slot.CustomStart();
		}

		CloseAllTabs();
		HideButtons();
		//Cursor.visible = false;
		//Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		if (UIStateManager.singleton.mostRecentState == UIState.Pause) return;
		if (Input.GetKeyDown(KeyCode.Tab)) UIStateManager.singleton.ToggleUIPage(UIState.Inventory);

		foreach (Slot i in slots)
		{
			if (i) i.CheckForItem();
		}

		foreach (Slot i in equipSlots)
		{
			i.CheckForItem();
		}
	}

	public void OpenInventory()
	{
		invSfxPlayer.PlayOpenSound(); //play the open sound effect on the parent of the inventory object (the inventory ui)

		isInventoryOpen = true;

		ReloadInventory();
		ShowButtons();

		//Cursor.visible = true;
		//Cursor.lockState = CursorLockMode.None;

		playerMovement.enabled = false;
		if (shadow) shadow.SetActive(true);
	}

	public void CloseInventory()
	{
		invSfxPlayer.PlayCloseSound(); //play the close sound on the parent of the inventory object (the inventory ui)

		CloseAllTabs();
		HideButtons();

		isInventoryOpen = false;
		//Cursor.visible = false;
		//Cursor.lockState = CursorLockMode.Locked;
		playerMovement.enabled = true;
		if (shadow) shadow.SetActive(false);
	}

	public bool CheckIfItemExists(int id)
	{
		// more efficient !!!!!!
		foreach (Slot slot in slots)
		{
			if (slot.slotsItem)
			{
				Item z = slot.slotsItem;
				if (z.ItemID == id)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SwapTuahTheStats()
	{
		current_tab = Tab.LevelUp;
		ReloadInventory();
	}

	public void ClearInventory()
	{
		foreach (Slot i in slots.Union(equipSlots))
		{
			if (!i) continue;

			Destroy(i.slotsItem?.gameObject);
			i.slotsItem = null;
		}
	}

	public void SwapToInventory()
	{
		current_tab = Tab.Inventory;
		ReloadInventory();
	}

	public void CloseAllTabs()
	{
		inventoryObject.SetActive(false);
		equipmentObject.SetActive(false);
		statObject.SetActive(false);
		craftObject.SetActive(false);
		levelUpObject.SetActive(false);
	}

	public void HideButtons()
	{
		InventoryTabButton.SetActive(false);
		LevelUpTabButton.SetActive(false);
	}

	public void ShowButtons()
	{
		InventoryTabButton.SetActive(true);
		LevelUpTabButton.SetActive(true);
	}

	public void ReloadInventory()
	{
		CloseAllTabs();

		switch (current_tab)
		{
			case Tab.Inventory:
				inventoryObject.SetActive(true);
				equipmentObject.SetActive(true);
				statObject.SetActive(true);
				craftObject.SetActive(true);
				break;
			case Tab.LevelUp:
				levelUpObject.SetActive(true);
				break;
		}
	}

	public int GetItemAmount(int id)
	{
		int num = 0;
		foreach (Slot slot in slots)
		{
			if (slot.slotsItem)
			{
				Item z = slot.slotsItem;
				if (z.ItemID == id)
				{
					num += z.amountInStack;
				}
			}
		}
		return num;
	}

	public void RemoveItemAmount(int id, int amountToRemove)
	{
		foreach (Slot i in slots)
		{
			if (amountToRemove <= 0)
			{
				return;
			}

			if (i.slotsItem)
			{
				Item z = i.slotsItem;
				if (z.ItemID == id)
				{
					int amountThatCanBeRemoved = z.amountInStack;
					if (amountThatCanBeRemoved <= amountToRemove)
					{
						Destroy(z.gameObject);
						amountToRemove -= amountThatCanBeRemoved;
					}
					else
					{
						z.amountInStack -= amountToRemove;
						amountToRemove = 0;
					}
				}
			}
			// not using a hotbar
			// hotbarParent.SelectItem();
		}
	}

	public void RemoveByID(int id)
	{
		foreach (Slot i in slots)
		{
			if (i.slotsItem)
			{
				Item z = i.slotsItem;
				if (z.ItemID == id)
				{
					Destroy(z.gameObject);
				}
			}
		}
	}


	public void AddItems(List<Item> itemsToBeAdded)
	{
		IEnumerator waiter()
		{
			foreach (Item i in itemsToBeAdded)
			{
				AddItem(i);
				print($"Added item: {i.name} | {emptySlots.Count}/{slots.Length} empty slots left");
				yield return new WaitForSeconds(0.003f);
			}
		}
		StartCoroutine(waiter());
	}

	public void AddItem(Item itemToBeAdded, Item startingItem = null)
	{
		if (readyToAdd)
		{
			int amountInStack = itemToBeAdded.amountInStack;
			List<Item> stackableItems = new List<Item>();
			List<Slot> empty = new List<Slot>();
			emptySlots = empty;

			for (int i = 0; i < slots.Length; i++)
			{
				if (!slots[i].slotsItem && !slots[i].gameObject.GetComponent<EquipmentSlot>()) empty.Add(slots[i]);
			}

			if (startingItem && startingItem.ItemID == itemToBeAdded.ItemID && startingItem.amountInStack < startingItem.maxStackSize)
			{
				stackableItems.Add(startingItem);
			}

			foreach (Slot slot in slots)
			{
				if (slot.slotsItem)
				{
					Item itemOfCurSlot = slot.slotsItem;
					if (itemOfCurSlot.ItemID == itemToBeAdded.ItemID && itemOfCurSlot.amountInStack < itemOfCurSlot.maxStackSize && itemOfCurSlot != startingItem)
					{
						stackableItems.Add(itemOfCurSlot);
					}
				}
			}

			foreach (Item i in stackableItems)
			{
				int amountThatCanBeAdded = i.maxStackSize - i.amountInStack;
				if (amountThatCanBeAdded > i.maxStackSize)
				{
					i.amountInStack = i.maxStackSize;
					amountInStack -= amountThatCanBeAdded;
				}
				else if (amountThatCanBeAdded <= i.maxStackSize)
				{
					i.amountInStack += amountInStack;
					if (i.amountInStack > i.maxStackSize)
					{
						i.transform.parent.gameObject.GetComponent<Slot>().slotsItem = null;
						i.transform.parent = null;
						for (int z = 0; z < slots.Length; z++)
						{
							if (!slots[z].slotsItem && !slots[z].gameObject.GetComponent<EquipmentSlot>()) empty.Add(slots[z]);
						}
						AddItem(i);
					}

					Destroy(itemToBeAdded.gameObject);
					return;
				}
			}

			itemToBeAdded.amountInStack = amountInStack;

			foreach (Slot i in empty)
			{
				if (itemToBeAdded.amountInStack > itemToBeAdded.maxStackSize)
				{
					Item newItem = Instantiate(itemToBeAdded.gameObject).GetComponent<Item>();
					newItem.amountInStack = newItem.maxStackSize;
					itemToBeAdded.amountInStack -= newItem.maxStackSize;
					newItem.transform.parent = i.transform;
					newItem.gameObject.SetActive(false);
				}
				else
				{
					itemToBeAdded.transform.parent = i.transform;
					itemToBeAdded.transform.gameObject.SetActive(false);
					break;
				}
			}

			StartCoroutine(ItemWait());
		}
	}


	IEnumerator ItemWait()
	{
		readyToAdd = false;
		yield return new WaitForSeconds(0.001f);
		readyToAdd = true;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, pickupRange);
	}
}
