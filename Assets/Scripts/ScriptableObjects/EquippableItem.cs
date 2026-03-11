using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType { 
    HELMET,
    CHESTPLATE,
    LEGGINGS,
    BOOTS,
    WEAPON
}

[CreateAssetMenu(fileName = "EquippableItem", menuName = "Item/New EquippableItem")]
public class EquippableItem : ItemData
{
    [SerializeField]
    private EquipmentType equipmentType;

    public EquipmentType EquipmentType => equipmentType;

    private bool isEquipped = false;
    public bool IsEquipped => isEquipped;
}
