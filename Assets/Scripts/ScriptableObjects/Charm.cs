using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Charm", menuName = "Charm/New Charm")]
public class Charm : ScriptableObject
{
    [SerializeField]
    private string charmName;
    [SerializeField]
    private Sprite charmSprite;
    [SerializeField]
    private int charmCost;
    [SerializeField]
    private string charmDescription;
    [SerializeField]
    private int charmID;
    [SerializeField]
    private bool unlocked = false;
    [SerializeField]
    private bool equipped = false;
    [SerializeField]
    private int equippedSlotID = -1;


    public string CharmName => charmName;
    public Sprite CharmSprite => charmSprite;
    public int CharmCost => charmCost;
    public string CharmDescription => charmDescription;
    public int CharmID => charmID;
    public bool Unlocked => unlocked;
    public bool Equipped => equipped;
    public int EquippedSlotID => equippedSlotID;

    public void SetUnlock()
    {
        unlocked = true;
    }

    public void SetEquipped(bool equipped)
    {
        this.equipped = equipped;
    }

    //This is used to set which EquipCharmSlot
    public void SetSlotID(int id)
    {
        equippedSlotID = id;
    }
}
