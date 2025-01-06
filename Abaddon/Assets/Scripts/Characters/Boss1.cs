using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    [HideInInspector] public bool inFight = false;
    [HideInInspector] public int stage = 0;

    public void StartFight() {
        inFight = true;
        stage = 1;
        Debug.Log("i am 1ssoB, and i hate");
    }
}
