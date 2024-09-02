using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatInfoUpdater : MonoBehaviour
{
    private enum FollowType {
        Wisdom,
        Strength,
        Dexterity,
        Constitution,
        Health
    }

    [SerializeField] private TMP_Text text;
    [SerializeField] private FollowType stat;
    
    void Start()
    {
        switch (stat)
        {
            case FollowType.Wisdom:
                Controller.main.stats.onChangeWisdom += UpdateText;
                break;
            case FollowType.Strength:
                Controller.main.stats.onChangeStrength += UpdateText;
                break;
            case FollowType.Dexterity:
                Controller.main.stats.onChangeDexterity += UpdateText;
                break;
            case FollowType.Constitution:
                Controller.main.stats.onChangeConstitution += UpdateText;
                break;
            case FollowType.Health:
                Controller.main.onHealthChanged += UpdateText;
                break;
        }
    }

    void UpdateText(int value) {
        text.text = $"{value}";
    }
}
