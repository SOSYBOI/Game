using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTipUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI descriptionName;
    [SerializeField]
    private TextMeshProUGUI description;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEquipmentDescription(string name, string description)
    {

    }

    public void SetSkillDescription(string name, string cost)
    {
        descriptionName.text = "Name: " + name;
        this.description.text = "Cost: " + cost;
    }

    public void SetDescription(string name,string description)
    {
        descriptionName.text = "Name: "+name;
        this.description.text = "Description: "+description;
    }

    public void ResetDescription()
    {
        descriptionName.text = "Name: ";
        this.description.text = "Description: ";
    }
}
