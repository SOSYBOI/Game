using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTabSystem : MonoBehaviour
{
    [SerializeField]
    private List<SkillTabButton> tabButtons;
    [SerializeField]
    private List<SkillPanel> tabPanels;
    [SerializeField]
    private Sprite[] tabSprites = new Sprite[2];

    private byte currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < tabButtons.Count; i++)
        {
            int index = i;

            tabButtons[i].Button.onClick.AddListener(() => SwitchPanel(index));
        }
        SwitchPanel();
    }

    void SwitchPanel()
    {
        for (int i = 0; i < tabButtons.Count; i++)
        {
            if (i == currentIndex) OpenPanel(i);
            else
            {
                ClosePanel(i);
                tabButtons[i].enabled = true;
            }
        }
        tabButtons[currentIndex].enabled = false;

        //ToolTipSystem.Instance.CloseToolTip();
    }

    void SwitchPanel(int index)
    {
        //Debug.Log($"Button {index} pressed.");
        currentIndex = (byte)index;
        for (int i = 0; i < tabButtons.Count; i++)
        {
            if (i == currentIndex) OpenPanel(i);
            else
            {
                ClosePanel(i);
                tabButtons[i].enabled = true;
            }
        }
        tabButtons[index].enabled = false;

        //ToolTipSystem.Instance.CloseToolTip();
    }

    void OpenPanel(int index)
    {
        tabButtons[index].OnSelect();
        Debug.Log("Panel");
        Debug.Log(tabPanels[index] == null);
        tabPanels[index].gameObject.SetActive(true);
    }

    void ClosePanel(int index)
    {
        tabButtons[index].OnUnselect();
        tabPanels[index].gameObject.SetActive(false);
    }
}
