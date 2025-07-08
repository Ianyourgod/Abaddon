using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrap : MonoBehaviour, CanBeInteractedWith
{
    [SerializeField] uint damage = 2;

    void Awake()
    {
        Controller.OnMoved += CustomUpdate;
    }

    void OnDestroy()
    {
        Controller.OnMoved -= CustomUpdate;
    }

    void CustomUpdate()
    {
        if (!this) return;
        if (transform.position == Controller.main.transform.position)
        {
            Controller.main.DamagePlayer(damage, false);
        }
    }

    public void Interact()
    {
        Controller.main.DamagePlayer(damage, false);
    }
}