using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This is used to select the saves.
/// </summary>
public class SaveSelector : SelectorManager
{
    public override void UIClicked(SelectionUI ui)
    {
        var saveUI = ui as SaveUI;

        SceneManager.LoadScene("Bootstrap");
    }
}
