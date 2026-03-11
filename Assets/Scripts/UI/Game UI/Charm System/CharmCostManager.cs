using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharmCostManager : MonoBehaviour
{
    public static CharmCostManager Instance;

    //If you want to make the max cost increase I have made a prefab for you to work on it.
    [SerializeField]
    private Image[] costUI;
    /// <summary>
    /// costSprites[0]: Cost not used.
    /// costSprites[1]: Cost used up.
    /// </summary>
    [SerializeField]
    private Sprite[] costSprites;
    private int costCount = 0;
    [SerializeField]
    private int maxCost;


    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    public bool CheckValidCost(int cost)
    {
        return cost+costCount <= maxCost;
    }

    public void IncreaseCost(int cost)
    {
        if (!CheckValidCost(cost))
        {
            Debug.LogError("Cost cannot be larger than maximum cost.");
            return;
        }

        for(int i = costCount; i < costCount + cost; i++)
        {
            costUI[i].sprite = costSprites[1];
        }
        costCount += cost;

        Debug.Log($"Charm Cost: {costCount}");
    }

    public void DecreaseCost(int cost)
    {
        Debug.Log($"Total Count: {costCount}");
        Debug.Log($"Decrease Amount: {cost}");
        for(int i = costCount - 1; i>costCount-cost-1; i--)
        {
            Debug.Log(i);
            costUI[i].sprite = costSprites[0];
        }

        costCount -= cost;

        Debug.Log($"Charm Cost: {costCount}");
    }

    public void InitializeUI(int totalCost)
    {
        costCount = totalCost;
        for(int i = 0; i < maxCost; i++)
        {
            if (i < totalCost)
                costUI[i].sprite = costSprites[1];
            else
                costUI[i].sprite = costSprites[0];
        }
    }
}
