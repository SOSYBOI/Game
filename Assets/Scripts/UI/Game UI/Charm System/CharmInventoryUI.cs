using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CharmInventoryUI : SelectorManager
{
    [SerializeField]
    private CharmDescriptor charmDescriptor;

    [SerializeField]
    private Scrollbar charmScrollbar;

    [SerializeField]
    private Transform viewPort;

    //This variable is for checking if the selected charm slot is out of the viewport.
    //We only look at the vertical distance
    [SerializeField]
    private float slotMaxDistance;

    [SerializeField]
    private float sliderMoveScale;

    protected override void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {

        CharmManager.OnCharmUnequipped += UpdateUnequippedCharm;
        CharmManager.OnUIModeChanged += UIHover;
        currentIndex = 0;
        if (uis.Length < 2)
        {
            Debug.LogError("Menu has only one option");
            return;
        }
        for (int i = 0; i < uis.Length; i++)
        {
            uis[i].UnHighlight();
        }
    }

    protected override void Update()
    {
        if (CharmManager.EquippedCharmMode) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            var slotUI = uis[currentIndex] as CharmSlotUI;
            if(slotUI == null)
            {
                Debug.LogError("Charm Inventory must only have CharmSlotUI as child");
                return;
            }
            if(slotUI.Charm == null)
            {
                Debug.LogWarning("Slot cannot have empty charm");
                return;
            }
            if (!slotUI.Charm.Unlocked)
            {
                Debug.Log("Charm is not unlocked.");
                return;
            }

            //Equip
            if (!slotUI.Equipped && CharmCostManager.Instance.CheckValidCost(slotUI.Charm.CharmCost))
            {
                Debug.Log($"Equip Charm {slotUI.Charm.CharmName}");

                CharmManager.OnCharmEquipped.Invoke(slotUI.Charm);
                slotUI.OnEquipped();
                CharmCostManager.Instance.IncreaseCost(slotUI.Charm.CharmCost);
            }
            //Remove
            else
            {
                Debug.Log($"Remove Charm {slotUI.Charm.CharmName}");

                CharmManager.OnCharmUnequipped.Invoke(slotUI.Charm);
                CharmCostManager.Instance.DecreaseCost(slotUI.Charm.CharmCost);
            }
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
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentIndex >= columns)
            {
            currentIndex = currentIndex - columns;
            }
            else if(currentIndex < columns)
            {
                CharmManager.EquippedCharmMode = true;
                CharmManager.OnUIModeChanged.Invoke();
            }
            UIHover();


        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(!CharmManager.EquippedCharmMode)
            {
                if (currentIndex + columns < uis.Length)
                {
                    currentIndex = currentIndex + columns;
                }
                UIHover();  
            }



        }
    }

    public void OnItemUnselected()
    {
        charmDescriptor.ResetDescription();
    }

    public override void UIHover()
    {
        if (!CharmManager.EquippedCharmMode)
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

            var slotUI = uis[currentIndex] as CharmSlotUI;
            if (slotUI == null)
            {
                return;
            }

            if (slotUI.Charm == null || !slotUI.Charm.Unlocked)
            {
                charmDescriptor.ResetDescription();
            }
            else if (slotUI.Charm != null && slotUI.Charm.Unlocked)
            {
                charmDescriptor.SetDescription(
                                            slotUI.Charm.CharmName,
                                            slotUI.Charm.CharmCost,
                                            slotUI.Charm.CharmDescription,
                                            slotUI.Charm.CharmSprite
                                            );
            }
            else
            {
                Debug.LogWarning("Slot selected but Charm cannot be null.");
            }

            float distance = viewPort.position.y-slotUI.transform.position.y;
            int count = 0;
            //If the slot is out of viewport
            if (distance*distance > slotMaxDistance*slotMaxDistance)
            {
                //the slot is lower than the viewport boundary
                if (distance > 0 && charmScrollbar.value - sliderMoveScale>0)
                {
                    while(distance * distance > slotMaxDistance * slotMaxDistance)
                    {
                        charmScrollbar.value -= sliderMoveScale;
                        distance = viewPort.position.y - slotUI.transform.position.y;
                        count++;
                        if (count > 10) {
                            Debug.LogError("Unexpected Issue from Sliding Charm Slot");
                            break; 
                        }
                    }
                }
                else if(distance < 0 && charmScrollbar.value + sliderMoveScale <= 1)//the slot is higher than the viewport boundary
                {
                    while(distance * distance > slotMaxDistance * slotMaxDistance)
                    {
                        charmScrollbar.value += sliderMoveScale;
                        distance = viewPort.position.y - slotUI.transform.position.y;
                        count++;
                        if (count > 10)
                        {
                            Debug.LogError("Unexpected Issue from Sliding Charm Slot");
                            break;
                        }
                    }
                }
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

    public override void UIHover(SelectionUI ui)
    {
        if (CharmManager.EquippedCharmMode)
        {
            CharmManager.EquippedCharmMode = false;
            CharmManager.OnUIModeChanged.Invoke();
        }
            var slotUI = ui as CharmSlotUI;
            if (slotUI == null)
            {
                return;
            }
            for(int i = 0;i<uis.Length;i++)
            {
                CharmSlotUI slot = uis[i] as CharmSlotUI;
                if (slotUI != slot) slot.UnHighlight();
                else { 
                    slot.Highlight(); 
                    currentIndex = i;
                }
            }


            if (slotUI.Charm == null || !slotUI.Charm.Unlocked)
            {
                charmDescriptor.ResetDescription();
            }
            else if (slotUI.Charm != null && slotUI.Charm.Unlocked)
            {
                charmDescriptor.SetDescription(
                                            slotUI.Charm.CharmName,
                                            slotUI.Charm.CharmCost,
                                            slotUI.Charm.CharmDescription,
                                            slotUI.Charm.CharmSprite
                                            );
            }
            else
            {
                Debug.LogError("Slot selected but Charm cannot be null.");
            }


        
    }

    public override void UIClicked(SelectionUI ui)
    {
        var slotUI = uis[currentIndex] as CharmSlotUI;
        if (slotUI == null)
        {
            Debug.LogError("Charm Inventory must only have CharmSlotUI as child");
            return;
        }
        if (slotUI.Charm == null)
        {
            Debug.LogWarning("Slot cannot have empty charm");
            return;
        }
        if (!slotUI.Charm.Unlocked)
        {
            Debug.Log("Charm is not unlocked.");
            return;
        }

        //Equip
        if (!slotUI.Equipped && CharmCostManager.Instance.CheckValidCost(slotUI.Charm.CharmCost))
        {
            Debug.Log($"Equip Charm {slotUI.Charm.CharmName}");

            CharmManager.OnCharmEquipped.Invoke(slotUI.Charm);
            slotUI.OnEquipped();
            CharmCostManager.Instance.IncreaseCost(slotUI.Charm.CharmCost);
        }
        //Remove
        else
        {
            Debug.Log($"Remove Charm {slotUI.Charm.CharmName}");

            CharmManager.OnCharmUnequipped.Invoke(slotUI.Charm);
            CharmCostManager.Instance.DecreaseCost(slotUI.Charm.CharmCost);
        }
    }

    private void UpdateEquippedCharm()
    {
        var slotUI = uis[currentIndex] as CharmSlotUI;
        slotUI.OnEquipped();
    }

    private void UpdateEquippedCharm(Charm charm)
    {
        for (int i = 0; i < uis.Length; i++)
        {
            var slotUI = uis[i] as CharmSlotUI;
            if (slotUI == null) return;
            if (slotUI.Charm == charm)
            {
                slotUI.OnEquipped();
            }
        }
    }

    private void UpdateUnequippedCharm()
    {
        var slotUI = uis[currentIndex] as CharmSlotUI;
        slotUI.OnUnequipped();
    }

    //Assume that all Charms are unique.
    private void UpdateUnequippedCharm(Charm charm)
    {
        for (int i = 0; i < uis.Length; i++)
        {
            var slotUI = uis[i] as CharmSlotUI;
            if (slotUI == null) return;
            if (slotUI.Charm != null && slotUI.Charm == charm)
            {
                slotUI.OnUnequipped();
                break;
            }
        }
    }

    //This is used to load the equipped charms (Basically save and load things.)
    public List<Charm> GetEquippedCharms()
    {
        List<Charm> charms = new List<Charm>();
        for(int i = 0; i < uis.Length; i++)
        {
            var slotUI = uis[i] as CharmSlotUI;
            if (slotUI == null)
            {
                Debug.LogError("Equip Charm Inventory must only have EquipCharmSlotUI as child");
                return null;
            }
            if (slotUI.Charm != null && slotUI.Equipped)
            {
                charms.Add(slotUI.Charm);
            }
        }

        return charms;
    }
}
