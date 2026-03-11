using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CharmDescriptor : MonoBehaviour
{
    [SerializeField]
    private Image charmImage;
    [SerializeField]
    private TextMeshProUGUI charmName;
    [SerializeField]
    private TextMeshProUGUI charmCost;
    [SerializeField]
    private TextMeshProUGUI charmDescription;

    private void Start()
    {
        ResetDescription();
    }

    public void SetDescription(string name, int cost, string desc, Sprite charmSprite)
    {
        charmImage.gameObject.SetActive(true);
        charmName.text = "Name: " + name;
        charmCost.text = "Charm Cost: " + cost.ToString();
        charmDescription.text = "Description: " + desc;
        charmImage.sprite = charmSprite;
    }

    public void ResetDescription()
    {
        charmName.text = "Name:";
        charmCost.text = "Charm Cost:";
        charmDescription.text = "Description:";
        charmImage.sprite = null;
        charmImage.gameObject.SetActive(false);
    }
}
