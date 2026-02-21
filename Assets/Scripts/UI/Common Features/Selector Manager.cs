using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectorManager : MonoBehaviour
{
    [SerializeField]
    protected SelectionUI[] uis;

    protected int currentIndex = 0;

    // Start is called before the first frame update
    private void Start()
    {
        if (uis.Length < 2)
        {
            Debug.LogError("Menu has only one option");
            return;
        }
        for (int i = 1; i < uis.Length; i++)
        {
            uis[i].UnHighlight();
        }
        uis[0].Highlight();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentIndex == 0)
            {
                currentIndex = uis.Length - 1;
            }
            else
            {
                currentIndex--;
            }
            UIHover();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentIndex == uis.Length - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }

            UIHover();
        }
    }

    public virtual void UIHover()
    {
        for (int i = 0; i < uis.Length; i++)
        {
            if (i != currentIndex)
            {
                uis[i].UnHighlight();
            }
            else
            {
                uis[i].Highlight();
            }
        }
    }

    public virtual void UIHover(SelectionUI UI)
    {
        for (int i = 0; i < uis.Length; i++)
        {
            if (uis[i] != UI)
            {
                uis[i].UnHighlight();
            }
            else
            {
                currentIndex = i;
                uis[i].Highlight();
            }
        }
    }

    public abstract void UIClicked(SelectionUI ui);
}
