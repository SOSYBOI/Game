using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuUISelector : SelectorManager
{

    public override void UIClicked(SelectionUI ui)
    {
        var menuUI = ui as MenuUI;
        if (menuUI == null) Debug.LogError("The UI type must be MenuUIType!");

        MenuManager.Instance.OpenPanel(menuUI.Command);
    }
}
