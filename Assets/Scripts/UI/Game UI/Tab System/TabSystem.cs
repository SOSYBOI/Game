using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// As seen in Progress report, 
/// tabPanels[0] is Equipment,
/// tabPanels[1] is Skill Tree,
/// tabPanels[2] is Charm,
/// tabPanels[3] is Inventory,
/// tabPanels[4] is Settings,
/// tabPanels[5] is Exit.
/// </summary>
public class TabSystem : MonoBehaviour
{
    [SerializeField]
    private Button[] tabButtons = new Button[6];
    [SerializeField]
    private TabPanel[] tabPanels = new TabPanel[6];
    [SerializeField]
    private Image tabImage;
    [SerializeField]
    private Sprite[] tabSprites = new Sprite[6];
    [SerializeField]
    private byte currentIndex = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i<tabButtons.Length;i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(()=> SwitchPanel(index));
        }
        SwitchPanel();
    }

    void SwitchPanel()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (i == currentIndex) OpenPanel(i);
            else { 
                ClosePanel(i); 
                tabButtons[i].enabled = true; 
            }
        }
        tabImage.sprite = tabSprites[currentIndex];
        tabButtons[currentIndex].enabled = false;

        //ToolTipSystem.Instance.CloseToolTip();
    }

    void SwitchPanel(int index)
    {
        //Debug.Log($"Button {index} pressed.");
        currentIndex = (byte)index;
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (i == currentIndex) OpenPanel(i);
            else { 
                ClosePanel(i);
                tabButtons[i].enabled = true;
            }
        }
        tabImage.sprite = tabSprites[index];
        tabButtons[index].enabled = false;

        //ToolTipSystem.Instance.CloseToolTip();
    }

    void OpenPanel(int index)
    {
        tabPanels[index].gameObject.SetActive(true);
    }

    void ClosePanel(int index)
    {
        tabPanels[index].gameObject.SetActive(false);
    }
}
