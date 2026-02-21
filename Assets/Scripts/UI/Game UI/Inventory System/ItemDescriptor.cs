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

    public void SetDescription(string name, string desc, Sprite itemSprite)
    {
        itemImage.gameObject.SetActive(true);
        itemName.text = name;
        itemDescription.text = desc;
        itemImage.sprite = itemSprite;
    }

    public void ResetDescription()
    {
        itemName.text = "";
        itemDescription.text = "";
        itemImage.sprite = null;
        itemImage.gameObject.SetActive(false);
    }
}
