using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class SkillNodeSelector : MonoBehaviour
{
    [SerializeField]
    private SkillNodeUI parentSkillNodeUI;

    private int currentIndex = 0;

    [SerializeField]
    private List<SkillNodeUI> skillNodes;

    [SerializeField]
    private SkillDescriptor skillDescriptor;
    
    private SkillNodeUI currentNode;

    private bool descriptorOn = false;


    private void Awake()
    {
        if(parentSkillNodeUI == null)
        {
            Debug.LogError("Parent node cannot be null.");
            return;
        }
        currentNode = parentSkillNodeUI;
        for (int i = 0; i < skillNodes.Count; i++)
        {
            Debug.Log($"Skill Node {i}:");
            Debug.Log(skillNodes[i] == null);
        }
        UIHover();
    }

    private void Start()
    {
        skillDescriptor.gameObject.SetActive(false);
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!descriptorOn)
            {
                skillDescriptor.gameObject.SetActive(true);
                descriptorOn = true;
            }
            else
            {
                skillDescriptor.gameObject.SetActive(false);
                descriptorOn = false;
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
            if (currentIndex < skillNodes.Count-1)
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
                //Debug.Log($"Current Index: {currentIndex}");
                UIHover();
            }


        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentNode.SkillNode.Children.Count > 0)
            {
                currentNode = currentNode.SkillNode.Children[0];
                currentIndex = skillNodes.IndexOf(currentNode);
                //Debug.Log($"Current Index: {currentIndex}");
                UIHover();
            }


        }
    }


    public virtual void UIHover()
    {
        for (int i = 0; i < skillNodes.Count; i++)
        {
            if (i != currentIndex)
            {
                Debug.Log(i);
                Debug.Log(skillNodes[i] == null);
                skillNodes[i].UnHighlight();
            }
            else
            {
                Debug.Log(i);
                skillNodes[i].Highlight();
            }
        }

        if (currentNode == null) return;

        if (currentNode.SkillNode != null)
        {
            skillDescriptor.SetDescription(
                currentNode.SkillNode.SkillData.UpgradeName,
                currentNode.SkillNode.SkillData.Value,
                currentNode.SkillNode.SkillData.Description,
                currentNode.SkillNode.SkillData.UpgradeSprite
            );
        }
        else
        {
            skillDescriptor.ResetDescription();
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

        if (currentNode == null) return;
        if (currentNode.SkillNode != null)
        {
            skillDescriptor.SetDescription(
                currentNode.SkillNode.SkillData.UpgradeName,
                currentNode.SkillNode.SkillData.Value,
                currentNode.SkillNode.SkillData.Description,
                currentNode.SkillNode.SkillData.UpgradeSprite
            );
        }
        else
        {
            skillDescriptor.ResetDescription();
        }

    }

    
}
