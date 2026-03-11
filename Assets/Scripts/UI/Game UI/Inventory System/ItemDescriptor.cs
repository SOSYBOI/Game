using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptor : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private TextMeshProUGUI itemDescription;
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private Image interactImage;
    [SerializeField]
    private TextMeshProUGUI interactText;


    private void Start()
    {
        ResetDescription();
    }

    public void SetDescription(ItemData item,bool equipped)
    {
        itemImage.gameObject.SetActive(true);
        itemName.text = "Name: " + item.ItemName;
        itemDescription.text = "Description: " + item.ItemDescription;
        itemImage.sprite = item.ItemSprite;

        bool equippable = item is EquippableItem;

        if(equippable)
        {
            interactImage.gameObject.SetActive(true);
        }
        else
        {
            interactImage.gameObject.SetActive(false);
        }

        Debug.Log(equipped);

        if (equipped)
        {
            interactImage.color = new Color(0.5f,0.5f,0.5f);
            interactText.color = new Color(0.5f, 0.5f, 0.5f);
        }
        else
        {
            interactImage.color = Color.white;
            interactText.color = Color.white;
        }

    }

    public void ResetDescription()
    {
        itemName.text = "Name: ";
        itemDescription.text = "Description: ";
        itemImage.sprite = null;
        itemImage.gameObject.SetActive(false);
        interactImage.gameObject.SetActive(false);
    }

    public void ResetDescription(ItemType type, bool unlocked, bool equippable)
    {
        ResetDescription();

        if (equippable)
        {
            interactImage.gameObject.SetActive(true);
            interactImage.color = new Color(0.5f, 0.5f, 0.5f);
        }
        else
        {
            interactImage.gameObject.SetActive(false);
        }
    }
}
