using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrap : MonoBehaviour, Interactable
{
    [SerializeField] uint damage = 2;

    public void Interact() {
        Controller.main.Hurt(damage, false);
    }
}