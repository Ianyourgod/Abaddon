using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySwapper : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite spriteOne;
    [SerializeField] Sprite spriteTwo;
    [SerializeField] Sprite spriteThree;

    public enum Ability
    {
        Attack,
        TempAbility,
        Other2
    }

    private static int currently_selected_int = 0;
    [HideInInspector] public static Ability currently_selected = Ability.Attack;

    void Start()
    {
        image.sprite = spriteOne;
        setSprite(currently_selected);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currently_selected_int--;
            currently_selected_int = currently_selected_int < 0 ? 2 : currently_selected_int;
            currently_selected = (Ability)currently_selected_int;
            setSprite(currently_selected);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            currently_selected_int++;
            currently_selected_int = currently_selected_int > 2 ? 0 : currently_selected_int;
            currently_selected = (Ability)currently_selected_int;
            setSprite(currently_selected);
        }
    }

    void setSprite(Ability n)
    {
        switch (n)
        {
            case Ability.Attack: image.sprite = spriteOne; break;
            case Ability.TempAbility: image.sprite = spriteTwo; break;
            case Ability.Other2: image.sprite = spriteThree; break;
        }
    }

    public static BaseAbility getAbility(Controller player)
    {
        return player.GetComponent<NormalAttack>();

        // switch (currently_selected) {
        //     case Ability.Attack: return player.GetComponent<NormalAttack>();
        //     case Ability.TempAbility: return player.GetComponent<TempAbility>();
        //     case Ability.Other2: return player.GetComponent<NormalAttack>();
        // }
        // // unreachable but c# is balls so we have to return something
        // return player.GetComponent<NormalAttack>();
    }
}
