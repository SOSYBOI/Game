using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingTab : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private SettingPanel settingPanel;

    [SerializeField]
    private TextMeshProUGUI settingText;

    /// <summary>
    /// selectedColors[0]: White (Unselected)
    /// selectedColors[1]: Yellow (Selected)
    /// </summary>
    [SerializeField]
    private Color[] selectedColors;

    private SettingSelector parentSelector;

    // Start is called before the first frame update
    private void Start()
    {
        if (settingPanel == null)
        {
            Debug.LogError("Setting Panel cannot be null.");
            return;
        }

        parentSelector = transform.parent.GetComponent<SettingSelector>();
        if(parentSelector == null)
        {
            Debug.LogError("The parent must have Setting Selector Component");
            return;
        }
    }

    public void OpenPanel()
    {
        settingText.color = selectedColors[1];
        settingPanel.gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
        settingText.color = selectedColors[0];
        settingPanel.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        parentSelector?.UIClicked(this);
    }

}
