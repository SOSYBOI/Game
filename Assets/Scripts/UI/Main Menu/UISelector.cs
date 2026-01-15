using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelector : MonoBehaviour
{
    [SerializeField]
    private MenuUI[] uis; 

    // Start is called before the first frame update
    void Start()
    {
        if (uis.Length < 2)
        {
            Debug.LogError("Menu has only one option");
            return;
        }
        for(int i = 1; i < uis.Length; i++)
        {
            uis[i].UnHighlight();
        }
        uis[0].Highlight();
    }

    public void UIHover(MenuUI UI)
    {
        foreach (MenuUI ui in uis)
        {
            if(ui != UI)
            {
                ui.UnHighlight();
            }
            else
            {
                ui.Highlight();
            }
        }
    }

    public void UIClicked(MenuUI ui)
    {
        switch (ui.Command)
        {
            case Command.START:
                Debug.Log("Start Game.");
                break;
            case Command.SETTING:
                Debug.Log("Setting.");
                break;
            case Command.EXIT:
                Debug.Log("Exit Game.");
                break;
            case Command.YES:
                Debug.Log("YES");
                break;
            case Command.NO:
                Debug.Log("NO");
                break;
        }
    }
}
