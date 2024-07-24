		///----------------------------\\\				
		//  Ultimate Inventory Engine   \\
// Copyright (c) N-Studios. All Rights Reserved. \\
//      https://nikichatv.com/N-Studios.html	  \\
///-----------------------------------------------\\\	



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
	[Header("Item")]
	[Tooltip("This is the current item that is inside the slot. If there is not an item it this box this means your slot is currently empty.")]
	[ReadOnly]
	public Item slotsItem;

	Sprite defaultSprite;
	[HideInInspector]
	Image itemImage;
	[HideInInspector]
	public Text amountText;
	[HideInInspector]
	public bool beingDragged, beingSplitted;
	[HideInInspector]
	public GameObject vLayer, vText, clone, thrownItem;
	Inventory inv;
	DragAndDrop drag;
	bool textChecked;

	public void CustomStart()
	{
			drag = GameObject.FindObjectOfType<DragAndDrop>();
			#region Placement

			inv = Object.FindObjectOfType<Inventory>();
			defaultSprite = GetComponent<Image>().sprite;

			var VisualLayer = new GameObject("GUI");
			VisualLayer.transform.SetParent(gameObject.transform);
			VisualLayer.AddComponent<RectTransform>();
			var text = new GameObject("Text");
			text.transform.SetParent(VisualLayer.transform);
			vLayer = VisualLayer;
			vText = text;
			vLayer.AddComponent<CanvasRenderer>();
			vLayer.AddComponent<Image>();
			vText.AddComponent<RectTransform>();
			vText.AddComponent<Text>();

			amountText = text.GetComponent<Text>();
			amountText.text = "";
			#endregion
		
	}

	private void Update()
	{
		if (Input.GetMouseButtonUp(0)) if (clone) Destroy(clone);

		if (vLayer)
		{
			vLayer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
			vLayer.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
			vLayer.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
			vLayer.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
			vLayer.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
			vLayer.GetComponent<RectTransform>().offsetMax = new Vector2(-0, vLayer.GetComponent<RectTransform>().offsetMax.y);
			vLayer.GetComponent<RectTransform>().offsetMax = new Vector2(vLayer.GetComponent<RectTransform>().offsetMax.x, -0);
			itemImage = vLayer.GetComponent<Image>();
		}
		if (vText && !beingDragged)
		{

			vText.GetComponent<Text>().alignment = TextAnchor.LowerRight;
			vText.GetComponent<Text>().fontSize = 35;
			var font = inv.font;
			vText.GetComponent<Text>().font = font;
			vText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
			vText.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
			vText.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
			vText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
			vText.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
			vText.GetComponent<RectTransform>().offsetMax = new Vector2(-10, vText.GetComponent<RectTransform>().offsetMax.y);
			vText.GetComponent<RectTransform>().offsetMax = new Vector2(vText.GetComponent<RectTransform>().offsetMax.x, -0);
		}

		if (!GetComponent<EquipmentSlot>() && !GetComponent<CraftingSlot>())
		{
			if (!beingDragged)
			{
				if (amountText)
				{
					if (slotsItem)
					{
						if (slotsItem.amountInStack > 1)
						{
							if (slotsItem && slotsItem.amountInStack > 1) StartCoroutine(Wait(0.0001f));
							amountText.text = slotsItem.amountInStack.ToString();
							amountText.transform.SetParent(transform.GetChild(0).transform);
							if (textChecked && !beingSplitted)
							{
								textChecked = false;
							}
						}
						else
						{
							if (slotsItem.amountInStack == 0)
							{
								Destroy(slotsItem.gameObject);
							}
							amountText.transform.SetParent(transform.GetChild(0).transform);
							amountText.text = "";
							if (textChecked && !beingSplitted)
							{
								textChecked = false;
							}
						}
					}
					else
					{
						amountText.text = "";
					}
				}

				if (itemImage)
				{
					if (slotsItem)
					{
						if (slotsItem.amountInStack > 0) itemImage.sprite = slotsItem.itemSprite;
						else
						{
							Destroy(slotsItem.gameObject);
							itemImage.sprite = defaultSprite;
						}
					}
					else itemImage.sprite = defaultSprite;
				}
			}
			else if (beingDragged)
			{
				amountText.gameObject.SetActive(false);
				if (itemImage) itemImage.sprite = defaultSprite;
				if (!textChecked && transform.GetChild(0).transform.GetChild(0))
				{
					if (slotsItem.amountInStack > 1)
					{
						clone = Instantiate(amountText.gameObject);
						clone.gameObject.SetActive(true);
						clone.transform.SetParent(drag.followMouseImage.transform);
						clone.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
						clone.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
						clone.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
						clone.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
						clone.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
						clone.GetComponent<RectTransform>().offsetMax = new Vector2(-0, clone.GetComponent<RectTransform>().offsetMax.y);
						clone.GetComponent<RectTransform>().offsetMax = new Vector2(clone.GetComponent<RectTransform>().offsetMax.x, -0);
						clone.GetComponent<Text>().fontSize = 10 / 11 * amountText.fontSize;
					}
					textChecked = true;
					StartCoroutine(WaitUntil(!beingDragged));
				}
			}
			if (beingSplitted && !beingDragged)
			{
				if ((slotsItem.amountInStack - Mathf.RoundToInt(slotsItem.amountInStack / 2)) > 1) amountText.text = (slotsItem.amountInStack - Mathf.RoundToInt(slotsItem.amountInStack / 2)).ToString();
				else amountText.text = "";

				if (!textChecked && transform.GetChild(0).transform.GetChild(0))
				{
					clone = null;
					if (Mathf.RoundToInt(slotsItem.amountInStack / 2) > 1)
					{
						clone = Instantiate(amountText.gameObject);
						clone.GetComponent<Text>().text = Mathf.RoundToInt(slotsItem.amountInStack / 2).ToString();
						clone.transform.SetParent(drag.followMouseImage.transform);
						clone.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
						clone.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
						clone.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
						clone.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
						clone.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
						clone.GetComponent<RectTransform>().offsetMax = new Vector2(-0, clone.GetComponent<RectTransform>().offsetMax.y);
						clone.GetComponent<RectTransform>().offsetMax = new Vector2(clone.GetComponent<RectTransform>().offsetMax.x, -0);
						clone.GetComponent<Text>().fontSize = 10 / 11 * amountText.fontSize;
					}
					textChecked = true;
					return;
				}
			}

			if (transform.childCount <= 1 && itemImage) itemImage.sprite = defaultSprite;
		}
		else if (GetComponent<EquipmentSlot>() && !GetComponent<CraftingSlot>())
		{
			Debug.Log(slotsItem);
			if (!beingDragged)
			{
				if (slotsItem)
				{
					if (slotsItem.amountInStack > 0) itemImage.sprite = slotsItem.itemSprite;
					else itemImage.sprite = defaultSprite;
				}
				else
				{
					itemImage.sprite = defaultSprite;
					amountText.text = "";
				}
				if (transform.childCount <= 1) itemImage.sprite = defaultSprite;
			}
			else
			{
				itemImage.sprite = defaultSprite;
			}
		}
	}

	public void DropItem(int removeQuantity, GameObject player, bool removeAll)
	{
		if (slotsItem)
		{
			if (!removeAll)
			{
				for (int i = 0; i < removeQuantity; i++)
				{
					if (slotsItem.amountInStack > 0)
					{
						thrownItem = Instantiate(slotsItem.gameObject, player.GetComponent<Inventory>().throwPosition.transform.position, Quaternion.identity);

						thrownItem.gameObject.SetActive(true);
						thrownItem.transform.localScale = slotsItem.startSize;
						thrownItem.transform.SetParent(null);

						thrownItem.GetComponent<Item>().amountInStack = 1;

						var rb = thrownItem.GetComponent<Rigidbody>();
						rb.isKinematic = false;
						rb.useGravity = true;

						//AddForce
						rb.interpolation = RigidbodyInterpolation.Extrapolate;
						rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
						rb.AddForce(player.transform.forward * player.GetComponent<Inventory>().throwForceForward, ForceMode.Impulse);
						rb.AddForce(player.transform.up * player.GetComponent<Inventory>().throwForceUp, ForceMode.Impulse);

						float random = Random.Range(-1f, 1f);
						rb.AddTorque(new Vector3(random, random, random) * 10);
					}
					else
					{
						Destroy(slotsItem.gameObject);
					}
					slotsItem.amountInStack -= removeQuantity;

					if (slotsItem && slotsItem.amountInStack == 0)
					{
						if (gameObject.GetComponent<EquipmentSlot>())
						{
							Destroy(transform.GetChild(1).gameObject);
						}
					}
				}

			}
			else if (removeAll)
			{
				if (slotsItem.amountInStack > 0)
				{
					thrownItem = Instantiate(slotsItem.gameObject, player.GetComponent<Inventory>().throwPosition.transform.position, Quaternion.identity);

					thrownItem.gameObject.SetActive(true);
					thrownItem.transform.localScale = slotsItem.startSize;
					thrownItem.GetComponent<Item>().amountInStack = slotsItem.amountInStack;

					Rigidbody rb = thrownItem.GetComponent<Rigidbody>();
					rb.isKinematic = false;
					rb.useGravity = true;

					rb.interpolation = RigidbodyInterpolation.Extrapolate;
					rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
					rb.AddForce(player.transform.forward * player.GetComponent<Inventory>().throwForceForward, ForceMode.Impulse);
					rb.AddForce(player.transform.up * player.GetComponent<Inventory>().throwForceUp, ForceMode.Impulse);
					//Add random rotation
					float random = Random.Range(-3f, 3f);
					rb.AddTorque(new Vector3(random, random, random) * 10);
				}
				else
				{
					Destroy(slotsItem.gameObject);
				}

				slotsItem.amountInStack = 0;
			}
		}
		inv.hotbarParent.SelectItem();
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

	IEnumerator WaitUntil(bool used)
	{
		yield return new WaitUntil(() => used = true);
		amountText.gameObject.SetActive(true);
	}

	IEnumerator Wait(float time)
	{
		yield return new WaitForSeconds(time);
		if (slotsItem && slotsItem.amountInStack >= 2) amountText.gameObject.SetActive(true);
	}

	public void CheckForItem()
	{
		if (transform.childCount > 1)
		{
			slotsItem = transform.GetChild(1).GetComponent<Item>();
		}

		else
		{
			slotsItem = null;
		}
	}
	public class ReadOnlyAttribute : PropertyAttribute
	{

	}

	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property,
												GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position,
								   SerializedProperty property,
								   GUIContent label)
		{
			GUI.enabled = false;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}
	}
}
