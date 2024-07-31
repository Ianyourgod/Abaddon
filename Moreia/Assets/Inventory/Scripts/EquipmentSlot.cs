		///----------------------------\\\				
		//  Ultimate Inventory Engine   \\
// Copyright (c) N-Studios. All Rights Reserved. \\
//      https://nikichatv.com/N-Studios.html	  \\
///-----------------------------------------------\\\	



using UnityEngine;

[RequireComponent(typeof(Slot))]
public class EquipmentSlot : MonoBehaviour
{
	[Header("Type of Equipment")]
	[Tooltip("Put the type of equipment that can be placed in this slot e.g., head, body, arms, etc. Then for all items that you want to be placeable in these slots make sure you have entered the matching string inside their equipment type box (head, body, arms, etc.).")]
	public string equipmentType;

	[Header("Items Tab")]
	[Tooltip("This is the item that is currently in the slot. If the box here is empty this means the slot is empty as well.")]
	public Item curItem;
	[Tooltip("You need to drag and drop here every single object that may be equipped. You also need to make sure that you only reference objects that are suitable for this slot's equipment type.")]
	public EqippableItem[] possibleEqips;

	[HideInInspector]
	public GameObject curItemObj;
	[HideInInspector]
	public bool equipped;
	[HideInInspector]
	public int currentEquippedItemID;
	int searchID;
	private int[] savedStats = new int[4];

	private void Update()
	{
		curItem = GetComponent<Slot>().slotsItem;
		if (GetComponent<Slot>().slotsItem && !equipped) Equip();
		if (GetComponent<Slot>().slotsItem == null && equipped) Unequip();
	}

	public void Equip()
	{
		currentEquippedItemID = GetComponent<Slot>().slotsItem.ItemID;
		Debug.Log("Equip");
		equipped = true;
		Controller.main.EquipItem(currentEquippedItemID, false);
		EqippableItem assigned = null;
		if (GetComponent<Slot>().slotsItem)
		{
			var item = GetComponent<Slot>().slotsItem;
			searchID = item.ItemID;
			if (item.TryGetComponent(out StatModifier slotModifier)) {
				print("has stats");
				
				Controller.main.dexterity += slotModifier.dexterity;
				Controller.main.constitution += slotModifier.constitution;
				Controller.main.strength += slotModifier.strength;
				Controller.main.wisdom += slotModifier.wisdom;
				savedStats = new int[] {
					slotModifier.dexterity,
					slotModifier.constitution,
					slotModifier.strength,
					slotModifier.wisdom
				};
			}
			for (int i = 0; i < possibleEqips.Length; i++)
			{
				if (possibleEqips[i].ItemID == searchID)
				{
					assigned = possibleEqips[i];
					curItem = GetComponent<Slot>().slotsItem;
					possibleEqips[i].gameObject.SetActive(true);
				}
			}
			if (assigned == null || !assigned.gameObject.activeSelf)
			{
				Debug.LogError("The piece of equipment you are currently trying to equip wasn't found inside the \"Possible Eqips\" array!");
				return;
			}
		}
		print(curItem.gameObject.name);
	}

	public void Unequip()
	{
		
		equipped = false;
		Controller.main.EquipItem(currentEquippedItemID, true);
		currentEquippedItemID = -1;
		var item = GetComponent<Slot>().slotsItem;
		print("item");

		Controller.main.dexterity -= savedStats[0];
		Controller.main.constitution -= savedStats[1];
		Controller.main.strength -= savedStats[2];
		Controller.main.wisdom -= savedStats[3];
		savedStats = new int[] { 0, 0, 0, 0 };
		foreach (EqippableItem eqippableItem in possibleEqips)
		{
			if (eqippableItem.ItemID == searchID)
			{
				eqippableItem.gameObject.SetActive(false);
				curItem = GetComponent<Slot>().slotsItem;
			}
		}
	}
}
