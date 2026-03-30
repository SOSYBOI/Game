using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menuPanel;

    [SerializeField]
    private GameObject savePanel;

    [SerializeField]
    private GameObject settingPanel;

    [SerializeField]
    private GameObject confirmPanel;

    public static MenuManager Instance;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        menuPanel.SetActive(true);
        savePanel.SetActive(false);
        settingPanel.SetActive(false);
        confirmPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenPanel(MenuCommand.RETURN);
        }
    }

    public void OpenPanel(MenuCommand command)
    {
        switch (command)
        {
            case MenuCommand.START:
                Debug.Log("Start Game.");

                menuPanel.SetActive(false);
                savePanel.SetActive(true);

                break;
            case MenuCommand.SETTING:
                Debug.Log("Setting.");

                menuPanel.SetActive(false);
                settingPanel.SetActive(true);

                break;
            case MenuCommand.EXIT:
                Debug.Log("Exit Game.");

                confirmPanel.SetActive(true);

                break;
            case MenuCommand.RETURN:
                Debug.Log("Return to Menu");

                menuPanel.SetActive(true);
                savePanel.SetActive(false);
                settingPanel.SetActive(false);
                confirmPanel.SetActive(false);

                break;
            default:
                Debug.LogError("Unknown MenuCommand. Please handle such case.");

                break;
        }
    }
}
