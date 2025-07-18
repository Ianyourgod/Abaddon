using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField]
    public int healAmount = 10;

    public void Consume()
    {
        if (Controller.main == null)
            return;

        GetComponent<SfxPlayerBetter>().PlaySound("drink");
        Controller.main.HealPlayer(healAmount);
    }
}
