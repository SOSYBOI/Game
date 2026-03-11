using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : SelectorManager
{
    [SerializeField]
    private ItemDescriptor itemDescriptor;

    protected override void Start()
    {
        EquipmentManager.OnEquipmentUnequipped += UnequipItem;
        EquipmentManager.OnEquipmentSwapped += UnequipItem;
        base.Start();
        UIHover();
    }

    protected override void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            var slotUI = uis[currentIndex] as ItemSlotUI;
            if (slotUI == null)
            {
                Debug.LogError("The Inventory must only have ItemSlotUI as child");
            }

            var equippableItem = slotUI.Item as EquippableItem;

            if (equippableItem != null)
            {
                //Equip Item
                if (!slotUI.Equipped)
                {
                    Debug.Log("Equipping Item");
                    EquipmentManager.OnEquipmentEquipped.Invoke(equippableItem);
                    slotUI.SetEquipped(true);
                }
                else
                {
                    Debug.Log("Unequipping Item");
                    EquipmentManager.OnEquipmentUnequipped.Invoke(equippableItem);
                }

            }

            UIHover();
        }
        //Debug.Log($"Item Slot ID:{currentIndex}");
        base.Update();
    }

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

        //Debug.Log($"Item Slot ID:{currentIndex}");

        var slotUI = uis[currentIndex] as ItemSlotUI;
        if (slotUI == null)
        {
            Debug.LogError("The child must be InventorySlotUI");
            return;
        }

        if (slotUI.Item == null)
        {
            itemDescriptor.ResetDescription();
        }
        else if(!slotUI.Item.Unlocked){
            bool equippable = slotUI.Item is EquippableItem;
            itemDescriptor.ResetDescription(
                                        slotUI.Item.ItemType,
                                        slotUI.Item.Unlocked,
                                        equippable
                                        ); 
        }
        else if (slotUI.Item.Unlocked)
        {
            itemDescriptor.SetDescription(
                                        slotUI.Item,
                                        slotUI.Equipped
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
        var slotUI = ui as ItemSlotUI;
        if (slotUI == null) { 
            Debug.LogError("The child must be InventorySlotUI");
            return;
        }
        foreach (ItemSlotUI slot in uis)
        {
            if (slotUI != slot) slot.UnHighlight();
            else slot.Highlight();
        }

        if (slotUI.Item != null) {
            itemDescriptor.SetDescription(
                                        slotUI.Item,
                                        slotUI.Equipped
                                        );
        }
    }

    private void UnequipItem(EquippableItem item)
    {
        for (int i = 0; i < uis.Length; i++)
        {
            ItemSlotUI slotUI = uis[i] as ItemSlotUI;
            if (slotUI != null)
            {
                EquippableItem eItem = slotUI.Item as EquippableItem;
                if (eItem != null)
                {
                    if(eItem == item)
                    {
                        slotUI.SetEquipped(false);
                        break;
                    }
                }
            }
            else
            {
                break;
            }
        }

        UIHover();
    }
}
