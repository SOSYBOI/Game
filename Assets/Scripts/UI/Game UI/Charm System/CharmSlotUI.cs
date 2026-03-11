using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharmSlotUI : SelectionUI
{
    [SerializeField]
    private Image charmImage;
    [SerializeField]
    private Charm charm;

    [SerializeField]
    private bool equipped;

    /// <summary>
    /// There are three types of borders/slots.
    /// borderSprites[0] is the selected slot.
    /// borderSprites[1] is unselected and unlocked slot.
    /// borderSprites[2] is unselected and locked slot.
    /// </summary>
    [SerializeField]
    private Sprite[] borderSprites;

    public Charm Charm => charm;
    public bool Equipped => equipped;   

    protected override void Start()
    {
        base.Start();
        if(charm != null)
        {
            charmImage.sprite = charm.CharmSprite;

            if (!charm.Unlocked)
            {
                charmImage.color = Color.black;
            }

        }
        else
        {
            charmImage.gameObject.SetActive(false);
        }
    }

    public override void Highlight()
    {
        highlightImage.sprite = borderSprites[0];
        //base.Highlight();
    }



    public override void UnHighlight()
    {
        if (charm == null || !charm.Unlocked)
        {
            highlightImage.sprite = borderSprites[2];
        }
        else
        {
            highlightImage.sprite = borderSprites[1];
        }
        //base.UnHighlight();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        ParentSelector?.UIHover(this);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        ParentSelector?.UIClicked(this);
    }


    public void OnUnlock()
    {
        charmImage.color = Color.white;
    }

    public void OnEquipped()
    {
        charmImage.color = new Color(0.5f, 0.5f, 0.5f);
        equipped = true;
        //charm.SetEquipped(true);
    }

    public void OnUnequipped()
    {
        charmImage.color = Color.white;
        equipped = false;
        //charm.SetSlotID(-1);
        //charm.SetEquipped(false);
    }
}
