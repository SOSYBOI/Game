using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSelector : MonoBehaviour
{
    [SerializeField]
    private List<SettingTab> tabs;
    private SettingTab currentTab;
    private int currentIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        if (tabs == null)
        {
            Debug.LogError("It must at least have one tab.");
            return;
        }
        currentTab = tabs[0];
        InitializeUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if(currentIndex > 0)
            {
                tabs[currentIndex].ClosePanel();
                currentIndex--;
                tabs[currentIndex].OpenPanel();
            }

        }else if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentIndex < tabs.Count - 1)
            {
                tabs[currentIndex].ClosePanel();
                currentIndex++;
                tabs[currentIndex].OpenPanel();
            }
        }
    }

    public void UIClicked(SettingTab tab)
    {
        tab.ClosePanel();
        currentTab = tab;
        currentTab.OpenPanel();
    }

    private void InitializeUI()
    {
        tabs[currentIndex].OpenPanel();

        for (int i = 1; i < tabs.Count; i++)
        {
            tabs[i].ClosePanel();
        }

    }

}
