using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatInfoUpdater : MonoBehaviour
{
    private enum Stat
    {
        Wisdom,
        Strength,
        Dexterity,
        Constitution,
        Health,
    }

    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private Stat stat;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (Controller.main == null)
            return;

        switch (stat)
        {
            case Stat.Wisdom:
                text.text = $"Wisdom: {Controller.main.wisdom + Controller.main.wisModifier}";
                break;
            case Stat.Strength:
                text.text = $"Strength: {Controller.main.strength + Controller.main.strModifier}";
                break;
            case Stat.Dexterity:
                text.text = $"Dexterity: {Controller.main.dexterity + Controller.main.dexModifier}";
                break;
            case Stat.Constitution:
                text.text =
                    $"Constitution: {Controller.main.constitution + Controller.main.conModifier}";
                break;
            case Stat.Health:
                text.text = $"{Controller.main.health}";
                break;
        }
    }
}
