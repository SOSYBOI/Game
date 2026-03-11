using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;


//This is used to show the Armor Inventory.
public class EquipmentSlotUI : SelectionUI, IPointerExitHandler
{
    [SerializeField]
    private Image itemImage;

    /// <summary>
    /// There are three types of borders/slots.
    /// borderSprites[0] is the selected slot.
    /// borderSprites[1] is unselected slot.
    /// </summary>
    [SerializeField]
    private Sprite[] borderSprites;
    private EquippableItem equippedItem;
    private bool equipped = false;

    public EquippableItem EquippedItem => equippedItem;
    public bool Equipped => equipped;

    protected override void Start()
    {
        base.Start();
        if (equippedItem != null)
        {
            itemImage.sprite = equippedItem.ItemSprite;

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
        highlightImage.sprite = borderSprites[1];
        //base.UnHighlight();
    }

    public void Equip(EquippableItem item)
    {
        equippedItem = item;
        itemImage.sprite = item.ItemSprite;
        itemImage.gameObject.SetActive(true);
        equipped = true;
    }

    public void Unequip()
    {
        equippedItem = null;
        itemImage.sprite = null;
        itemImage.gameObject.SetActive(false);
        equipped = false;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        ToolTipSystem.Instance.OnToolTipSelected(this);
        base.OnPointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipSystem.Instance.CloseToolTip();
    }
}
