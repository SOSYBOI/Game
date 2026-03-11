using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionUI : MonoBehaviour
{
    /// <summary>
    /// potionSprites[0]: Full Potion
    /// potionSprites[1]: 2/3 Potion
    /// potionSprites[2]: 1/3 Potion
    /// potionSprites[3]: Empty Potion
    /// </summary>
    [SerializeField]
    private Sprite[] potionSprites;
    [SerializeField]
    private Image potionImage;
    private int count = 0;

    public void OnUsedPotion()
    {
        count++;
        potionImage.sprite = potionSprites[count];
    }

    public void ResetPotion()
    {
        count = 0;
        potionImage.sprite = potionSprites[0];
    }
}
