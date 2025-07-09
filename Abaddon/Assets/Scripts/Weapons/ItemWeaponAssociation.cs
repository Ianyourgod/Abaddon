using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class ItemWeaponAssociation : MonoBehaviour
{
    [SerializeField] Weapon weaponType;
    [SerializeField] int one = 1;

    public Weapon GetWeaponType()
    {
        return weaponType;
    }
}