using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This is used for showing the saves with their details like the progression.
/// </summary>
public class SaveUI : SelectionUI
{
    //Descriptions of the Save. e.g. Save name, save progression, etc.
    [SerializeField]
    private TextMeshProUGUI[] descs; 

    /// <summary>
    /// borderSprites[0] is Selected Border,
    /// borderSprites[1] is Unselected Border;
    /// </summary>
    [SerializeField]
    private Sprite[] borderSprites = new Sprite[2];

    public override void Highlight()
    {
        highlightImage.sprite = borderSprites[0];
        base.Highlight();
    }

    public override void UnHighlight()
    {
        highlightImage.sprite = borderSprites[1];
        base.UnHighlight();
    }


    //This function is used for whoever is working on the LoadSave so you can assign text to this object.
    public void InitializeUI()
    {

    }
}
