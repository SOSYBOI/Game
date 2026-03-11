using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipCharmInventoryUI : SelectorManager
{
    [SerializeField]
    private CharmDescriptor charmDescriptor;

    private int nextEmptySlot = 0;

    protected override void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        CharmManager.OnCharmEquipped += EquipCharm;
        CharmManager.OnCharmUnequipped += RemoveCharm;
        CharmManager.OnUIModeChanged += UIHover;
        currentIndex = 0;

        if (uis.Length < 2)
        {
            Debug.LogError("Menu has only one option");
            return;
        }
        uis[0].Highlight();

        for (int i = 1; i < uis.Length; i++)
        {
            uis[i].UnHighlight();
        }

    }

    protected override void Update()
    {
        if (!CharmManager.EquippedCharmMode) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            //Unequip
            var slotUI = uis[currentIndex] as EquipCharmSlotUI;
            if (slotUI == null)
            {
                Debug.LogError("Equip Charm Inventory must only have EquipCharmSlotUI as child");
                return;
            }
            if (slotUI.EquippedCharm == null) return;
            CharmCostManager.Instance.DecreaseCost(slotUI.EquippedCharm.CharmCost);
            CharmManager.OnCharmUnequipped.Invoke(slotUI.EquippedCharm);
            RemoveCharm();
        }


        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentIndex > 0)
            {
                currentIndex--;
            }
            UIHover();

        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentIndex < uis.Length - 1)
            {
                currentIndex++;
            }
            UIHover();

        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            CharmManager.EquippedCharmMode = false;
            CharmManager.OnUIModeChanged.Invoke();
            UIHover();
        }
    }

    public override void UIHover()
    {
        if (CharmManager.EquippedCharmMode)
        {
            for (int i = 0; i < uis.Length; i++)
            {
                if (i != currentIndex)
                {
                   uis[i].UnHighlight();
                }
                else
                {
                    uis[i].Highlight();
                }
            }

            var slotUI = uis[currentIndex] as EquipCharmSlotUI;

            if(slotUI == null)
            {
                return;
            }

            if (slotUI.EquippedCharm == null || !slotUI.EquippedCharm.Unlocked)
            {
                charmDescriptor.ResetDescription();
            }
            else if (slotUI.EquippedCharm != null && slotUI.EquippedCharm.Unlocked)
            {
                charmDescriptor.SetDescription(
                                            slotUI.EquippedCharm.CharmName,
                                            slotUI.EquippedCharm.CharmCost,
                                            slotUI.EquippedCharm.CharmDescription,
                                            slotUI.EquippedCharm.CharmSprite
                                                );
            }
        }
        else
        {
            for (int i = 0; i < uis.Length; i++)
            {
                uis[i].UnHighlight();
            }
        }
}

    public override void UIHover(SelectionUI UI)
    {
        if (!CharmManager.EquippedCharmMode)
        {
            CharmManager.EquippedCharmMode = true;
            CharmManager.OnUIModeChanged.Invoke();
        }

            var slotUI = UI as EquipCharmSlotUI;
            if (slotUI == null)
            {
                return;
            }

        for (int i = 0; i < uis.Length; i++)
        {
            EquipCharmSlotUI slot = uis[i] as EquipCharmSlotUI;
            if (slotUI != slot) slot.UnHighlight();
            else
            {
                currentIndex = i;
                slot.Highlight();
            }

        }


            if (slotUI.EquippedCharm == null)
            {
                charmDescriptor.ResetDescription();
            }
            else if (slotUI.EquippedCharm != null && slotUI.EquippedCharm.Unlocked)
            {
                charmDescriptor.SetDescription(
                                            slotUI.EquippedCharm.CharmName,
                                            slotUI.EquippedCharm.CharmCost,
                                            slotUI.EquippedCharm.CharmDescription,
                                            slotUI.EquippedCharm.CharmSprite
                                            );
            }
    }

    public override void UIClicked(SelectionUI ui)
    {
        //Invoke Event. Will be added.
        //Unequip
        var slotUI = uis[currentIndex] as EquipCharmSlotUI;
        if (slotUI == null)
        {
            Debug.LogError("Equip Charm Inventory must only have EquipCharmSlotUI as child");
            return;
        }
        if (slotUI.EquippedCharm == null) return;
        CharmCostManager.Instance.DecreaseCost(slotUI.EquippedCharm.CharmCost);
        CharmManager.OnCharmUnequipped.Invoke(slotUI.EquippedCharm);
        RemoveCharm();
    }

    private void EquipCharm(Charm charm)
    {
        if (nextEmptySlot >= uis.Length)
        {
            Debug.LogError("The slots for equipping charms are full.");
        }

        var slotUI = uis[nextEmptySlot] as EquipCharmSlotUI;
        if(slotUI == null)
        {
            Debug.LogError("Non-EquippedCharmSlotUI selected. The child must be EquippedCharmSlotUI.");
        }
        else if (slotUI.EquippedCharm == null)
        {
            slotUI.EquipCharm(charm);
            //charm.SetSlotID(nextEmptySlot);
            nextEmptySlot++;
            var nextUI = uis[nextEmptySlot] as EquipCharmSlotUI;
            while (nextUI.EquippedCharm != null)
            {
                nextEmptySlot++;
                if (nextEmptySlot >= uis.Length) break;
                nextUI = uis[nextEmptySlot] as EquipCharmSlotUI;
            }
        }
        else 
        {
            Debug.Log("The slot is occupied.");
        }
    }

    public void InitializeEquipCharmSlots(Charm charm)
    {
        var slotUI = uis[charm.EquippedSlotID] as EquipCharmSlotUI;
        if (slotUI == null)
        {
            Debug.LogError("Non-EquippedCharmSlotUI selected. The child must be EquippedCharmSlotUI.");
        }
        else if (slotUI.EquippedCharm == null)
        {
            slotUI.EquipCharm(charm);
            nextEmptySlot++;
            var nextUI = uis[nextEmptySlot] as EquipCharmSlotUI;
            while (nextUI.EquippedCharm != null)
            {
                nextEmptySlot++;
                if (nextEmptySlot >= uis.Length) break;
                nextUI = uis[nextEmptySlot] as EquipCharmSlotUI;
            }
        }
        else
        {
            Debug.LogError("The slot is already occupied.");
        }
    }

    private void RemoveCharm(int index)
    {
        if(index < 0)
        {
            Debug.LogError("Index cannot be negative in Equip Charm Slots.");
        }

        if (CharmManager.EquippedCharmMode)
        {
            var slotUI = uis[index] as EquipCharmSlotUI;
            if (slotUI == null)
            {
                Debug.LogError("Non-EquippedCharmSlotUI selected. The child must be EquippedCharmSlotUI.");
                return;
            }else if (slotUI.EquippedCharm != null)
            {
                slotUI.RemoveCharm();
                if(nextEmptySlot > index)
                {
                    nextEmptySlot = index;
                }
            }
            else Debug.Log("The slot is empty.");
        }
    }


    private void RemoveCharm()
    {
        if (CharmManager.EquippedCharmMode)
        {
            var slotUI1 = uis[currentIndex] as EquipCharmSlotUI;
            if (slotUI1 == null)
            {
                Debug.LogError("Non-EquippedCharmSlotUI selected. The child must be EquippedCharmSlotUI.");
                return;
            }

            if(slotUI1.EquippedCharm != null)
            {
                slotUI1.RemoveCharm();
                nextEmptySlot--;
            }

            for(int i = 0; i < uis.Length-1; i++)
            {
                slotUI1 = uis[i] as EquipCharmSlotUI;
                var slotUI2 = uis[i+1] as EquipCharmSlotUI;
                if (slotUI1.EquippedCharm == null && slotUI2.EquippedCharm != null)
                {
                    slotUI1.EquipCharm(slotUI2.EquippedCharm);
                    //slotUI2.EquippedCharm.SetSlotID(i);
                    slotUI2.RemoveCharm();
                }
                else break;
            }

        }
    }

    private void RemoveCharm(Charm charm)
    {
        if (!CharmManager.EquippedCharmMode)
        {
            for(int i = 0; i < uis.Length; i++)
            {
                var slotUI = uis[i] as EquipCharmSlotUI;
                if (slotUI == null) return;

                if(slotUI.EquippedCharm == charm)
                {
                    slotUI.RemoveCharm();
                    nextEmptySlot--;
                }
            }

            for (int i = 0; i < uis.Length - 1; i++)
            {
                var slotUI1 = uis[i] as EquipCharmSlotUI;
                var slotUI2 = uis[i + 1] as EquipCharmSlotUI;
                if (slotUI1.EquippedCharm == null && slotUI2.EquippedCharm != null)
                {
                    slotUI1.EquipCharm(slotUI2.EquippedCharm);
                    //slotUI2.EquippedCharm.SetSlotID(i);
                    slotUI2.RemoveCharm();
                }
            }
        }
    }
}
