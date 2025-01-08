		///----------------------------\\\				
		//  Ultimate Inventory Engine   \\
// Copyright (c) N-Studios. All Rights Reserved. \\
//      https://nikichatv.com/N-Studios.html	  \\
///-----------------------------------------------\\\	



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GraphicRaycaster))]
public class CraftingSlot : MonoBehaviour
{
	[Header("Required Items")]
	[Tooltip("Here you can choose what items are needed for the crafting process (by putting their IDs inside) and the required amount for each of them.")]
    public RequiredItem[] itemsNeeded;

	[Header("Needed References")]
	[Tooltip("Drag your player's inventory here.")]
    public Inventory inv;
	[Tooltip("Make a prefab out of your item that you want to be crafted and assign it here.")]
	public GameObject craftedItemPrefab;
	[Tooltip("Select the quantity of items to create.")]
	public int craftedItemAmount;
	Item curItem;

	private void Start()
	{
		curItem = craftedItemPrefab.GetComponent<Item>(); 
	}

	public IEnumerator CraftItem(bool shiftHeld)
	{
		if (!shiftHeld)
        {
			int possibleAmount = 0;
			foreach (Slot slot in inv.slots)
			{
				if (!slot.gameObject.GetComponent<EquipmentSlot>() && !slot.slotsItem)
				{
					possibleAmount += curItem.maxStackSize;
				}

				if (slot.slotsItem && slot.slotsItem.ItemID == curItem.ItemID)
				{
					possibleAmount += slot.slotsItem.maxStackSize - slot.slotsItem.amountInStack;
				}
			}

			foreach (RequiredItem rq in itemsNeeded)
			{
				if (inv.GetItemAmount(rq.ItemID) < rq.amountNeeded || craftedItemAmount > possibleAmount)
				{
					yield break;
				}
			}
			foreach (RequiredItem rq in itemsNeeded)
			{
				inv.RemoveItemAmount(rq.ItemID, rq.amountNeeded);
			}

			Item newItem = Instantiate(craftedItemPrefab, Vector3.zero, Quaternion.identity).GetComponent<Item>();
			//newItem.
			newItem.amountInStack = craftedItemAmount;

			yield return new WaitForSeconds(0.0001f);

			inv.AddItem(newItem);
			yield return null;
		}
		else if (shiftHeld)
		{
			List<int> allValues = new List<int>();
			int possibleAmount = 0;
			foreach (Slot slot in inv.slots)
			{
				if (!slot.gameObject.GetComponent<EquipmentSlot>() && !slot.slotsItem)
				{
					possibleAmount += curItem.maxStackSize;
				}

				if (slot.slotsItem && slot.slotsItem.ItemID == curItem.ItemID)
				{
					possibleAmount += slot.slotsItem.maxStackSize - slot.slotsItem.amountInStack;
				}
			}
			allValues.Add(possibleAmount);
			foreach (RequiredItem rq in itemsNeeded)
			{
				int invAmount = GetGlobalAmount(rq.ItemID);
				int remainder = invAmount % rq.amountNeeded;
				int final = invAmount - remainder;
				int timesCraftable = final / rq.amountNeeded;
				allValues.Add(timesCraftable);

				if (inv.GetItemAmount(rq.ItemID) < rq.amountNeeded || craftedItemAmount > possibleAmount)
				{
					yield break;
				}
			}

			int times = Mathf.Min(allValues.ToArray());

			for (int i = 0; i < times; i++)
			{
				foreach (RequiredItem rq in itemsNeeded)
				{
					inv.RemoveItemAmount(rq.ItemID, rq.amountNeeded);
				}
			}

			yield return new WaitForSeconds(0.0001f);

			Item newItem = Instantiate(craftedItemPrefab, Vector3.zero, Quaternion.identity).GetComponent<Item>();
			newItem.amountInStack = craftedItemAmount * times;

			inv.AddItem(newItem);
			yield return null;
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
		{
			var obj = GetObjectUnderMouse();
			if (obj == gameObject) StartCoroutine(CraftItem(false));
		}
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
		{
			var obj = GetObjectUnderMouse();
			if (obj == gameObject) StartCoroutine(CraftItem(true));
		}
	}

	int GetGlobalAmount(int ID)
	{
		int globalAmount = 0;

		for (int i = 0; i < inv.slots.Length; i++)
		{
			if (inv.slots[i].slotsItem)
			{
				if (inv.slots[i].slotsItem.ItemID == ID)
				{
					globalAmount += inv.slots[i].slotsItem.amountInStack;
				}
			}
		}

		return globalAmount;
	}

	GameObject GetObjectUnderMouse()
	{
		GraphicRaycaster rayCaster = GetComponent<GraphicRaycaster>();
		PointerEventData eventData = new PointerEventData(EventSystem.current);

		eventData.position = Input.mousePosition;

		List<RaycastResult> results = new List<RaycastResult>();

		rayCaster.Raycast(eventData, results);

		foreach (RaycastResult i in results)
		{
			if (i.gameObject.GetComponent<CraftingSlot>())
			{
				return i.gameObject;
			}
		}
		return null;
	}
}

[System.Serializable]
public class RequiredItem
{
	public int ItemID;
	public int amountNeeded;
}
