using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class XpCounter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    // Update is called once per frame
    void Update()
    {
        if (Controller.main == null)
            return;

        text.text = $"xp: {Controller.main.exp}";
    }
}
