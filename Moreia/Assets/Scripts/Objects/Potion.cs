using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField] int healAmount = 10;

    public void Consume(UISfx sfxPlayer) {
        print("healing for " + healAmount);
        sfxPlayer.PlayUseSound(0);
        Controller.main.HealPlayer(healAmount);
        Destroy(gameObject);
    }
}
