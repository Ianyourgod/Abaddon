using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    [SerializeField]
    GameObject[] requiredEnemies;
    bool allEnemiesDead = false;

    void Update()
    {
        if (allEnemiesDead)
        {
            return;
        }

        allEnemiesDead = true;
        foreach (GameObject enemy in requiredEnemies)
        {
            if (enemy != null)
            {
                allEnemiesDead = false;
                return;
            }
        }

        // set all children to active
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
