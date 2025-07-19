using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrap : MonoBehaviour
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
        if (!this || Controller.main == null)
            return;

        if (Vector2.Distance(transform.position, Controller.main.transform.position) < 0.5f)
        {
            Controller.main.DamagePlayer(damage, false);
        }
    }
}
