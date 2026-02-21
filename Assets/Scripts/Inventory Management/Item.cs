using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Item",menuName = "Item/New Item")]
public class Item : ScriptableObject
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


    public string ItemName => itemName;
    public Sprite ItemSprite => itemSprite;
    public string ItemDescription => itemDescription;
    public int ItemID => itemID;
    public bool Unlocked => unlocked;

    public void SetUnlock()
    {
        unlocked = true;
    }
}
