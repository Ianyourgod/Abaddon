using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionButton : MonoBehaviour
{
    [SerializeField] Image image;
    
    void Start()
    {
        image.enabled = false;
    }

    void Update()
    {
        image.enabled = Controller.main.CanStartConversation();
    }
}
