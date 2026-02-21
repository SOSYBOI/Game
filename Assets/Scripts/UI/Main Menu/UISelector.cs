using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UISelector : SelectorManager
{

    public override void UIClicked(SelectionUI ui)
    {
        var menuUI = ui as MenuUI;
        if (menuUI == null) Debug.LogError("The UI type must be MenuUIType!");
        switch (menuUI.Command)
        {
            case MenuCommand.START:
                Debug.Log("Start Game.");
                break;
            case MenuCommand.SETTING:
                Debug.Log("Setting.");
                break;
            case MenuCommand.EXIT:
                Debug.Log("Exit Game.");
                break;
            case MenuCommand.YES:
                Debug.Log("YES");
                break;
            case MenuCommand.NO:
                Debug.Log("NO");
                break;
        }
    }
}
