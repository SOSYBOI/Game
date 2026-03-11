using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipCharmSlotUI : SelectionUI
{
    [SerializeField]
    private Image charmImage;

    /// <summary>
    /// There are three types of borders/slots.
    /// borderSprites[0] is the selected slot.
    /// borderSprites[1] is unselected slot.
    /// </summary>
    [SerializeField]
    private Sprite[] borderSprites;

    private Charm equipCharm;

    public Charm EquippedCharm => equipCharm;

    protected override void Start()
    {
        base.Start();
        if(equipCharm == null)
        {
            charmImage.gameObject.SetActive(false);
            return;
        }

        //charmImage.gameObject.SetActive(equipCharm.Equipped);
    }

    public void RemoveCharm()
    {
        charmImage.sprite = null;
        charmImage.gameObject.SetActive(false);
        equipCharm = null;
    }

    public void EquipCharm(Charm charm)
    {
        charmImage.sprite = charm.CharmSprite;
        charmImage.gameObject.SetActive(true);
        equipCharm = charm;
    }

    public override void Highlight()
    {
        highlightImage.sprite = borderSprites[0];
    }



    public override void UnHighlight()
    {
        highlightImage.sprite = borderSprites[1];
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        ParentSelector?.UIHover(this);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        ParentSelector?.UIClicked(this);
    }

}
