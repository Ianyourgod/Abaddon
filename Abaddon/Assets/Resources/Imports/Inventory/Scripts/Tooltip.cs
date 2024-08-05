		///----------------------------\\\				
		//  Ultimate Inventory Engine   \\
// Copyright (c) N-Studios. All Rights Reserved. \\
//      https://nikichatv.com/N-Studios.html	  \\
///-----------------------------------------------\\\	



using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [Header("Drag & Drop")]
    [Tooltip("Drag your canvas object containing the \"DragAndDrop\" script here.")]
    public DragAndDrop drag;
    [Header("Objects")]
    [Tooltip("Add your player object here. It has to have the \"Inventory\" script attached to it.")]
    public GameObject invObject;
    [Tooltip("Add your main tooltip object here.")]
    public GameObject tooltipObject;

    [Header("Visuals")]
    [Tooltip("For each text you want to be shown with the tooltip assign it here.")]
    public Text[] tooltipTexts;
    [Tooltip("If you have images that you want to be shown together with the tooltip itself add them here.")]
    public Image[] tooltipImages;

    [HideInInspector]
    public GameObject hovered;


    void Awake()
    {
        var back = tooltipObject.GetComponent<Image>();
        back.color = new Color(back.color.r, back.color.g, back.color.g, 0f);
		foreach (Text tooltipText in tooltipTexts)
		{
            var text = tooltipText.GetComponent<Text>();
            text.enabled = true;
            text.gameObject.SetActive(true);
            text.color = new Color(text.color.r, text.color.g, text.color.g, 0f);
        }

        foreach (Image image in tooltipImages)
        {
            var comp = image.GetComponent<Image>();
            comp.enabled = true;
            comp.gameObject.SetActive(true);
            comp.color = new Color(comp.color.r, comp.color.g, comp.color.g, 0f);
        }
    }

    private void Update()
    {
        if (invObject.activeSelf == true)
		{
            hovered = drag.GetObjectUnderMouse(false);
            if (hovered && (hovered.gameObject == gameObject || hovered.gameObject == transform.parent.gameObject || hovered.gameObject == tooltipObject))
            {
                ShowToolTip();
            }
            else
			{
                HideTooltip();
			}
        }
    }
    public void ShowToolTip()
    {
        foreach (Text tooltipText in tooltipTexts)
        {
            var text = tooltipText.GetComponent<Text>();
            text.color = new Color(text.color.r, text.color.g, text.color.g, Mathf.Lerp(text.color.a, 1f, 0.05f));
        }

        foreach (Image image in tooltipImages)
        {
            var comp = image.GetComponent<Image>();
            comp.color = new Color(comp.color.r, comp.color.g, comp.color.g, Mathf.Lerp(comp.color.a, 1f, 0.05f));
        }

        var back = tooltipObject.GetComponent<Image>();
        back.color = new Color(back.color.r, back.color.g, back.color.g, Mathf.Lerp(back.color.a, 1f, 0.05f));
    }

    public void HideTooltip()
	{
        foreach (Text tooltipText in tooltipTexts)
        {
            var text = tooltipText.GetComponent<Text>();
            text.color = new Color(text.color.r, text.color.g, text.color.g, Mathf.Lerp(text.color.a, 0f, 0.05f));
        }

        foreach (Image image in tooltipImages)
        {
            var comp = image.GetComponent<Image>();
            comp.color = new Color(comp.color.r, comp.color.g, comp.color.g, Mathf.Lerp(comp.color.a, 0f, 0.05f));
        }

        var back = tooltipObject.GetComponent<Image>();
        back.color = new Color(back.color.r, back.color.g, back.color.g, Mathf.Lerp(back.color.a, 0f, 0.05f));
    }

}
