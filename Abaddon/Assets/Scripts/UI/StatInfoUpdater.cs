using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatInfoUpdater : MonoBehaviour
{
    private enum Stat {
        Wisdom,
        Strength,
        Dexterity,
        Constitution,
        Health
    }

    [SerializeField] private TMP_Text text;
    [SerializeField] private Stat stat;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (stat)
        {
            case Stat.Wisdom:
                text.text = $"{Controller.main.wisdom}";
                break;
            case Stat.Strength:
                text.text = $"{Controller.main.strength}";
                break;
            case Stat.Dexterity:
                text.text = $"{Controller.main.dexterity}";
                break;
            case Stat.Constitution:
                text.text = $"{Controller.main.constitution}";
                break;
            case Stat.Health:
                text.text = $"{Controller.main.health}";
                break;
        }
    }
}
