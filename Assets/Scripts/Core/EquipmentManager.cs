using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EquipmentManager : MonoBehaviour
{
    public static UnityAction<EquippableItem> OnEquipmentEquipped;
    public static UnityAction<EquippableItem> OnEquipmentUnequipped;
    public static UnityAction<EquippableItem> OnEquipmentSwapped;
}
