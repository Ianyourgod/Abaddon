using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField] int healAmount = 10;


    public void Consume() {
        GetComponent<UsableSfx>().PlayUseSound();
        Controller.main.HealPlayer(healAmount);
    }
}
