///----------------------------\\\
//  Ultimate Inventory Engine   \\
// Copyright (c) N-Studios. All Rights Reserved. \\
//      https://nikichatv.com/N-Studios.html	  \\
///-----------------------------------------------\\\
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEditor;

public class HotbarParent : MonoBehaviour
{
    [Header("Needed References")]
    [Tooltip("Reference your inventory here.")]
    public Inventory inv;

    [Tooltip(
        "Select where the equpped items will be placed by assigning an empty game object having the right position as its transform."
    )]
    public Transform hand;

    [Tooltip("Select an image that will act as a slot indicator.")]
    public GameObject slotIndicator;

    [Header("Modify")]
    [ReadOnly]
    [Tooltip(
        "Whenever you want to update you item's statistics (eg. your pistol's ammo) this is the object tha will be modified AS WELL AS the \"Item To Modify\"."
    )]
    public GameObject equippedObject;

    [ReadOnly]
    [Tooltip(
        "Whenever you want to update you item's statistics (eg. your pistol's ammo) this is the object tha will be modified AS WELL AS the \"Equpped Object\"."
    )]
    public Item itemToModify;

    [HideInInspector]
    public int selectedItem = 0;

    [HideInInspector]
    public Transform[] slots;

    [HideInInspector]
    public Item[] curItems;

    [HideInInspector]
    public List<Item> itemList;

    [HideInInspector]
    public List<GameObject> clones;

    [HideInInspector]
    public Item oldItem;
    bool checkedForChange;
    bool checkedForDrop;

    void Start()
    {
        List<Transform> localSlots = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
            localSlots.Add(transform.GetChild(i).transform);
        slotIndicator.transform.position = localSlots.ToArray()[1].position;
        slots = localSlots.ToArray();
        if (selectedItem >= slots.Length - 1 && slots.Length >= 1)
            selectedItem = 1;
        else
            selectedItem++;
        SelectItem();
    }

    void Update()
    {
        bool reseted = false;
        for (int i = 0; i < slots.Length; i++)
        {
            if (!reseted)
            {
                itemList.Clear();
                reseted = true;
            }
            if (slots[i].GetComponent<Slot>())
            {
                if (slots[i].GetComponent<Slot>().slotsItem)
                {
                    if (!itemList.Contains(slots[i].GetComponent<Slot>().slotsItem))
                    {
                        itemList.Add(slots[i].GetComponent<Slot>().slotsItem);
                    }
                }
            }
        }
        curItems = itemList.ToArray();

        if (inv.inventoryObject.activeSelf == false)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (selectedItem <= 1 && slots.Length >= 1)
                    selectedItem = slots.Length - 1;
                else
                    selectedItem--;
                SelectItem();
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (selectedItem >= slots.Length - 1 && slots.Length >= 1)
                    selectedItem = 1;
                else
                    selectedItem++;
                SelectItem();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (slots.Length >= 2)
                {
                    selectedItem = 1;
                    SelectItem();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (slots.Length >= 3)
                {
                    selectedItem = 2;
                    SelectItem();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (slots.Length >= 4)
                {
                    selectedItem = 3;
                    SelectItem();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (slots.Length >= 5)
                {
                    selectedItem = 4;
                    SelectItem();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (slots.Length >= 6)
                {
                    selectedItem = 5;
                    SelectItem();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (slots.Length >= 7)
                {
                    selectedItem = 6;
                    SelectItem();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (slots.Length >= 8)
                {
                    selectedItem = 7;
                    SelectItem();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                if (slots.Length >= 9)
                {
                    selectedItem = 8;
                    SelectItem();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                if (slots.Length >= 10)
                {
                    selectedItem = 9;
                    SelectItem();
                }
            }
        }

        if (!checkedForChange)
            StartCoroutine(CheckForChange());
        if (!checkedForDrop)
            StartCoroutine(CheckForDrop());

        if (equippedObject)
        {
            itemToModify = oldItem;
        }
        else
        {
            itemToModify = null;
        }
    }

    public void SelectItem()
    {
        int i = 0;

        foreach (Transform slot in slots)
        {
            if (slot)
            {
                if (i == selectedItem)
                {
                    slotIndicator.transform.position = slot.transform.position;

                    if (slot.GetComponent<Slot>())
                    {
                        if (slot.GetComponent<Slot>().slotsItem)
                        {
                            foreach (Transform tr in hand)
                            {
                                Destroy(tr.gameObject);
                            }

                            if (slot.GetComponent<Slot>().slotsItem)
                            {
                                oldItem = slot.GetComponent<Slot>().slotsItem;
                                GameObject item = Instantiate(
                                    slot.GetComponent<Slot>().slotsItem.gameObject,
                                    hand
                                );
                                item.transform.localScale = slot.GetComponent<Slot>()
                                    .slotsItem.GetComponent<Item>()
                                    .startSize;
                                item.SetActive(true);
                                equippedObject = item;
                                item.GetComponent<Item>().canBePicked = false;
                                if (item.GetComponent<Rigidbody>())
                                    item.GetComponent<Rigidbody>().isKinematic = true;
                                if (item.GetComponent<Item>().useOutline)
                                {
                                    Shader outlineShader = Shader.Find("Standard");
                                    if (item.GetComponent<MeshRenderer>())
                                    {
                                        item.GetComponent<MeshRenderer>().material.shader =
                                            outlineShader;
                                    }
                                    else
                                    {
                                        item.GetComponentInChildren<MeshRenderer>().material.shader =
                                            outlineShader;
                                    }
                                }
                                item.transform.localPosition = new Vector3(0, 0, 0);
                                item.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            }
                            else
                            {
                                foreach (Transform tr in hand)
                                {
                                    Destroy(tr.gameObject);
                                }
                            }
                        }
                        else
                        {
                            foreach (Transform tr in hand)
                            {
                                Destroy(tr.gameObject);
                            }
                        }
                    }
                }
            }

            i++;
        }
    }

    IEnumerator CheckForChange()
    {
        if (slots[selectedItem] != null)
        {
            checkedForChange = true;
            var old = slots[selectedItem].GetComponent<Slot>().slotsItem;
            yield return new WaitUntil(() =>
                old != slots[selectedItem].GetComponent<Slot>().slotsItem
            );
            checkedForChange = false;
            SelectItem();
        }
    }

    IEnumerator CheckForDrop()
    {
        if (slots[selectedItem] != null)
        {
            if (slots[selectedItem].GetComponent<Slot>().slotsItem)
            {
                checkedForDrop = true;
                yield return new WaitUntil(() =>
                    slots[selectedItem].GetComponent<Slot>().slotsItem == null
                );
                checkedForDrop = false;
                SelectItem();
            }
            else
            {
                checkedForDrop = false;
                SelectItem();
            }
        }
    }

    public class ReadOnlyAttribute : PropertyAttribute { }

    /*
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
    */
}
