using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SkillTabButton : MonoBehaviour
{
    /// <summary>
    /// tabSprites[0] Selected Tab Sprite,
    /// tabSprites[1] Unselected Tab Sprite.
    /// </summary>
    [SerializeField]
    private Sprite[] tabSprites;

    [SerializeField]
    private float[] originalXPositions;

    [SerializeField]
    private RectTransform tabIcon;

    private Image tabImage;
    private Button button;

    public Button Button => button;

    private void Awake()
    {
        button = GetComponent<Button>();
        tabImage = GetComponent<Image>();
    }

    public void OnSelect()
    {
        tabIcon.anchoredPosition = new Vector2(originalXPositions[1], tabIcon.anchoredPosition.y);
        tabImage.sprite = tabSprites[0];
    }

    public void OnUnselect()
    {
        tabIcon.anchoredPosition = new Vector2(originalXPositions[0], tabIcon.anchoredPosition.y);
        tabImage.sprite = tabSprites[1];
    }
}
