﻿		///----------------------------\\\				
		//  Ultimate Inventory Engine   \\
// Copyright (c) N-Studios. All Rights Reserved. \\
//      https://nikichatv.com/N-Studios.html	  \\
///-----------------------------------------------\\\	



using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    [Header("Equipment")]
    [Tooltip("If you are going to use that item as an equippable one you will have to insert you equipment type in here as a string. This same string must be set up in the slot that is going to support it.")]
    public string equipmentType;

    [Header("Item Settings")]
    [Tooltip("Select the quantity of this item object. When you pick it up you will recive the amount given.")]
    public int amountInStack = 1;
    [Tooltip("Select the maximum possible amount in a single slot.")]
    public int maxStackSize = 1000;
    [Tooltip("Select an ID for this item. Each item MUST have its own UNIQUE ID!")]
    public int ItemID;

    [Header("Appearance")]
    [Tooltip("Select an icon for your item.")]
    public Sprite itemSprite;
    [Tooltip("When this is turned on and you are close enough to be able to pick up the item it will be rendered with outline around it. NOTE: This only works with the Built-in rendering pipeline!")]
    public bool useOutline;

    [HideInInspector]
    public bool close, canBePicked = true;
    [HideInInspector]
    public Vector3 startSize;
    [HideInInspector]
    public Quaternion startRotation;
    [HideInInspector]
    public Inventory player;

    private void Start()
    {
        player = Object.FindObjectOfType<Inventory>();
        //GetComponent<Rigidbody>().useGravity = true;
        startSize = transform.localScale;
        GetComponent<MeshCollider>().convex = true;
        if (GetComponent<SphereCollider>()) GetComponent<SphereCollider>().isTrigger = true;
        else { gameObject.AddComponent<SphereCollider>(); GetComponent<SphereCollider>().isTrigger = true; }
        startRotation = transform.localRotation;
        var rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }

	private void Update()
    {
        if (player != null)
        {
            Vector3 distanceToPlayer = transform.position - player.transform.position;
            if (distanceToPlayer.magnitude <= player.pickupRange) close = true;
            else close = false;
        }

        if (canBePicked)
        {
            if (amountInStack == 0)
            {
                Destroy(gameObject);
                return;
            }
            else if (amountInStack != 0)
            {

            }
        }

        if (close && canBePicked)
        {
            if (useOutline && DetectRenderingPipeline())
			{
                Shader outlineShader = Shader.Find("Outlined/Custom");
                if (GetComponent<MeshRenderer>())
                {
                    GetComponent<MeshRenderer>().material.shader = outlineShader;
                }
                else
                {
                    GetComponentInChildren<MeshRenderer>().material.shader = outlineShader;
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                int possibleAmount = 0;
                Debug.Log(possibleAmount);

                foreach (Slot slot in player.slots)
                {
                    Debug.Log("Here I Exist");
                    if (!slot.gameObject.GetComponent<EquipmentSlot>() && !slot.slotsItem)
                    {
                        Debug.Log("First Item");
                        possibleAmount += maxStackSize;
                    }

                    if (slot.slotsItem && slot.slotsItem.ItemID == ItemID)
                    {
                        Debug.Log("Stacking");
                        possibleAmount += slot.slotsItem.maxStackSize - slot.slotsItem.amountInStack;
                    }
                }

                if (possibleAmount > 0)
                {
                    if (player.readyToAdd)
                    {
                        if (possibleAmount > amountInStack)
                        {
                            Debug.Log("No Stack");
                            player.AddItem(this, null);
                        }
                        else
                        {
                            Debug.Log("Little Guy");
                            var clone = Instantiate(this);
                            clone.amountInStack = possibleAmount;
                            amountInStack -= possibleAmount;
                            player.AddItem(clone);
                        }
                    }
                    else
                    {
                        if (possibleAmount > amountInStack) StartCoroutine(WaitUntilReady(this));
                        else
                        {
                            var clone = Instantiate(this);
                            clone.amountInStack = possibleAmount;
                            amountInStack -= possibleAmount;
                            StartCoroutine(WaitUntilReady(clone));
                        }
                    }
                }
            }
        }
        else
        {
            if (useOutline && DetectRenderingPipeline())
            {
                Shader outlineShader = Shader.Find("Standard");
                if (GetComponent<MeshRenderer>())
                {
                    GetComponent<MeshRenderer>().material.shader = outlineShader;
                }
                else
                {
                    GetComponentInChildren<MeshRenderer>().material.shader = outlineShader;
                }
            }
        }
    }

	IEnumerator WaitUntilReady(Item item)
	{
        yield return new WaitUntil(() => player.readyToAdd == true);
        player.AddItem(item, null);
    }

    private bool DetectRenderingPipeline()
	{
        if (GraphicsSettings.renderPipelineAsset)
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (player) Gizmos.DrawWireSphere(transform.position, player.pickupRange);
        if (!GetComponent<Rigidbody>()) Debug.LogError("You must have a Rigidbody attached to your item!");
    }
}