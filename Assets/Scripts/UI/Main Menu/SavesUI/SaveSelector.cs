using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is used to select the saves.
/// </summary>
public class SaveSelector : SelectorManager
{
    public override void UIClicked(SelectionUI ui)
    {
        var saveUI = ui as SaveUI;
    }
}
