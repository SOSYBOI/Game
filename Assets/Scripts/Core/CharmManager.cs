using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
public class CharmManager : MonoBehaviour
{
    public static UnityAction<Charm> OnCharmEquipped;
    public static UnityAction<Charm> OnCharmUnequipped;
    public static UnityAction OnUIModeChanged;
    public static bool EquippedCharmMode = true;

    [SerializeField]
    private CharmInventoryUI charmInventoryUI;

    [SerializeField]
    private EquipCharmInventoryUI equipCharmInventoryUI;

    [SerializeField]
    private CharmCostManager costManager;

    private void Awake()
    {
        UpdateCharm();
    }


    private void UpdateCharm()
    {
        int totalCost = 0;
        List<Charm> charms = charmInventoryUI.GetEquippedCharms();
        foreach (Charm charm in charms)
        {
            equipCharmInventoryUI.InitializeEquipCharmSlots(charm);
            totalCost += charm.CharmCost;
        }
        costManager.InitializeUI(totalCost);
    }
}
