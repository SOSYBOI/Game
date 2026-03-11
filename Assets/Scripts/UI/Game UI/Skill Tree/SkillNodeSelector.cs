using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillNodeSelector : MonoBehaviour
{
    [SerializeField]
    private SkillNodeUI parentSkillNodeUI;

    private int currentIndex = 0;

    [SerializeField]
    private List<SkillNodeUI> skillNodes;
    private SkillNodeUI currentNode;



    private void Awake()
    {
        if(parentSkillNodeUI == null)
        {
            Debug.LogError("Parent node cannot be null.");
            return;
        }
        currentNode = parentSkillNodeUI;
        InitializeNodes(parentSkillNodeUI);
        UIHover();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(currentNode != null)
            {
                Debug.Log("Upgrade");
                currentNode.Upgrade();
            }
                
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(currentIndex > 0)
            {
                currentIndex--;
                currentNode = skillNodes[currentIndex];
                Debug.Log($"Current Index: {currentIndex}");
                UIHover();
            }

        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentIndex < skillNodes.Count)
            {
                currentIndex++;
                currentNode = skillNodes[currentIndex];
                Debug.Log($"Current Index: {currentIndex}");
                UIHover();
            }


        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentNode.SkillNode.ParentNode)
            {
                currentNode = currentNode.SkillNode.ParentNode;
                currentIndex = skillNodes.IndexOf(currentNode);
                Debug.Log($"Current Index: {currentIndex}");
                UIHover();
            }


        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentNode.SkillNode.Children.Count > 0)
            {
                currentNode = currentNode.SkillNode.Children[0];
                currentIndex = skillNodes.IndexOf(currentNode);
                Debug.Log($"Current Index: {currentIndex}");
                UIHover();
            }


        }
    }

    private void InitializeNodes(SkillNodeUI currentParentNode)
    {

        if (currentParentNode == null || currentParentNode.SkillNode == null) return;
        if(currentParentNode.SkillNode.Children.Count == 0) return;

        for(int i = 0;i<currentParentNode.SkillNode.Children.Count; i++)
        {
            InitializeNodes(currentParentNode.SkillNode.Children[i]);
        }

    }


    public virtual void UIHover()
    {
        for (int i = 0; i < skillNodes.Count; i++)
        {
            if (i != currentIndex)
            {
                skillNodes[i].UnHighlight();
            }
            else
            {
                //Debug.Log(i);
                skillNodes[i].Highlight();
            }
        }
    }

    public void UIHover(SkillNodeUI UI)
    {
        for (int i = 0; i < skillNodes.Count; i++)
        {
            if (skillNodes[i] != UI)
            {
                skillNodes[i].UnHighlight();
            }
            else
            {
                currentIndex = i;
                currentNode = skillNodes[i];
                skillNodes[i].Highlight();
            }
        }
    }

    
}
