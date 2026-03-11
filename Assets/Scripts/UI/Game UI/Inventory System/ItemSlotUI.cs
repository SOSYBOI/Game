using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Different from SelectionUI is that you don't need to click to select it. 
public class ItemSlotUI : SelectionUI
{
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private ItemData item;

    /// <summary>
    /// There are three types of borders/slots.
    /// borderSprites[0] is the selected slot.
    /// borderSprites[1] is unselected and unlocked slot.
    /// borderSprites[2] is unselected and locked slot.
    /// </summary>
    [SerializeField]
    private Sprite[] borderSprites;
    [SerializeField]
    private bool equipped = false;

    public ItemData Item => item;
    public bool Equipped => equipped;

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
        else
        {
            itemImage.gameObject.SetActive(false);
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

    public void OnUnlock()
    {
        itemImage.color = Color.white;
    }

    public void SetEquipped(bool equipped)
    {
        this.equipped = equipped;
        itemImage.color = (equipped)? new Color(0.5f, 0.5f, 0.5f) : Color.white;
    }
}
