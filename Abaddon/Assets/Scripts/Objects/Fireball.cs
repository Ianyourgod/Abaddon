using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] uint lastingTime = 2;
    [SerializeField] uint damage = 8;

    private uint timeAlive = 0;

    void Awake()
    {
        if (transform.position == Controller.main.transform.position)
        {
            Controller.main.DamagePlayer(damage, false);
        }
    }

    void OnDestroy()
    {
    }

    // TODO: ADD A NEW WAY TO CALL THIS
    void CustomUpdate()
    {
        timeAlive++;
        if (timeAlive > lastingTime)
        {
            Destroy(gameObject);
        }

        if (transform.position == Controller.main.transform.position)
        {
            Controller.main.DamagePlayer(damage, false);
        }
    }
}