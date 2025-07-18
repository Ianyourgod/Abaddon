using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModifier : MonoBehaviour
{
    [SerializeField]
    public int constitution;

    [SerializeField]
    public int dexterity;

    [SerializeField]
    public int strength;

    [SerializeField]
    public int wisdom;

    public override string ToString()
    {
        return $"Constitution: {constitution}, Dexterity: {dexterity}, Strength: {strength}, Wisdom: {wisdom}";
    }
}
