using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MenuCommand
{
    START,
    SETTING,
    EXIT,
    RETURN
}

public class MenuUI : SelectionUI
{
    [SerializeField]
    private MenuCommand command;
    public MenuCommand Command => command;
}
