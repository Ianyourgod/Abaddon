using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is so stats actually update even when the inventory is open (controller is disabled)

[RequireComponent(typeof(Controller))]
public class StatUpdater : MonoBehaviour {
    public enum Stat {
        Wisdom,
        Strength,
        Dexterity,
        Constitution,
    }

    public void AttemptToBuy(Stat stat, int xp_cost, int amount) {
        if (Controller.main.exp >= xp_cost) {
            Controller.main.exp -= xp_cost;
            AddToStat(stat, amount);
        }
    }

    public void AddToStat(Stat stat, int amount) {
        switch (stat) {
            case Stat.Wisdom:
                Controller.main.wisdom += amount;
                break;
            case Stat.Strength:
                Controller.main.strength += amount;
                break;
            case Stat.Dexterity:
                Controller.main.dexterity += amount;
                break;
            case Stat.Constitution:
                Controller.main.constitution += amount;
                break;
        }
        Controller.main.UpdateStats();
    }

    public void IncreaseConstitution(int cost) {
        AttemptToBuy(Stat.Constitution, cost, 1);
    }
}