using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrap : MonoBehaviour, CanBeInteractedWith
{
    [SerializeField]
    uint damage = 2;

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
        if (!this)
            return;

        if (Controller.main == null)
            return;

        if (transform.position == Controller.main.transform.position)
        {
            Controller.main.DamagePlayer(damage, false);
        }
    }

    // lmao
    // if the player is facing a spike and interacts with it, they basically instantly die because it interacts hundreds of times
    public void Interact()
    {
        if (Controller.main == null)
            return;

        Controller.main.DamagePlayer(damage, false);
    }
}
