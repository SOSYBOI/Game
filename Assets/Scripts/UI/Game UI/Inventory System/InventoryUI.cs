using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : SelectorManager
{
    [SerializeField]
    private ItemDescriptor itemDescriptor;

    public void OnItemUnselected()
    {
        itemDescriptor.gameObject.SetActive(false);
    }

    public override void UIHover()
    {

        for (int i = 0; i < uis.Length; i++)
        {
            if(i != currentIndex)
            {
                uis[i].UnHighlight();
            }
            else
            {
                uis[i].Highlight();
            }
        }

        var slotUI = uis[currentIndex] as InventorySlotUI;
        if (slotUI == null)
        {
            Debug.LogError("The child must be InventorySlotUI");
            return;
        }

        if (slotUI.Item == null || !slotUI.Item.Unlocked){ 
            itemDescriptor.ResetDescription(); 
        }
        else if (slotUI.Item != null && slotUI.Item.Unlocked)
        {
            itemDescriptor.SetDescription(
                                        slotUI.Item.ItemName,
                                        slotUI.Item.ItemDescription,
                                        slotUI.Item.ItemSprite
                                        );
        }
        else
        {
            Debug.LogWarning("Slot selected but Item cannot be null.");
        }
    }

    public override void UIHover(SelectionUI UI)
    {
        UIClicked(UI);
    }

    public override void UIClicked(SelectionUI ui)
    {
        var slotUI = ui as InventorySlotUI;
        if (slotUI == null) { 
            Debug.LogError("The child must be InventorySlotUI");
            return;
        }
        foreach (InventorySlotUI slot in uis)
        {
            if (slotUI != slot) slot.UnHighlight();
            else slot.Highlight();
        }

        if (slotUI.Item != null) { 
            itemDescriptor.SetDescription(slotUI.Item.ItemName, slotUI.Item.ItemDescription, slotUI.Item.ItemSprite);
        }
        else
        {
            Debug.LogError("Slot selected but Item cannot be null.");
        }
    }
}
