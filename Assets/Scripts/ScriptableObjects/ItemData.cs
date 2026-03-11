using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    NORMAL,
    ARMOR,
    WEAPON
}

[CreateAssetMenu(fileName="Item",menuName = "Item/New Item")]
public class ItemData : ScriptableObject
{
    [SerializeField]
    private string itemName;
    [SerializeField]
    private Sprite itemSprite;
    [SerializeField]
    private string itemDescription;
    [SerializeField] 
    private int itemID;
    [SerializeField]
    private bool unlocked;
    [SerializeField]
    private ItemType itemType;


    public string ItemName => itemName;
    public Sprite ItemSprite => itemSprite;
    public string ItemDescription => itemDescription;
    public int ItemID => itemID;
    public bool Unlocked => unlocked;
    public ItemType ItemType => itemType;

    public void SetUnlock()
    {
        unlocked = true;
    }
}
