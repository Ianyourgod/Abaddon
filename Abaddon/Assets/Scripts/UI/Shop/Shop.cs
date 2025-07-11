using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ImageComponent = UnityEngine.UI.Image; //To make the Image class be the correct image component and not some weird microsoft stuff

public class Shop : MonoBehaviour
{
    [SerializeField]
    float timeToEnter;

    [SerializeField, Range(0, 1)]
    float shopDarknessLevel;

    [SerializeField]
    TextMeshProUGUI itemTitle;

    [SerializeField]
    TextMeshProUGUI statsTextOne;

    [SerializeField]
    TextMeshProUGUI statsTextTwo;

    [SerializeField]
    TextMeshProUGUI costText;

    [SerializeField]
    Button purchase;

    [SerializeField]
    ShopItem[] items;

#nullable enable
    GameObject? current_item = null;
    int current_cost = 0;

    public void Awake()
    {
        Transform UIHolder = gameObject.transform.GetChild(0);
        Transform slots_obj = UIHolder.GetChild(2);

        if (slots_obj.childCount != items.Length)
            throw new Exception("items and slots count do not match");

        ShopSlot[] slots = new ShopSlot[slots_obj.childCount];
        for (int c = 0; c < slots_obj.childCount; c++)
        {
            slots[c] = slots_obj.GetChild(c).gameObject.GetComponent<ShopSlot>();
            slots[c].SetShopItem(items[c]);
        }
    }

    void clearFields()
    {
        purchase.enabled = false;
        costText.text = "";
        statsTextOne.text = "";
        statsTextTwo.text = "";
        itemTitle.text = "";
    }

    public void OnEnable()
    {
        Controller.main.enabled = false;
        UIStateManager.singleton.FadeInDarkener(timeToEnter, shopDarknessLevel);
        clearFields();
    }

    public void OnDisable()
    {
        Controller.main.enabled = true;
        UIStateManager.singleton.FadeOutDarkener(timeToEnter, shopDarknessLevel);
    }

#nullable enable
    public void ItemSelected(ShopItem s_item)
    {
        GameObject prefab = s_item.prefab;
        Item item = prefab.GetComponent<Item>();
        string item_name = s_item.name;
        StatModifier statModifier = prefab.GetComponent<StatModifier>();
        static string num_to_stat(int n)
        {
            return n switch
            {
                0 => "str",
                1 => "dex",
                2 => "con",
                3 => "wis",
                _ => "unkn.",
            };
        }

        costText.text = $"{s_item.cost}";
        itemTitle.text = item_name;
        if (statModifier != null)
        {
            int strength = statModifier.strength;
            int dexterity = statModifier.dexterity;
            int constitution = statModifier.constitution;
            int wisdom = statModifier.wisdom;

            (int, int)[] stats = { (strength, 0), (dexterity, 1), (constitution, 2), (wisdom, 3) };

            var l = stats.ToList();
            l.Sort(
                (a, b) =>
                {
                    return b.Item1.CompareTo(a.Item1);
                }
            );
            (int, int) max = l[0];
            (int, int) second_max = l[1];
            (int, int) min = l[3];

            if (min.Item1 == 0)
            {
                statsTextTwo.text = "";
            }
            else
            {
                statsTextTwo.text =
                    min.Item1 > 0
                        ? $"+{second_max.Item1} {num_to_stat(second_max.Item2)}"
                        : $"-{min.Item1} {num_to_stat(min.Item2)}";
            }

            statsTextOne.text = $"+{max.Item1} {num_to_stat(max.Item2)}";
        }
        else
        {
            statsTextOne.text = "";
            statsTextTwo.text = "";
        }

        current_item = prefab;
        current_cost = s_item.cost;

        purchase.enabled = Controller.main.goldCount >= current_cost;
    }

    public void PurchaseCurrentItem()
    {
        if (Controller.main.goldCount >= current_cost)
        {
            // TODO! play sfx
            Debug.Log("buybuybuy");
            Controller.main.goldCount -= current_cost;
            var item_maybe = Instantiate(current_item);
            GameObject item = item_maybe == null ? throw new Exception("AAAAHHHHH") : item_maybe;
            Item i = item.GetComponent<Item>();
            i.player = Controller.main.inventory;
            i.Pickup(false);
            clearFields();
        }
        else
        {
            // this shouldn't happen, but just in case...
            Debug.Log("you aint nothin but a broke boy boy boy boy");
        }
    }
}

[Serializable]
public struct ShopItem
{
    public GameObject prefab;
    public string name;
    public int cost;
}
