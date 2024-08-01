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
using System.Linq;

[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(StandaloneInputModule))]
[RequireComponent(typeof(EventSystem))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(UISfx))]

public class DragAndDrop : MonoBehaviour
{
	[Header("Main Objects")]
	[Tooltip("Put your player here.")]
	public Inventory inv;
	[HideInInspector]
	[Tooltip("Assign your inventory object in here.")]
	public GameObject invObj;
	[Tooltip("Create an empty UI image and put it as last child of the Canvas object")]
	public Image followMouseImage;

	[HideInInspector]
	public GameObject curSlot;
	[HideInInspector]
	public Item curSlotsItem;
	[HideInInspector]
	public Text followMouseText;
	[HideInInspector]
	public int clicks;
	[HideInInspector]
	public Item oldItem;
	[HideInInspector]
	public GameObject oldSlot;

	[HideInInspector]
	public readonly float betweenDrop = 0.1f;
	[HideInInspector]
	public float timer;
	bool started;

	UISfx sfxPlayer;

	[HideInInspector]
	public List<Item> itemAmountsDescending, itemAmountsAscending, positiveItems;

	int amountToAdd;
	int globalAmount;
	Item[] identicalItems;
	Item[] descendingAmounts;
	Item[] ascendingAmounts;
	[HideInInspector]
	public bool shouldCheck;

	private float last_click;

	void Awake()
    {
		sfxPlayer = GetComponent<UISfx>();
    }

	private void Start()
	{
		invObj = inv.inventoryObject;
		followMouseImage.color = new Color(followMouseImage.color.r, followMouseImage.color.g, followMouseImage.color.b, 0);
	}

	private void Update()
	{
		if (followMouseImage.transform.childCount > 1) for (int i = 0; i < followMouseImage.transform.childCount; i++) if (i < followMouseImage.transform.childCount) Destroy(followMouseImage.transform.GetChild(i));

		timer -= Time.deltaTime;


		if (invObj.activeSelf)
		{
			if (curSlot && curSlotsItem)
			{
				//gsdoihureiugkbsdj.Log(curSlot);
				//fdst.Log(curSlotsItem);
				//var textChild = curSlotsItem.transform.parent.transform.GetChild(0).transform.GetChild(0);
				/*textChild.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);	
				textChild.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
				textChild.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
				textChild.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
				textChild.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
				textChild.GetComponent<RectTransform>().offsetMax = new Vector2(-10, textChild.GetComponent<RectTransform>().offsetMax.y);
				textChild.GetComponent<RectTransform>().offsetMax = new Vector2(textChild.GetComponent<RectTransform>().offsetMax.x, -0);*/
			}

			followMouseImage.transform.position = Input.mousePosition;

			if (Input.GetKey(KeyCode.Q) && timer <= 0 && !Input.GetKey(KeyCode.LeftControl))
			{
				GameObject obj = GetObjectUnderMouse(true);
				if (obj)
				{
					if (obj.GetComponent<Slot>())
					{
						if (obj.transform.childCount > 1)
						{
							if (obj.transform.GetChild(1).GetComponent<Item>().amountInStack > 0)
							{
								obj.GetComponent<Slot>().DropItem(1, inv.gameObject, false);
								timer = betweenDrop;
							}
						}
					}
				}
			}
			else if (Input.GetKey(KeyCode.LeftControl) && timer <= 0 && Input.GetKey(KeyCode.Q))
			{
				GameObject obj = GetObjectUnderMouse(true);
				if (obj)
				{
					if (obj.GetComponent<Slot>())
					{
						if (obj.transform.childCount > 1)
						{
							if (obj.transform.GetChild(1).GetComponent<Item>().amountInStack > 0)
							{
								obj.GetComponent<Slot>().DropItem(0, inv.gameObject, true);
								timer = betweenDrop;
							}
						}
					}
				}
			}

			if (Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
			{
				curSlot = GetObjectUnderMouse(true);
				GameObject obj = GetObjectUnderMouse(true);
				if (curSlot)
				{
					if (curSlot.gameObject.GetComponent<Slot>())
					{
						if (curSlot.GetComponent<Slot>().slotsItem)
						{
							clicks++;

							if (!started && Time.time - last_click < 0.25)
							{
								Item objectsItem = obj.GetComponent<Slot>().slotsItem;
								DoubleClick(objectsItem);
								return;
							}
							last_click = Time.time;
							sfxPlayer.PlayPickupSound();
						}
					}
				}
			}
			else if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
			{
				if (curSlot && curSlot.GetComponent<Slot>().slotsItem)
				{
					followMouseImage.color = new Color(255, 255, 255);
					if (curSlot.transform.childCount > 1) followMouseImage.sprite = curSlot.transform.GetChild(1).GetComponent<Item>().itemSprite;
					if (curSlot) curSlot.GetComponent<Slot>().beingDragged = true;
					if (curSlot) curSlot.GetComponent<Slot>().beingSplitted = false;
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
				if (curSlot && curSlot.GetComponent<Slot>().slotsItem)
				{
					if (!GetComponent<EquipmentSlot>())
                    {
						sfxPlayer.PlayPlaceSound();
					}
                    else
                    {
						sfxPlayer.PlayEquipSound();
                    }
					Destroy(curSlot.GetComponent<Slot>().clone);
					curSlotsItem = curSlot.GetComponent<Slot>().slotsItem;

					GameObject newObj = GetObjectUnderMouse(true);
					if (newObj && newObj != curSlot)
					{
						if (newObj.GetComponent<EquipmentSlot>() && newObj.GetComponent<EquipmentSlot>().equipmentType != curSlotsItem.equipmentType)
						{
							return;
						}

						if (newObj.GetComponent<Slot>().slotsItem)
						{
							Item objectsItem = newObj.GetComponent<Slot>().slotsItem;
							if (objectsItem.ItemID == curSlotsItem.ItemID && objectsItem.amountInStack + curSlotsItem.amountInStack <= objectsItem.maxStackSize && !newObj.GetComponent<EquipmentSlot>())
							{
								curSlotsItem.transform.parent = null;
								inv.AddItem(curSlotsItem, objectsItem);
							}
							else if (objectsItem.ItemID == curSlotsItem.ItemID && objectsItem.amountInStack + curSlotsItem.amountInStack > objectsItem.maxStackSize && !newObj.GetComponent<EquipmentSlot>())
							{
								int needed = objectsItem.maxStackSize - objectsItem.amountInStack;
								objectsItem.amountInStack += needed;
								curSlotsItem.amountInStack -= needed;
							}
							else
							{
								if (curSlotsItem.equipmentType == objectsItem.equipmentType || !curSlot.GetComponent<EquipmentSlot>())
								{
									objectsItem.transform.parent = curSlot.transform;
									curSlotsItem.transform.parent = newObj.transform;
								}
							}
						}
						else
						{
							if (curSlotsItem) curSlotsItem.transform.parent = newObj.transform;
						}
					}
					curSlot.GetComponent<Slot>().beingDragged = false;
				}
			}

			if (Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButton(0))
			{
				curSlot = GetObjectUnderMouse(true);
			}
			else if (Input.GetMouseButton(1) && !Input.GetMouseButton(0))
			{
				if (curSlot && curSlot.GetComponent<Slot>().slotsItem)
				{
					curSlotsItem = curSlot.GetComponent<Slot>().slotsItem;
					if (curSlotsItem.amountInStack > 1)
					{
						if (curSlot) curSlot.GetComponent<Slot>().beingSplitted = true;
						followMouseImage.color = new Color(255, 255, 255);
						if (curSlot.transform.childCount > 1) followMouseImage.sprite = curSlot.transform.GetChild(1).GetComponent<Item>().itemSprite;
						oldItem = curSlotsItem;
						oldSlot = curSlot;
					}
					else
					{
						followMouseImage.color = new Color(255, 255, 255);
						if (curSlot.transform.childCount > 1) followMouseImage.sprite = curSlot.transform.GetChild(1).GetComponent<Item>().itemSprite;
						if (curSlot) curSlot.GetComponent<Slot>().beingDragged = true;
						if (curSlot) curSlot.GetComponent<Slot>().beingSplitted = false;
					}
				}
			}
			else if (Input.GetMouseButton(1) && !Input.GetMouseButton(0))
			{
				if (curSlot && curSlot.GetComponent<Slot>().slotsItem)
				{
					followMouseImage.color = new Color(255, 255, 255);
					if (curSlot.transform.childCount > 1) followMouseImage.sprite = curSlot.transform.GetChild(1).GetComponent<Item>().itemSprite;
					if (curSlot && curSlotsItem && curSlotsItem.amountInStack > 1) curSlot.GetComponent<Slot>().beingSplitted = true;
					print("dada");
					if (curSlot) curSlot.GetComponent<Slot>().beingDragged = false;
				}
			}
			else if (Input.GetMouseButtonUp(1))
			{
				if (curSlot && curSlot.GetComponent<Slot>().slotsItem)
				{
					curSlotsItem = curSlot.GetComponent<Slot>().slotsItem;
					if (curSlot) curSlot.GetComponent<Slot>().beingSplitted = false;

					if (curSlotsItem.amountInStack > 1)
					{
						GameObject newObj = GetObjectUnderMouse(true);
						Destroy(curSlot.GetComponent<Slot>().clone);
						if (newObj && newObj != curSlot)
						{
							if (newObj.GetComponent<EquipmentSlot>() && newObj.GetComponent<EquipmentSlot>().equipmentType != curSlotsItem.equipmentType)
							{
								return;
							}

							if (newObj.GetComponent<Slot>().slotsItem)
							{
								Item objectsItem = newObj.GetComponent<Slot>().slotsItem;
								if (objectsItem.ItemID == curSlotsItem.ItemID && objectsItem.amountInStack != objectsItem.maxStackSize && !newObj.GetComponent<EquipmentSlot>())
								{
									GameObject objectUnder = GetObjectUnderMouse(true);
									var item = objectUnder.GetComponent<Slot>().slotsItem;
									if (item.amountInStack + Mathf.RoundToInt(curSlotsItem.amountInStack / 2) > item.maxStackSize)
									{
										int amountPossible = item.maxStackSize - item.amountInStack;
										item.amountInStack = item.maxStackSize;
										curSlotsItem.amountInStack -= amountPossible;
									}
									else
									{
										item.amountInStack += Mathf.RoundToInt(curSlotsItem.amountInStack / 2);
										curSlotsItem.amountInStack -= Mathf.RoundToInt(curSlotsItem.amountInStack / 2);
									}
								}
								else
								{
									return;
								}
							}
							else
							{
								curSlot.GetComponent<Slot>().beingSplitted = false;
								var clone = Instantiate(curSlotsItem);
								clone.amountInStack -= Mathf.RoundToInt(curSlotsItem.amountInStack / 2);
								if (oldSlot) clone.transform.parent = oldSlot.transform;
								else clone.transform.parent = curSlot.transform;
								curSlotsItem.transform.parent = newObj.transform;
								curSlotsItem.amountInStack = Mathf.RoundToInt(curSlotsItem.amountInStack / 2);
							}
						}
						curSlot.GetComponent<Slot>().beingDragged = false;
					}
					else
					{
						if (curSlot && curSlot.GetComponent<Slot>().slotsItem)
						{
							curSlotsItem = curSlot.GetComponent<Slot>().slotsItem;

							GameObject newObj = GetObjectUnderMouse(true);
							if (newObj && newObj != curSlot)
							{
								if (newObj.GetComponent<EquipmentSlot>() && newObj.GetComponent<EquipmentSlot>().equipmentType != curSlotsItem.equipmentType)
								{
									return;
								}

								if (newObj.GetComponent<Slot>().slotsItem)
								{
									Item objectsItem = newObj.GetComponent<Slot>().slotsItem;
									if (objectsItem.ItemID == curSlotsItem.ItemID && objectsItem.amountInStack != objectsItem.maxStackSize && !newObj.GetComponent<EquipmentSlot>())
									{
										curSlotsItem.transform.parent = null;
										inv.AddItem(curSlotsItem, objectsItem);
									}
									else
									{
										objectsItem.transform.parent = curSlot.transform;
										curSlotsItem.transform.parent = newObj.transform;
									}
								}
								else
								{
									curSlotsItem.transform.parent = newObj.transform;
								}
							}
							curSlot.GetComponent<Slot>().beingDragged = false;	
						}
					}
				}
			}


			if (!((Input.GetMouseButton(0) || Input.GetMouseButton(1)) || (Input.GetMouseButton(0) && Input.GetMouseButton(1))))
			{
				followMouseImage.sprite = null;
				followMouseImage.color = new Color(0, 0, 0, 0);
				if (curSlot) curSlot.GetComponent<Slot>().beingDragged = false;
				if (curSlot) curSlot.GetComponent<Slot>().beingSplitted = false;
			}

			if (shouldCheck)
			{
				Item clicked = curSlotsItem;

				if (amountToAdd > 0)
				{
					for (int i = 0; i < identicalItems.Length; i++)
					{
						if (amountToAdd > 0)
						{
							if (ascendingAmounts[i] != clicked)
							{
								if (ascendingAmounts[i].amountInStack <= amountToAdd)
								{
									clicked.amountInStack += ascendingAmounts[i].amountInStack;
									amountToAdd -= ascendingAmounts[i].amountInStack;
									ascendingAmounts[i].amountInStack = 0;
								}
								else if (ascendingAmounts[i].amountInStack > amountToAdd)
								{
									clicked.amountInStack += amountToAdd;
									ascendingAmounts[i].amountInStack -= amountToAdd;
									amountToAdd = 0;
								}
							}
							else if (ascendingAmounts[i] == clicked)
							{
								int z = identicalItems.Length;

								if (i + 1 > z)
								{
									if (ascendingAmounts[i - 1].amountInStack > amountToAdd)
									{
										clicked.amountInStack += amountToAdd;
										ascendingAmounts[i].amountInStack -= amountToAdd;
										amountToAdd = 0;
									}
								}
							}
						}
						else
						{
							shouldCheck = false;
							return;
						}
					}
				}
				else
				{
					shouldCheck = false;
					return;
				}
			}
		}
		else
		{
			if (Input.GetKey(KeyCode.Q) && timer <= 0 && !Input.GetKey(KeyCode.LeftControl) && inv.hotbarParent.equippedObject)
			{
				GameObject obj = null;
				if (inv.hotbarParent.oldItem) { obj = inv.hotbarParent.oldItem.transform.parent.gameObject; }
				if (obj)
				{
					if (obj.GetComponent<Slot>())
					{
						if (obj.transform.childCount > 1)
						{
							if (obj.transform.GetChild(1).GetComponent<Item>().amountInStack > 0)
							{
								obj.GetComponent<Slot>().DropItem(1, inv.gameObject, false);
								timer = betweenDrop;
							}
						}
					}
				}
			}
			else if (Input.GetKey(KeyCode.LeftControl) && timer <= 0 && Input.GetKey(KeyCode.Q))
			{
				GameObject obj = null;
				if (inv.hotbarParent.oldItem) { obj = inv.hotbarParent.oldItem.transform.parent.gameObject; }
				if (obj)
				{
					if (obj.GetComponent<Slot>())
					{
						if (obj.transform.childCount > 1)
						{
							if (obj.transform.GetChild(1).GetComponent<Item>().amountInStack > 0)
							{
								obj.GetComponent<Slot>().DropItem(1, inv.gameObject, true);
								timer = betweenDrop;
							}
						}
					}
				}
			}
		}
	}

	public void DoubleClick(Item clickedItem)
	{
		print("clicking");
		if (clickedItem.TryGetComponent(out Potion potion)) {
			print("clicking");
			potion.Consume();
			inv.RemoveItemAmount(clickedItem.ItemID, 1);
		}
	}

	public List<Item> GetItem(int type, int ID)
	{
		List<Item> items = new List<Item>();
		items.Clear();

		foreach (Slot slot in inv.slots) 
		{
			if (slot.slotsItem)
			{
				if (slot.slotsItem.ItemID == ID) 
				{
					items.Add(slot.slotsItem.GetComponent<Item>());
				}
			}
		}

		return items.OrderByDescending(o => o.amountInStack).ToList();
	}

	public List<Item> GetIdenticalItems(int ID)
	{
		List<Item> items = new List<Item>();
		items.Clear();

		foreach (Slot slot in inv.slots)
		{
			if (slot.slotsItem)
			{
				if (slot.slotsItem.ItemID == ID)
				{
					items.Add(slot.slotsItem.GetComponent<Item>());
				}
			}
		}

		return items;
	}

	public GameObject GetObjectUnderMouse(bool slotsOnly)
	{
		if (slotsOnly == true)
		{
			GraphicRaycaster rayCaster = GetComponent<GraphicRaycaster>();
			PointerEventData eventData = new PointerEventData(EventSystem.current);

			eventData.position = Input.mousePosition;

			List<RaycastResult> results = new List<RaycastResult>();

			rayCaster.Raycast(eventData, results);

			foreach (RaycastResult i in results)
			{
				if (i.gameObject.GetComponent<Slot>())
				{
					return i.gameObject;
				}
			}
			return null;
		}
		else
		{
			GraphicRaycaster rayCaster = GetComponent<GraphicRaycaster>();
			PointerEventData eventData = new PointerEventData(EventSystem.current);

			eventData.position = Input.mousePosition;

			List<RaycastResult> results = new List<RaycastResult>();

			rayCaster.Raycast(eventData, results);

			foreach (RaycastResult i in results)
			{
				if (i.gameObject != followMouseImage.gameObject)
				{
					return i.gameObject;
				}
			}
			return null;
		}
	}
}
