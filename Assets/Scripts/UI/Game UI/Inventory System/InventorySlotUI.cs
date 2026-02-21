using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Different from SelectionUI is that you don't need to click to select it. 
public class InventorySlotUI : SelectionUI
{
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private Item item;

    /// <summary>
    /// There are three types of borders/slots.
    /// borderSprites[0] is the selected slot.
    /// borderSprites[1] is unselected and unlocked slot.
    /// borderSprites[2] is unselected and locked slot.
    /// </summary>
    [SerializeField]
    private Sprite[] borderSprites;

    private InventoryUI ParentInventory;

    public Item Item => item;

    protected override void Start()
    {
        base.Start();
        if(item != null)
        {
            itemImage.sprite = item.ItemSprite;

            if (!item.Unlocked)
            {
                itemImage.color = Color.black;
            }

        }
    }

    public override void Highlight()
    {
        highlightImage.sprite = borderSprites[0];
        //base.Highlight();
    }



    public override void UnHighlight()
    {
        if (item == null || !item.Unlocked)
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
        ParentInventory?.UIHover(this);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        ParentInventory?.UIClicked(this);
    }


    public void OnUnlock()
    {
        itemImage.color = Color.white;
    }
}
