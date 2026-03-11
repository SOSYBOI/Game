using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI coinCount;

    public void updateCount(int count)
    {
        coinCount.text = count.ToString();
    }
}
