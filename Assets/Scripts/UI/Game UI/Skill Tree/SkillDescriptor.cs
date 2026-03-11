using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillDescriptor : MonoBehaviour
{
    [SerializeField]
    private Image skillImage;
    [SerializeField]
    private TextMeshProUGUI skillName;
    [SerializeField]
    private TextMeshProUGUI charmCost;
    [SerializeField]
    private TextMeshProUGUI skillDescription;

    public void SetDescription(string name, string cost, string desc, Sprite charmSprite)
    {
        skillImage.gameObject.SetActive(true);
        skillName.text = name;
        skillDescription.text = desc;
        skillImage.sprite = charmSprite;
    }

    public void ResetDescription()
    {
        skillName.text = "";
        skillDescription.text = "";
        skillImage.sprite = null;
        skillImage.gameObject.SetActive(false);
    }
}
