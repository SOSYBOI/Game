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
    private TextMeshProUGUI skillCost;
    [SerializeField]
    private TextMeshProUGUI skillDescription;

    public void SetDescription(string name, float cost, string desc, Sprite skillSprite)
    {
        skillImage.gameObject.SetActive(true);
        skillName.text = "Upgrade name: "+name;
        skillCost.text = "Skill Cost: "+cost.ToString();
        skillDescription.text = "Description: "+desc;
        skillImage.sprite = skillSprite;
        skillImage.preserveAspect = true;
    }

    public void ResetDescription()
    {
        skillName.text = "Upgrade name: ";
        skillCost.text = "Skill Cost: ";
        skillDescription.text = "Description: ";
        skillImage.sprite = null;
        skillImage.gameObject.SetActive(false);
    }
}
