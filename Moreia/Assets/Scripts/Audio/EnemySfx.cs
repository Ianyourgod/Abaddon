using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySfx : CharacterSfx
{
    [Header("EnemySfx Attributes")]
    [Tooltip("Determines whether the attack sound effect plays when the player takes damager")]
    public bool playAttackOnDamagePlayer = true;
}
