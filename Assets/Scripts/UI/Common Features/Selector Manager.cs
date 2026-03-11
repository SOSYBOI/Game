using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectorManager : MonoBehaviour
{
    [SerializeField]
    protected SelectionUI[] uis;

    [SerializeField]
    protected int columns=1;

    protected int currentIndex = 0;

    // Start is called before the first frame update
    protected virtual void Start()
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
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
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
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
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

        }else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentIndex < columns)
            {
                currentIndex = (uis.Length - columns + currentIndex );
            }
            else
            {
                currentIndex = currentIndex - columns;
            }

            UIHover();

        }
        else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentIndex+columns >= uis.Length)
            {
                currentIndex = currentIndex+columns - uis.Length;
            }
            else
            {
                currentIndex = currentIndex + columns;
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
